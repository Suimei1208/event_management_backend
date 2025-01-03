using Microsoft.EntityFrameworkCore;
using ticket_service.Model;

namespace ticket_service
{
    public class TicketDbContext: DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ticket_cancellation_period> CancellationPeriods { get; set; }
        public DbSet<detail_ticket_cancellation_period> detail_Ticket_Cancellation_Periods { get; set; }
    }
}
