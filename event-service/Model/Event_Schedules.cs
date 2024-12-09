using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace event_service.Model
{
    public class Event_Schedules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int eventId { get; set; }

        public string session_Name { get; set; }

        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string description { get; set; }
    }
}
