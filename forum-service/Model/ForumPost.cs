using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace forum_service.Model
{
    public class ForumPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string uid { get; set; }
        public DateTime timepost { get; set; }
        public int likes { get; set; }
        public int comments_count { get; set; }
        [AllowNull]
        public string? image { get; set; }
    }
}
