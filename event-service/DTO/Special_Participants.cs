namespace event_service.DTO
{
    public class Special_ParticipantsDto
    {
        public int id { get; set; }
        public int eventId { get; set; }
        public DateTime registration_Date { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string description { get; set; }
        public string photoUrl { get; set; }
    }
}
