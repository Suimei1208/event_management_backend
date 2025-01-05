using Microsoft.AspNetCore.Mvc;
using ticket_service.DTO;
using ticket_service.Model;

namespace ticket_service.Interface
{
    public interface ITicketService
    {
        Task<bool> AddTicket(int eventId, string userId, string status);
        Task<List<TicketDTO>> GetTicketsByUserId(string userId);
        Task UpdateStatusTicket(string userid, int eventid, string status);
        Task<dynamic> getQrTicket(string uid, int eid);
    }
}
