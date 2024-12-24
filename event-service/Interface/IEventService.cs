using event_service.DTO;

namespace event_service.Interface
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(EventDto eventDto);
        Task<List<EventDto>> GetEventAsync(string id);
        Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category);

        Task<bool> UpdateEventAsync(int id, EventWithParticipantsDto eventDto);
        Task<bool> DeleteEventAsync(int id);
        Task<string> GetIdEvent(string idCreate, string name);
        Task<EventWithParticipantsDto> GetEventByIdAsync(int id);
        Task<List<EventDto>> GetEventHomePage(string uid, string role);
        Task<ScheduleDto> CreateScheduleAsync(int eventId, ScheduleDto scheduleDto);
        Task<List<ScheduleDto>> GetSchedulesForEventAsync(int eventId);
        Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleDto scheduleDto);
        Task<bool> DeleteScheduleAsync(int scheduleId);
    }
}
