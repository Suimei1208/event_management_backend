using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_service.Interface;
using ticket_service.Model;

namespace ticket_service.Controllers
{
    public class CancellationPeriodsController: ControllerBase
    {
        private readonly ICancellationPeriodsService _cancellationPeriodsService;

        public CancellationPeriodsController(ICancellationPeriodsService cancellationPeriodsService)
        {
            _cancellationPeriodsService = cancellationPeriodsService;
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
                Message = "get feedback cancel successfully",
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
