using event_service.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace event_service
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }
        
        public DbSet<Events> Events { get; set; }
        public DbSet<Participants> Participants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participants>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(p => p.eventId)
                .HasConstraintName("FK_Participants_Events");

            base.OnModelCreating(modelBuilder);
        }
    }
}
