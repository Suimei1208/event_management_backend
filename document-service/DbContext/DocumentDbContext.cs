using document_service.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace document_service.DbContext
{
    public class DocumentDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options)
            : base(options)
        {
        }
        public DbSet<DocumentList> Document { get; set; }
    }
}
