using forum_service.Model;

namespace forum_service.DTO
{
    public class ForumPostDTO
    {
        public int Id { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public string Category { get; set; }
        public string Uid { get; set; } 
        public DateTime TimePost { get; set; } 
        public int Likes { get; set; } 
        public int CommentsCount { get; set; } 
        public string? Image { get; set; } 
    }

    public static class ForumPostExtensions
    {
        public static ForumPostDTO ToDTO(this ForumPost post)
        {
            if (post == null) return null;
            return new ForumPostDTO
            {
                Id = post.id,
                Title = post.title,
                Description = post.description,
                Category = post.category,
                Uid = post.uid,
                TimePost = post.timepost,
                Likes = post.likes,
                CommentsCount = post.comments_count,
                Image = post.image
            };
        }      

        public static ForumPost ToEntity(this ForumPostDTO postDTO)
        {
            if (postDTO == null) return null;
            return new ForumPost
            {
                id = postDTO.Id,
                title = postDTO.Title,
                description = postDTO.Description,
                category = postDTO.Category,
                uid = postDTO.Uid,
                timepost = postDTO.TimePost,
                likes = postDTO.Likes,
                comments_count = postDTO.CommentsCount,
                image = postDTO.Image
            };
        }
    }
}
