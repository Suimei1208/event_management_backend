using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace event_service.Model
{
    public class Guests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public string contact {  get; set; }
        public string role { get; set; }
        public int eventId { get; set; }
    }
}
