using event_service.DTO;
using event_service.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace event_service.Interface
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(EventDto eventDto);
        Task<List<EventDto>> GetEventAsync(string id);
        Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category);

        Task<bool> UpdateEventAsync(int id, Events eventDto);
        Task<bool> DeleteEventAsync(int id);
        Task<string> GetIdEvent(string idCreate, string name);
        Task<Events> GetEventByIdAsync(int id);
        Task<List<EventDto>> GetEventHomePage(string uid);
        Task<ScheduleDto> CreateScheduleAsync(int eventId, ScheduleDto scheduleDto);
        Task<List<ScheduleDto>> GetSchedulesForEventAsync(int eventId);
        Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleDto scheduleDto);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<EventDto>> GetEventStatus(string status);
        Task<List<ParticipantsDto>> GetParticipantsByEventIdandRole(int eventId, string role);
        Task<bool> DeleteParticipantAsync(int eventId, int participantId);
        Task<List<ParticipantsDto>> GetStatusParticipantsAsync(int eventId, String status);
        Task<bool> ApproveParticipantAsync(int eventId, int participantId);
        Task<bool> AddParticipantToScheduleAsync(int scheduleId, string userId);
        Task<bool> UpdateEventAccessAsync(int eventId, bool access);
        Task<bool> UpdateEventAllowAsync(int eventId, bool allow);
        Task<EventDto> GetTicketDataByIdAsync(int id);
    }
}
