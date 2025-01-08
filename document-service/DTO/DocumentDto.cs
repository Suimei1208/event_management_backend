namespace document_service.DTO
{
    public class DocumentDto
    {
        public string Id { get; set; }
        public int eventId { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
