using event_service.DTO;
using event_service.Interface;
using event_service.Kafka;
using event_service.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
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

        // Lọc sự kiện theo loại hình
        public async Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _context.Events
                .Where(e => e.type.ToLower() == category.ToLower())
                .ToListAsync();

            return events.Select(EventMapper.ToDto);
        }

        // Chỉnh sửa sự kiện
        public async Task<bool> UpdateEventAsync(int id, EventDto eventDto)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return false;
            }

            eventItem.Name = eventDto.Name;
            eventItem.Description = eventDto.Description;
            eventItem.StartDate = eventDto.StartDate;
            eventItem.EndDate = eventDto.EndDate;
            eventItem.Location = eventDto.Location;
            eventItem.TargetAudience = eventDto.TargetAudience;
            eventItem.type = eventDto.type;
            eventItem.Banner = eventDto.Banner;

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

        public async Task<EventWithParticipantsDto> GetEventByIdAsync(int id, CancellationToken cancellationToken)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return null;
            }
            // gọi kafka để láy userid
            await _participantsService.GetParticipants(eventEntity.id);

            List<CustomParticipants> results = await _kafkaConsumerService.ConsumeMessagesAsync(cancellationToken, eventEntity.id.ToString());
            // ra rồi đó m xử lý ngay đây này
            foreach(var user in results)
            {
                Console.WriteLine(user.ToString());
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
                Participants = eventEntity.Participants?.Select(ParticipantsMapper.ToDto).ToList() ?? new List<ParticipantsDto>()
            };
        }
    }

}
