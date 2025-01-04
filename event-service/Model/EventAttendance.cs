namespace event_service.Model
{
    public class EventAttendance
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int eventId{ get; set; }
        public bool checkIn { get; set; }
        public DateTime checkInTime { get; set; }
        public bool checkOut { get; set; }
        public DateTime checkOutTime { get; set; }
    }
}
