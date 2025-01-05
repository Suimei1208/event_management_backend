using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_service.DTO;
using ticket_service.Interface;
using ticket_service.Model;

namespace ticket_service.Controllers
{
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("event/{eventId}/add-tickets/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddParticipantToSchedule(int eventId, string userId, string status)
        {
            var success = await _ticketService.AddTicket(eventId, userId, status);

            if (!success)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while confirm the ticket",
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Ticket confirm successfully",
            });
        }



        [HttpGet("tickets/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTickets(string userId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByUserId(userId);
                if (tickets == null || tickets.Count == 0)
                {
                    return Ok(new { Success = false, Message = "No tickets found for this user.",});
                }

                return Ok(new
                {
                    success = true,
                    message = "ok",
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while fetching tickets.", Error = ex.Message });
            }
        }

        [HttpGet("tickets/{userId}/qr/{eventid}")]
        [Authorize]
        public async Task<IActionResult> GetTicketsQr(string userId, int eventid)
        {
            try
            {
                var tickets = await _ticketService.getQrTicket(userId, eventid);
                if (tickets == null)
                {
                    return Ok(new { Success = false, Message = "No qr tickets found for this user.", });
                }

                return Ok(new
                {
                    success = true,
                    message = "ok",
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while fetching tickets.", Error = ex.Message });
            }
        }
    }
}
 