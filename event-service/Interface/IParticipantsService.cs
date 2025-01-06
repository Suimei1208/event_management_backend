using event_service.DTO;
using event_service.Model;

namespace event_service.Interface
{
    public interface IParticipantsService
    {
        Task AddParticipants(List<ParticipantsDto> participantsDtos);
        Task<List<object>> getEventRegisterPending(string uid);
        Task GetParticipants(int eventId);
        Task UnregisterEvent(string eventid, string uid);
        Task<List<Participants>> AddParticipantsFromExcelAsync(int eventId, List<string> userIds);
        Task<ParticipantsDto> GetParticipantRoleByUserIdAsync(string userId, int eventId);
    }
}
