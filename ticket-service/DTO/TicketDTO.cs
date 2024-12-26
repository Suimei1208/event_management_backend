using ticket_service.Model;

namespace ticket_service.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string QRCode { get; set; }
        public string Status { get; set; }
    }
    public static class TicketMapper
    {
        public static TicketDTO ToDTO(Ticket ticket)
        {
            return new TicketDTO
            {
                Id = ticket.id,
                EventId = ticket.EventId,
                UserId = ticket.UserId,
                PurchaseDate = ticket.PurchaseDate,
                QRCode = ticket.QRCode,
                Status = ticket.Status
            };
        }

        // Chuyển đổi từ TicketDTO sang Ticket
        public static Ticket ToEntity(TicketDTO ticketDTO)
        {
            return new Ticket
            {
                id = ticketDTO.Id,
                EventId = ticketDTO.EventId,
                UserId = ticketDTO.UserId,
                PurchaseDate = ticketDTO.PurchaseDate,
                QRCode = ticketDTO.QRCode,
                Status = ticketDTO.Status
            };
        }
    }
}
