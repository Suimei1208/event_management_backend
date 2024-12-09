using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace event_service.Model
{
    public class Participants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string userId { get; set; }
        public string eventId { get; set; }
        public DateTime registration_Date { get; set; }
        public string status { get; set; }
    }
}
