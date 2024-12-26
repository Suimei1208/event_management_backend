using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ticket_service.Model
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required] 
        public string QRCode { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
