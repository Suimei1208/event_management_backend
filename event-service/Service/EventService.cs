using event_service.DTO;
using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class EventService : IEventService
    {
        private readonly EventDbContext _context;
        private readonly IKafkaConsumer _consumer;

        public EventService(EventDbContext context, IKafkaConsumer consumer)
        {
            _context = context;
            _consumer = consumer;
        }

        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            var userRoleEvent = await _consumer.ListenForUserRoleChanges(CancellationToken.None);

            if (userRoleEvent != null && userRoleEvent.UserId == eventDto.IdCreate 
                && userRoleEvent.Role.Equals("Organizer", StringComparison.OrdinalIgnoreCase))
            {
                Events newEvent = eventDto.ToEntity();

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                return EventMapper.ToDto(newEvent);
            }
            else
            {
                Console.WriteLine($"Người dùng với ID {eventDto.IdCreate} không phải là Ban tổ chức.");
                return null;
            }
        }

        // Lấy thông tin sự kiện theo ID
        public async Task<EventDto> GetEventAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return null;
            }
            return EventMapper.ToDto(eventItem);
        }

        // Lọc sự kiện theo loại hình
        public async Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _context.Events
                .Where(e => e.Category.ToLower() == category.ToLower())
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
            eventItem.Category = eventDto.Category;

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
    }

}
