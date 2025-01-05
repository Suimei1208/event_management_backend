using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class SpendingService : ISpendingService
    {
        private readonly EventDbContext _context;

        public SpendingService(EventDbContext context)
        {
            _context = context;
        }

        public async Task<List<Spending>> GetSpendingsAsyncByEventId(int eventId)
        {
            var spendings = await _context.Spendings
                .Where(s => s.eventId == eventId)
                .ToListAsync();
            return spendings;
        }

        public async Task<Spending> GetSpendingByIdAsync(int id)
        {
            var spending = await _context.Spendings
                .FirstOrDefaultAsync(s => s.id == id);
            return spending;
        }

        public async Task<Spending> AddSpendingAsync(int eventId, string category, double amount, string type)
        {
            var normalizedCategory = category.Trim().ToLower();

            var existingSpending = await _context.Spendings
                .Where(s => s.eventId == eventId && s.category.ToLower() == normalizedCategory && s.type == type)
                .FirstOrDefaultAsync();

            if (existingSpending != null)
            {
                existingSpending.amount += amount;
                await _context.SaveChangesAsync();
                return existingSpending;
            }
            else
            {
                var newSpending = new Spending
                {
                    eventId = eventId,
                    category = category,
                    amount = amount,
                    type = type
                };

                _context.Spendings.Add(newSpending);
                await _context.SaveChangesAsync();
                return newSpending;
            }
        }


        public async Task<Spending> UpdateSpendingAsync(int id, double amount, string category)
        {
            var spending = await _context.Spendings
                .FirstOrDefaultAsync(s => s.id == id);

            if (spending == null) return null;

            spending.amount = amount;
            spending.category = category;
            await _context.SaveChangesAsync();
            return spending;
        }

        public async Task<bool> RemoveSpendingAsync(int eventId, int id)
        {
            var spending = await _context.Spendings
                .FirstOrDefaultAsync(s => s.eventId == eventId && s.id == id);

            if (spending == null) return false;

            _context.Spendings.Remove(spending);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
