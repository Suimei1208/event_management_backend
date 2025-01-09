using forum_service.Model;

namespace forum_service.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Comment { get; set; }
        public DateTime TimePost { get; set; }
        public int Likes { get; set; }
        public int PostId { get; set; }
    }


    public static class CommentExtensions
    {
        public static CommentDTO ToDTO(this Comment comment)
        {
            if (comment == null) return null;
            return new CommentDTO
            {
                Id = comment.id,
                Uid = comment.uid,
                Comment = comment.comment,
                TimePost = comment.timepost,
                Likes = comment.likes,
                PostId = comment.post_id
            };
        }

        public static Comment ToEntity(this CommentDTO commentDTO)
        {
            if (commentDTO == null) return null;
            return new Comment
            {
                id = commentDTO.Id,
                uid = commentDTO.Uid,
                comment = commentDTO.Comment,
                timepost = commentDTO.TimePost,
                likes = commentDTO.Likes,
                post_id = commentDTO.PostId
            };
        }
    }
}
