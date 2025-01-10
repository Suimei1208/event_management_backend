namespace event_service.DTO
{
    public class EventAttendanceDto
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int eventId { get; set; }
        public bool checkIn { get; set; }
        public DateTime checkInTime { get; set; }
        public bool checkOut { get; set; }
        public DateTime checkOutTime { get; set; }
    }

    public class CheckInRequest
    {
        public string QRCode { get; set; }
    }

    public class EventStatisticsDto
    {
        public int checkedInParticipants { get; set; }
        public double AverageParticipationTime { get; set; }
        public double ParticipationPercentage { get; set; }
        public string AverageParticipationTimeFormatted { get; internal set; }
    }
}
