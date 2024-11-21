using event_service.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace event_service
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<users> Users { get; set; }
    }
}
