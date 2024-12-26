using event_service.DTO;
using Microsoft.AspNetCore.Mvc;

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
        Task<List<EventDto>> GetEventStatus(string status);
        Task<List<ParticipantsDto>> GetParticipantsByEventIdAndRoleAsync(int eventId, string role);
        Task<bool> DeleteParticipantAsync(int eventId, int participantId, string role);
        Task<List<ParticipantsDto>> GetPendingParticipantsAsync(int eventId);
        Task<bool> ApproveParticipantAsync(int eventId, int participantId);
    }
}
