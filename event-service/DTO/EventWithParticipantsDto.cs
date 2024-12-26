using System.Collections.Generic;

namespace event_service.DTO
{
    public class EventWithParticipantsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TargetAudience { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Banner { get; set; }
        public int eventCode { get; set; }

        public List<ParticipantsDto> Participants { get; set; }
    }
}
