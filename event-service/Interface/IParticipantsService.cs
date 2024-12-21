using event_service.DTO;

namespace event_service.Interface
{
    public interface IParticipantsService
    {
        Task AddParticipants(List<ParticipantsDto> participantsDtos);
        Task GetParticipants(int eventId);
    }
}
