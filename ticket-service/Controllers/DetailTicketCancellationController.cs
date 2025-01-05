using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_service.DTO;
using ticket_service.Interface;

namespace ticket_service.Controllers
{
    public class DetailTicketCancellationController : ControllerBase
    {
        private readonly IDetailTicketCancellationService _detailTicketCancellationService;
        public DetailTicketCancellationController(IDetailTicketCancellationService detailTicketCancellationService)
        {
            _detailTicketCancellationService = detailTicketCancellationService;
        }

        [HttpPost("feedback/user-cancel/create")]
        [Authorize]
        public async Task<IActionResult> createDetailfeedbackCancel([FromBody] detail_ticket_cancellation_period_DTO detail)
        {
            await _detailTicketCancellationService.CreateDetailTicketCancellation(detail);
            return Ok(new
            {
                Success = true,
                Message = "create detail feedback cancel successfully",
            });
        }

        [HttpGet("feedback/user-cancel/get/status")]
        [Authorize]
        public async Task<IActionResult> getDetailfeedbackCancel(int eventid, string uid)
        {
            var result = await _detailTicketCancellationService.getStatusTicketCancellation(eventid, uid);
            return Ok(new
            {
                Success = true,
                Message = "get detail status feedback cancel successfully",
                Data = result
            });
        }

        [HttpGet("feedback/user-cancel/get/list-user/pending")]
        [Authorize]
        public async Task<IActionResult> getListUserCancelPending(int eventId, string status)
        {
            var result = await _detailTicketCancellationService.GetDetailCancelAsync(eventId, status);
            return Ok(new
            {
                Success = true,
                Message = "get detail status feedback cancel successfully",
                Data = result
            });
        }

        [HttpPut("feedback/user-cancel/put/list-user/{status}")]
        [Authorize]
        public async Task<IActionResult> UpdateListUserCancel([FromBody] List<string> uid, string status)
        {
            await _detailTicketCancellationService.UpdteDetailCancelAsync(uid, status);
            return Ok(new
            {
                Success = true,
                Message = "Update detail status feedback cancel successfully",
            });
        }
    }
}
