using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ticket_service.Model
{
    public class ticket_cancellation_period
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int event_id { get; set; }
        [AllowNull]
        public DateTime start_date { get; set; }
        [AllowNull]
        public DateTime end_date { get; set; }
        public bool is_reason_imgage_required { get; set; }
        public bool is_link_required { get; set; }
        [AllowNull]
        public string? link { get; set; }
    }
}
