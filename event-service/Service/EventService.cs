using event_service.DTO;
using event_service.Interface;
using event_service.Kafka;
using event_service.Model;
using Google.Api.Gax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Text;

namespace event_service.Service
{
    public class EventService : IEventService
    {
        private readonly EventDbContext _context;
        private readonly IParticipantsService _participantsService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public EventService(EventDbContext context, IParticipantsService participantsService, IKafkaProducerService kafkaProducerService)
        {
            _context = context;
            _participantsService = participantsService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            Events newEvent = eventDto.ToEntity();

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return EventMapper.ToDto(newEvent);

        }

        public async Task<List<EventDto>> GetEventAsync(string id)
        {
            var eventItems = await _context.Events
                                    .Where(u => u.IdCreate == id)
                                    .ToListAsync();
            if (eventItems == null || !eventItems.Any())
            {
                return new List<EventDto>();
            }
            foreach (var eventItem in eventItems)
            {
                if (eventItem.Banner == null)
                {
                    eventItem.Banner = "";
                }
            }
            return eventItems.Select(EventMapper.ToDto).ToList();
        }
        public async Task<List<EventDto>> GetEventStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty.", nameof(status));
            }

            try
            {
                var events = await _context.Events
                                           .Where(e => e.Status == status && e.access == true)
                                           .ToListAsync();

                return events?.Any() == true ? EventMapper.ToDtoList(events) : new List<EventDto>();
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is set up
                throw new Exception("An error occurred while retrieving event statuses.", ex);
            }
        }


        // Lọc sự kiện theo loại hình
        public async Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _context.Events
                .Where(e => e.type.ToLower() == category.ToLower())
                .ToListAsync();

            return events.Select(EventMapper.ToDto);
        }



        // Chỉnh sửa sự kiện
        public async Task<bool> UpdateEventAsync(int id, Events eventDto)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return false;
            }

            eventItem.Name = eventDto.Name ?? "error name";
            eventItem.Description = eventDto.Description ?? "error desc";
            eventItem.StartDate = eventDto.StartDate;
            eventItem.EndDate = eventDto.EndDate;
            eventItem.Location = eventDto.Location ?? "error loca";
            eventItem.TargetAudience = eventDto.TargetAudience ?? "error obj";
            eventItem.type = eventDto.type ?? "Seminar";
            eventItem.Banner = eventDto.Banner ?? "";

            _context.Entry(eventItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa sự kiện
        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return false;
            }
            _context.Events.Remove(eventItem);
            var schedules = await _context.Schedules.Where(e => e.EventId == eventItem.id).ToListAsync();
            if (schedules != null)
            {
                foreach (var i in schedules)
                {
                    var schedules_par = await _context.Schedule_Participants.Where(e => e.scheduleId == i.Id).ToListAsync();
                    _context.Schedule_Participants.RemoveRange(schedules_par);
                }

                _context.Schedules.RemoveRange(schedules);
            }
            var eventAttendances = await _context.EventAttendances.Where(e => e.eventId == eventItem.id).ToListAsync();
            if (eventAttendances != null && eventAttendances.Any())
            {
                _context.EventAttendances.RemoveRange(eventAttendances);
            }
            var paritipant = await _context.Participants.Where(e => e.eventId == eventItem.id).ToListAsync();
            if (paritipant != null && paritipant.Any())
            {
                _context.Participants.RemoveRange(paritipant);
            }
            var review = await _context.Reviews.Where(e => e.Eventid == eventItem.id).ToListAsync();
            if (review != null && review.Any())
            {
                _context.Reviews.RemoveRange(review);
            }

            await _context.SaveChangesAsync();
            await _kafkaProducerService.SendMessageAsync(eventItem.id);
            return true;
        }

        public async Task<string> GetIdEvent(string idCreate, string name)
        {
            var currentEvent = await _context.Events
                .FirstOrDefaultAsync(p => p.IdCreate == idCreate && p.Name == name);

            if (currentEvent == null)
            {
                return null;
            }

            return currentEvent.id.ToString();
        }

        public async Task<Events> GetEventByIdAsync(int id)
        {
            try
            {
                var eventEntity = await _context.Events
                    .FirstOrDefaultAsync(e => e.id == id);

                if (eventEntity == null)
                {
                    return null;
                }

                return new Events
                {
                    id = eventEntity.id,
                    Name = eventEntity.Name,
                    Description = eventEntity.Description,
                    StartDate = eventEntity.StartDate,
                    EndDate = eventEntity.EndDate,
                    Location = eventEntity.Location,
                    TargetAudience = eventEntity.TargetAudience,
                    Status = eventEntity.Status,
                    type = eventEntity.type,
                    Banner = eventEntity.Banner,
                    eventCode = eventEntity.eventCode,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<EventDto>> GetEventHomePage(string uid)
        {
            DateTime currentDateTime = DateTime.Now;

            HashSet<int> addedEventIds = new HashSet<int>();
            List<EventDto> listEvent = new List<EventDto>();

            //if (role == "Organizer")
            //{
            var list = await GetEventAsync(uid);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.status == "Ongoing" && addedEventIds.Add(item.id))
                    {
                        //Console.WriteLine($"StartDate: {eventItem.StartDate}, currentDateTime: {currentDateTime}");

                        listEvent.Add(item);
                    }

                    if (currentDateTime <= item.StartDate && addedEventIds.Add(item.id) && item.status != "Cancelled")
                    {
                        //Console.WriteLine($"StartDate: {item.StartDate}, currentDateTime: {currentDateTime}");
                        listEvent.Add(item);
                    }
                }
            }
            //}

            var userRegisterEvent = await _context.Participants.Where(u => u.userId == uid && u.status != "Pending").ToListAsync();
            if (userRegisterEvent == null || !userRegisterEvent.Any())
            {
                return listEvent.OrderBy(e => Math.Abs((e.StartDate - DateTime.Now).TotalMilliseconds))
           .ToList();
            }

            foreach (var participant in userRegisterEvent)
            {
                var eventItem = await _context.Events.FindAsync(participant.eventId);
                if (eventItem.Status == "Ongoing" && addedEventIds.Add(eventItem.id))
                {
                    //Console.WriteLine($"StartDate: {eventItem.StartDate}, currentDateTime: {currentDateTime}");

                    listEvent.Add(EventMapper.ToDto(eventItem));
                }
                if (eventItem != null)
                {
                    if (currentDateTime <= eventItem.StartDate && addedEventIds.Add(eventItem.id))
                    {
                        //Console.WriteLine($"StartDate: {eventItem.StartDate}, currentDateTime: {currentDateTime}");

                        listEvent.Add(EventMapper.ToDto(eventItem));
                    }
                }
            }

            return listEvent.OrderBy(e => Math.Abs((e.StartDate - DateTime.Now).TotalMilliseconds))
            .ToList();
        }

        public async Task<ScheduleDto> CreateScheduleAsync(int eventId, ScheduleDto scheduleDto)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                throw new Exception("Event not found");
            }

            var schedule = new Event_Schedules
            {
                EventId = eventId,
                Time = scheduleDto.Time,
                Title = scheduleDto.Title,
                Location = scheduleDto.Location
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new ScheduleDto
            {
                Id = schedule.Id,
                Time = schedule.Time,
                Title = schedule.Title,
                Location = schedule.Location
            };
        }

        // Get schedules for a specific event
        public async Task<List<ScheduleDto>> GetSchedulesForEventAsync(int eventId)
        {
            var schedules = await _context.Schedules
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.Time)
                .ToListAsync();

            return schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                Time = s.Time,
                Title = s.Title,
                Location = s.Location
            }).ToList();
        }

        // Update a schedule
        public async Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleDto scheduleDto)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            schedule.Time = scheduleDto.Time;
            schedule.Title = scheduleDto.Title;
            schedule.Location = scheduleDto.Location;

            _context.Entry(schedule).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        // Delete a schedule
        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ParticipantsDto>> GetParticipantsByEventIdandRole(int eventId, string role)
        {
            try
            {
                var participants = await _context.Participants
                    .Where(p => p.eventId == eventId && p.role == role)
                    .ToListAsync();

                if (participants == null || !participants.Any())
                {
                    return new List<ParticipantsDto>();
                }

                return participants.Select(ParticipantsMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching participants.", ex);
            }
        }

        public async Task<List<ParticipantsDto>> GetParticipantsAsync(int eventId, String status, String role)
        {
            try
            {
                var participants = await _context.Participants
                    .Where(p => p.eventId == eventId && p.status == status && p.role == role)
                    .ToListAsync();

                if (participants == null || !participants.Any())
                {
                    return new List<ParticipantsDto>();
                }

                return participants.Select(ParticipantsMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching participants.", ex);
            }
        }

        public async Task<bool> DeleteParticipantAsync(int eventId, int participantId)
        {
            try
            {
                var participant = await _context.Participants
                    .Where(p => p.eventId == eventId && p.id == participantId)
                    .FirstOrDefaultAsync();

                if (participant == null)
                {
                    return false;
                }

                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the participant.", ex);
            }
        }

        public async Task<bool> RemoveParticipantAsync(int eventId, int participantId)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.eventId == eventId && p.id == participantId);

            if (participant == null)
            {
                return false;
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ApproveParticipantAsync(int eventId, int participantId)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.eventId == eventId && p.id == participantId && p.status == "Pending");

            if (participant == null)
            {
                return false;
            }

            participant.status = "Approved";
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddParticipantToScheduleAsync(int scheduleId, string userId)
        {
            var scheduleParticipant = new Schedule_Participants
            {
                scheduleId = scheduleId,
                userId = userId,
                status = "Approved",
                role = "Participant"
            };

            try
            {
                _context.Schedule_Participants.Add(scheduleParticipant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEventAccessAsync(int eventId, bool access)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }

            eventEntity.access = access;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEventAllowAsync(int eventId, bool allow)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }

            eventEntity.allowSelectSchedule = allow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EventDto> GetTicketDataByIdAsync(int id)
        {
            var eventEntity = await _context.Events
                .Where(e => e.id == id)
                .FirstOrDefaultAsync();

            if (eventEntity == null)
            {
                return null;
            }

            var eventDto = new EventDto
            {
                id = eventEntity.id,
                Name = eventEntity.Name,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                status = eventEntity.Status,
                Banner = eventEntity.Banner,
                access = eventEntity.access,
                allowSelectSchedule = eventEntity.allowSelectSchedule,
                Description = eventEntity.Description,
                Location = eventEntity.Location
            };

            return eventDto;
        }

        public async Task<object> GetEventStats(int EventId)
        {
            var registered = await _context.Participants.CountAsync(e => e.eventId == EventId &&
            e.role == "Participant" && e.status == "Approved" || e.status == "Added");

            var speaker = await _context.Special_Participants.CountAsync(e => e.eventId == EventId);

            var Sessions = await _context.Schedules.CountAsync(e => e.EventId == EventId);

            return new
            {
                registered = registered,
                speaker = speaker,
                sessions = Sessions
            };
        }

        public async Task ChangeStatusEventAsync(int eventId, string status)
        {
            var currentEvent = await _context.Events.FirstOrDefaultAsync(e => e.id == eventId);
            if (currentEvent == null)
            {
                throw new Exception("Event not found.");
            }

            currentEvent.Status = status;
            await _context.SaveChangesAsync();
        }


    }
}