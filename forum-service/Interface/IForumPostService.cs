using forum_service.DTO;
using forum_service.Model;

namespace forum_service.Interface
{
    public interface IForumPostService
    {
       Task CreatePost(ForumPostDTO forumPostDTO);
        Task<List<dynamic>> getPostAsync(string uid);
        Task UpdateLike(int id, bool isLike);
        Task<dynamic> detailPost(int idPost, string uid);
        Task CreateComment(CommentDTO commentDTO);
        Task CreateReplyComment(CommentReplies commentReplies);
        Task DeleteComment(int commentId);
        Task DeleteReplyComment(int commentId);
        Task DeletePost(int idPost);
        Task EditPost(int postId, string title, string description, string category, string? image);
    }
}
