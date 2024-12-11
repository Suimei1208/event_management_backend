using event_service.DTO;

namespace event_service.Interface
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(EventDto eventDto);
        Task<List<EventDto>> GetEventAsync(string id);
        Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category);
        Task<bool> UpdateEventAsync(int id, EventDto eventDto);
        Task<bool> DeleteEventAsync(int id);
    }
}
