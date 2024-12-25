using event_service.DTO;

namespace event_service.Interface
{
    public interface IParticipantsService
    {
        Task AddParticipants(List<ParticipantsDto> participantsDtos);
        Task<List<object>> getEventRegisterPending(string uid);
        Task GetParticipants(int eventId);
        Task UnregisterEvent(string eventid, string uid);
    }
}
