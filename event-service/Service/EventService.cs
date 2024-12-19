using event_service.DTO;
using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class EventService : IEventService
    {
        private readonly EventDbContext _context;

        public EventService(EventDbContext context)
        {
            _context = context;
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
    }

}
