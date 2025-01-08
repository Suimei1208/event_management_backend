using document_service.DbContext;
using document_service.Interface;
using document_service.Model;
using Microsoft.EntityFrameworkCore;

namespace document_service.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentDbContext _context;

        public DocumentService(DocumentDbContext context)
        {
            _context = context;
        }

        // Fetch documents by eventId
        public async Task<List<DocumentList>> GetDocumentsByEventIdAsync(int eventId)
        {
            return await _context.Document
                .Where(d => d.eventId == eventId)
                .ToListAsync();
        }

        // Add a new document
        public async Task<DocumentList> AddDocumentAsync(DocumentList document)
        {
            document.Id = Guid.NewGuid().ToString();
            document.UploadedAt = DateTime.UtcNow;

            _context.Document.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<bool> DeleteDocumentAsync(string documentId)
        {
            var document = await _context.Document
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                return false;
            }

            _context.Document.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
