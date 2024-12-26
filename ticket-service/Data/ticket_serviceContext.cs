using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ticket_service.Model;

namespace ticket_service.Data
{
    public class ticket_serviceContext : DbContext
    {
        public ticket_serviceContext (DbContextOptions<ticket_serviceContext> options)
            : base(options)
        {
        }

        public DbSet<ticket_service.Model.Ticket> Ticket { get; set; } = default!;
    }
}
