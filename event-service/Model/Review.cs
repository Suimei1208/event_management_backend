using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace event_service.Model
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int Eventid { get; set; }
        public string uid { get; set; }
        public int rate { get; set; }
        [AllowNull]
        public string? review { get; set; }
    }
}
