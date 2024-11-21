using event_service.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace event_service
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }

        public DbSet<Events> Events { get; set; }
    }
}
