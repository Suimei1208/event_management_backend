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

        public async Task<bool> AddTicket(int eventId, string userId)
        {
            string qrCode = GenerateQRCode(eventId,userId);
            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                QRCode = qrCode,
                Status = "Approved"
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
            string userIdLast6Digits = userId.Length >= 6 ? userId.Substring(userId.Length - 6) : userId;

            string inputString = $"{eventId}{userIdLast6Digits}";

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                string qrCode = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return qrCode;
            }
        }

        public async Task<List<TicketDTO>> GetTicketsByUserId(string userId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return tickets.Select(t => TicketMapper.ToDTO(t)).ToList();
        }
    }
}
