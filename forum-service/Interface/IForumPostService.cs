using forum_service.DTO;

namespace forum_service.Interface
{
    public interface IForumPostService
    {
       Task CreatePost(ForumPostDTO forumPostDTO);
        Task<List<dynamic>> getPostAsync(string uid);
        Task UpdateLike(int id, bool isLike);
        Task<dynamic> detailPost(int idPost, string uid);
    }
}
