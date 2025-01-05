using event_service.Model;
using Google.Api.Gax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace event_service
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }
        
        public DbSet<Events> Events { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Event_Schedules> Schedules { get; set; }
        public DbSet<Schedule_Participants> Schedule_Participants { get; set; }
        public DbSet<Special_Participants> Special_Participants { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Spending> Spendings { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Participants>()
        //        .HasOne(p => p.Event)
        //        .WithMany(e => e.Participants)
        //        .HasForeignKey(p => p.eventId)
        //        .HasConstraintName("FK_Participants_Events");

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
