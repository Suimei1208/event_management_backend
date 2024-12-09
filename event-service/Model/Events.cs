using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace event_service.Model
{
    public class Events
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string Name { get; set; }

        public string IdCreate { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Location { get; set; }

        public string TargetAudience { get; set; }
        [AllowNull]
        public string Banner { get; set; }
        public string Status { get; set; }
        [Required]
        public string type { get; set; }
    }
}
