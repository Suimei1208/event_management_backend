namespace event_service.Model
{
    public class Schedule_Participants
    {
        public int id { get; set; }
        public int scheduleId { get; set; }
        public string userId { get; set; }
        public string status { get; set; }
        public string role { get; set; }

    }
}
