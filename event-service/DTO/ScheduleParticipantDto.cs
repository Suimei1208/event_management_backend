namespace event_service.DTO
{
    public class ScheduleParticipantDto
    {
        public int id { get; set; }
        public int scheduleId { get; set; }
        public int userId { get; set; }
        public string status { get; set; }
        public string role { get; set; }
    }
}
