using Microsoft.EntityFrameworkCore;

namespace event_finance_service.DbContext
{
    public class FinanceDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }
    }
}
