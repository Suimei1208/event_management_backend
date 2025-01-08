using document_service.Interface;
using document_service.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace document_service.Controllers
{
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("event/{eventId}/documents")]
        [Authorize]
        public async Task<IActionResult> GetDocuments(int eventId)
        {
            var documents = await _documentService.GetDocumentsByEventIdAsync(eventId);

            if (documents == null || !documents.Any())
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "No documents found for this event.",
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Documents fetched successfully.",
                Data = documents
            });
        }

        [HttpPost("event/{eventId}/documents/add")]
        [Authorize]
        public async Task<IActionResult> AddDocument(int eventId, [FromBody] DocumentList document)
        {
            if (document == null || string.IsNullOrEmpty(document.FileName) || string.IsNullOrEmpty(document.Url))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid document data.",
                });
            }

            document.eventId = eventId;

            var addedDocument = await _documentService.AddDocumentAsync(document);

            return Ok(new
            {
                Success = true,
                Message = "Document added successfully.",
                Data = addedDocument
            });
        }

        [HttpDelete("event/{eventId}/documents/{documentId}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteDocument(string documentId)
        {
            var success = await _documentService.DeleteDocumentAsync(documentId);

            if (!success)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Document not found.",
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Document deleted successfully.",
            });
        }
    }
}
