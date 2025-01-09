using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace forum_service.Model
{
    public class CommentReplies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string uid { get; set; }
        public int comment_id { get; set; }
        public DateTime timepost { get; set; }
        public int likes { get; set; }
        public string comment { get; set; }
    }
}
