using Microsoft.AspNetCore.Mvc;
using ticket_service.DTO;
using ticket_service.Model;

namespace ticket_service.Interface
{
    public interface ITicketService
    {
        Task<bool> AddTicket(int eventId, string userId, string status);
        Task<List<TicketDTO>> GetTicketsByUserId(string userId);
    }
}
