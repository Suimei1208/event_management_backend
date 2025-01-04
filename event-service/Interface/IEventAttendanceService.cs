namespace event_service.Interface
{
    public interface IEventAttendanceService
    {
        Task RecordCheckInAsync(string qrCode);
        Task RecordCheckOutAsync(string qrCode);
    }
}
