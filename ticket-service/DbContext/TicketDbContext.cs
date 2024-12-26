using Microsoft.EntityFrameworkCore;
using ticket_service.Model;

namespace ticket_service
{
    public class TicketDbContext: DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }
    }
}
