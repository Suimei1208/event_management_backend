using event_service.DTO;
using event_service.Interface;
using event_service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("review/add")]
        [Authorize]
        public async Task<IActionResult> addReview([FromBody] ReviewDTO reviewDTO)
        {
            await _reviewService.AddReview(reviewDTO);
            return Ok(new
            {
                Success = true,
                Message = "add successfully"
            });
        }

        [HttpGet("review/get-event")]
        [Authorize]
        public async Task<IActionResult> getEventCompleted(string uid)
        {
            var listEvent = await Task.Run(() => _reviewService.getReview(uid));
            return Ok(new CustomData
            {
                Success = true,
                Message = "successfully",
                Data = listEvent
            });
        }
    }
}
