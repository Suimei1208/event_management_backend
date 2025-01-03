using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ticket_service.DTO;

namespace ticket_service.Model
{
    public class detail_ticket_cancellation_period
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int event_id { get; set; }
        public string uid { get; set; }
        public DateTime send_at { get; set; }
        public string reason { get; set; }
        public string link_image { get; set; }
    }
}
