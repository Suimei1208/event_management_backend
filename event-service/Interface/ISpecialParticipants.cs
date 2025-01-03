using event_service.DTO;
using event_service.Model;

namespace event_service.Interface
{
    public interface ISpecialParticipants
    {
        Task<bool> AddSpecialParticipantAsync(int eventId, string name, string role, string description, string photoUrl);
        Task<List<Special_Participants>> GetSpecialParticipantsAsync(int eventId);
        Task<bool> RemoveSpecialParticipantAsync(int eventId, int participantId);
    }
}
