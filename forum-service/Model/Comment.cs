using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace forum_service.Model
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string uid { get; set; }
        public string comment { get; set; }
        public DateTime timepost { get; set; }
        public int likes { get; set; }
        public int post_id { get; set; }    
    }
}
