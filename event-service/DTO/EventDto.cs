using event_service.Model;

namespace event_service.DTO
{
    public class EventDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string IdCreate { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TargetAudience { get; set; }
        public string Banner {  get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }
    public static class EventMapper
    {
        public static EventDto ToDto(this Events eventEntity)
        {
            return new EventDto
            {
                id = eventEntity.id,
                Name = eventEntity.Name,
                IdCreate = eventEntity.IdCreate,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Location = eventEntity.Location,
                TargetAudience = eventEntity.TargetAudience,
                type = eventEntity.type,
                status = eventEntity.Status
            };
        }

        public static Events ToEntity(this EventDto eventDto)
        {
            return new Events
            {
                id = eventDto.id,
                Name = eventDto.Name,
                IdCreate = eventDto.IdCreate,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                Location = eventDto.Location,
                TargetAudience = eventDto.TargetAudience,
                type = eventDto.type,
                Status = eventDto.status
            };
        }
    }

}
