using event_service.DTO;
using event_service.Interface;
using event_service.Kafka;
using event_service.Model;
using Google.Api.Gax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace event_service.Service
{
    public class EventService : IEventService
    {
        private readonly EventDbContext _context;
        private readonly IParticipantsService _participantsService;
        private readonly KafkaConsumerService _kafkaConsumerService;

        public EventService(EventDbContext context, IParticipantsService participantsService, KafkaConsumerService kafkaConsumerService)
        {
            _context = context;
            _participantsService = participantsService;
            _kafkaConsumerService = kafkaConsumerService;
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
                                           .Where(e => e.Status == status)
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
        public async Task<bool> UpdateEventAsync(int id, EventWithParticipantsDto eventDto)
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
            eventItem.type = eventDto.Type ?? "Seminar";
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
            await _context.SaveChangesAsync();

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

        public async Task<EventWithParticipantsDto> GetEventByIdAsync(int id)
        {
            try
            {
                var eventEntity = await _context.Events
                    .Include(e => e.Participants)
                    .FirstOrDefaultAsync(e => e.id == id);

                if (eventEntity == null)
                {
                    return null;
                }

                return new EventWithParticipantsDto
                {
                    Id = eventEntity.id,
                    Name = eventEntity.Name,
                    Description = eventEntity.Description,
                    StartDate = eventEntity.StartDate,
                    EndDate = eventEntity.EndDate,
                    Location = eventEntity.Location,
                    TargetAudience = eventEntity.TargetAudience,
                    Status = eventEntity.Status,
                    Type = eventEntity.type,
                    Banner = eventEntity.Banner,
                    Participants = eventEntity.Participants?
                        .Select(ParticipantsMapper.ToDto)
                        .ToList() ?? new List<ParticipantsDto>()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<EventDto>> GetEventHomePage(string uid, string role)
        {
            DateTime currentDateTime = DateTime.Now;

            HashSet<int> addedEventIds = new HashSet<int>();
            List<EventDto> listEvent = new List<EventDto>();

            if (role == "Organizer")
            {
                var list = await GetEventAsync(uid);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (currentDateTime <= item.StartDate &&  addedEventIds.Add(item.id))
                        {
                            Console.WriteLine($"StartDate: {item.StartDate}, currentDateTime: {currentDateTime}");
                            listEvent.Add(item);
                        }
                    }
                }
            }

            var userRegisterEvent = await _context.Participants.Where(u => u.userId == uid).ToListAsync();
            if (userRegisterEvent == null || !userRegisterEvent.Any())
            {
                return null;
            }

            foreach (var participant in userRegisterEvent)
            {
                var eventItem = await _context.Events.FindAsync(participant.eventId);
                if (eventItem != null)
                {
                    if (currentDateTime <= eventItem.StartDate && addedEventIds.Add(eventItem.id))
                    {
                        Console.WriteLine($"StartDate: {eventItem.StartDate}, currentDateTime: {currentDateTime}");

                        listEvent.Add(EventMapper.ToDto(eventItem));
                    }
                }
            }

            return listEvent.OrderBy(e => Math.Abs((e.StartDate - DateTime.Now).TotalMilliseconds))
            .ToList(); ;
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

        public async Task<List<ParticipantsDto>> GetParticipantsByEventIdAndRoleAsync(int eventId, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Role cannot be null or empty.", nameof(role));
            }

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

        public async Task<List<ParticipantsDto>> GetPendingParticipantsAsync(int eventId)
        {
            try
            {
                var participants = await _context.Participants
                    .Where(p => p.eventId == eventId && p.status == "Pending")
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

        public async Task<bool> DeleteParticipantAsync(int eventId, int participantId, string role)
        {
            try
            {
                var participant = await _context.Participants
                    .Where(p => p.eventId == eventId && p.id == participantId && p.role == role)
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
    }
}
