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
        private readonly ICancellationPeriodsService _cancellationPeriodsService;

        public TicketController(ITicketService ticketService, ICancellationPeriodsService cancellationPeriodsService)
        {
            _ticketService = ticketService;
            _cancellationPeriodsService = cancellationPeriodsService;
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

        [HttpPost("tickets/feedback-cancel-event/create")]
        [Authorize]
        public async Task<IActionResult> createFeedbackCancel([FromBody] ticket_cancellation_period period)
        {
            await _cancellationPeriodsService.CreateCancellationPeriods(period);
            return Ok(new
            {
                Success = true,
                Message = "create feedback cancel successfully",
            });
        }

        [HttpGet("feedback/get/{EventId}")]
        [Authorize]
        public async Task<IActionResult> getFeedback(int EventId)
        {
            var result = await _cancellationPeriodsService.GetPeriod(EventId);
            return Ok(new
            {
                Success = true,
                Message = "create feedback cancel successfully",
                Data = result
            });
        }

        [HttpPut("feedback/update")]
        [Authorize]
        public async Task<IActionResult> update([FromBody] ticket_cancellation_period period)
        {
            await _cancellationPeriodsService.update(period);

            return Ok(new
            {
                Success = true,
                Message = "create feedback cancel successfully",
            });
        }
    }
}
