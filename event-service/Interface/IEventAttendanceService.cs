using event_service.DTO;

namespace event_service.Interface
{
    public interface IEventAttendanceService
    {
        Task<List<EventAttendanceDto>> GetCheckedInAndCheckedOutParticipantsAsync(int eventId);
        Task RecordCheckInAsync(string qrCode);
        Task RecordCheckOutAsync(string qrCode);
    }
}
