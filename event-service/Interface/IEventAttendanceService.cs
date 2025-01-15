using event_service.DTO;

namespace event_service.Interface
{
    public interface IEventAttendanceService
    {
        Task<List<EventAttendanceDto>> GetCheckedInAndCheckedOutParticipantsAsync(int eventId);
        Task<List<EventAttendanceDto>> GetCheckedInParticipantsAsync(int eventId);
        Task<List<EventAttendanceDto>> GetCheckedOutParticipantsAsync(int eventId);
        Task RecordCheckInAsync(string qrCode);
        Task RecordCheckOutAsync(string qrCode);
        Task RecordCheckInManuallyAsync(int eventId, string inputName);
        Task RecordCheckOutManuallyAsync(int eventId, string inputName);
        Task<EventStatisticsDto> GetEventStatisticsAsync(int eventId);
    }
}
