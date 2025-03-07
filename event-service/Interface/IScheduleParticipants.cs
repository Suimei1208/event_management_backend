using event_service.Model;

namespace event_service.Interface
{
    public interface IScheduleParticipants
    {
        Task<List<Schedule_Participants>> GetScheduleParticipantsAsync(int eventId);
        Task<bool> RemoveScheduleParticipantAsync(int eventId, string participantId);
    }
}
