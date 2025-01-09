using forum_service.Model;
using Microsoft.EntityFrameworkCore;
namespace forum_service.DbContext
{
    public class ForumDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
        {
        }

        public DbSet<ForumPost> Forums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReplies> CommentReplies { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }

    }
    
}
