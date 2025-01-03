using Microsoft.EntityFrameworkCore;
using ticket_service.Interface;
using ticket_service.Model;

namespace ticket_service.Service
{
    public class CancellationPeriodsService : ICancellationPeriodsService
    {
        private readonly TicketDbContext _context;

        public CancellationPeriodsService(TicketDbContext context)
        {
            _context = context;
        }

        public async Task CreateCancellationPeriods(ticket_cancellation_period period)
        {
            await _context.CancellationPeriods.AddAsync(period);
            await _context.SaveChangesAsync();
        }
        public async Task<ticket_cancellation_period> GetPeriod(int EventId)
        {
            var result = await _context.CancellationPeriods.FirstOrDefaultAsync(e => e.event_id == EventId);
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task update(ticket_cancellation_period period)
        {
            var current = await _context.CancellationPeriods.FirstOrDefaultAsync(e => e.id == period.id);
            if (current != null)
            {
                current.event_id = period.event_id;
                current.start_date = period.start_date;
                current.end_date = period.end_date;
                current.is_link_required = period.is_link_required;
                current.is_reason_imgage_required = period.is_reason_imgage_required;
                current.link = period.link;

                await _context.SaveChangesAsync();
            }
        }


    }
}
