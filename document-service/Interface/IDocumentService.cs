using document_service.Model;

namespace document_service.Interface
{
    public interface IDocumentService
    {
        Task<List<DocumentList>> GetDocumentsByEventIdAsync(int eventId);
        Task<DocumentList> AddDocumentAsync(DocumentList document);
        Task<bool> DeleteDocumentAsync(string documentId);
    }
}
