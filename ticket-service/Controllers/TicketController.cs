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
        public async Task<IActionResult> AddParticipantToSchedule(int eventId, string userId)
        {
            var success = await _ticketService.AddTicket(eventId, userId);

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
                    return Ok(new { Success = true, Message = "No tickets found for this user." });
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while fetching tickets.", Error = ex.Message });
            }
        }

    }
}
