namespace event_service.DTO
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public bool allow { get; set; }
    }

}
