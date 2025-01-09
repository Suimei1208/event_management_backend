using forum_service.Model;

namespace forum_service.DTO
{
    public class CommentRepliesDTO
    {
        public int Id { get; set; } 
        public string Uid { get; set; } 
        public int CommentId { get; set; } 
        public DateTime TimePost { get; set; } 
        public int Likes { get; set; } 
        public string Comment { get; set; } 
    }

    public static class CommentRepliesExtensions
    {
        public static CommentRepliesDTO ToDTO(this CommentReplies reply)
        {
            if (reply == null) return null;
            return new CommentRepliesDTO
            {
                Id = reply.id,
                Uid = reply.uid,
                CommentId = reply.comment_id,
                TimePost = reply.timepost,
                Likes = reply.likes,
                Comment = reply.comment
            };
        }

        public static CommentReplies ToEntity(this CommentRepliesDTO replyDTO)
        {
            if (replyDTO == null) return null;
            return new CommentReplies
            {
                id = replyDTO.Id,
                uid = replyDTO.Uid,
                comment_id = replyDTO.CommentId,
                timepost = replyDTO.TimePost,
                likes = replyDTO.Likes,
                comment = replyDTO.Comment
            };
        }
    }
}
