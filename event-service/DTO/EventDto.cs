using event_service.Model;

namespace event_service.DTO
{
    public class EventDto
    {
        public string Name { get; set; }
        public string IdCreate { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TargetAudience { get; set; }
        public string Banner {  get; set; }
        public string Category { get; set; }
    }
    public static class EventMapper
    {
        public static EventDto ToDto(this Events eventEntity)
        {
            return new EventDto
            {
                Name = eventEntity.Name,
                IdCreate = eventEntity.IdCreate,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Location = eventEntity.Location,
                TargetAudience = eventEntity.TargetAudience,
                Category = eventEntity.Category
            };
        }

        public static Events ToEntity(this EventDto eventDto)
        {
            return new Events
            {
                Name = eventDto.Name,
                IdCreate = eventDto.IdCreate,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                Location = eventDto.Location,
                TargetAudience = eventDto.TargetAudience,
                Category = eventDto.Category
            };
        }
    }

}
