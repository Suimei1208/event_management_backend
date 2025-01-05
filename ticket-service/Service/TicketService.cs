using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using ticket_service.DTO;
using ticket_service.Interface;
using ticket_service.Model;

namespace ticket_service.Service
{
    public class TicketService : ITicketService
    {
        private readonly TicketDbContext _context;

        public TicketService(TicketDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTicket(int eventId, string userId, string status)
        {
            string qrCode = GenerateQRCode(eventId,userId);
            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                QRCode = qrCode,
                Status = status
            };

            try
            {
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GenerateQRCode(int eventId, string userId)
        {
            string encodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId));
            string encodedEventId = Convert.ToBase64String(Encoding.UTF8.GetBytes(eventId.ToString()));

            string inputString = $"{encodedEventId}-{encodedUserId}";

            return inputString;
        }


        private string ValidateQRCode(string qrCode)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(qrCode);
                string decodedString = Encoding.UTF8.GetString(bytes);
                return decodedString; // e.g., "userId-eventId"
            }
            catch (FormatException)
            {
                // Handle invalid base64 input
                return null;
            }
        }


        public async Task<List<TicketDTO>> GetTicketsByUserId(string userId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return tickets.Select(t => TicketMapper.ToDTO(t)).ToList();
        }

        public async Task UpdateStatusTicket(string userid, int eventid, string status)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(e => e.UserId == userid && e.EventId == eventid);
            ticket.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<dynamic> getQrTicket(string uid, int eid)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(e => e.UserId == uid && e.EventId == eid);
            return new { 
                qr = ticket?.QRCode,
                statusTicket = ticket?.Status
            };
        }
    }
}
