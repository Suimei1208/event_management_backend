using event_service.Interface;
using event_service.Model;
using event_service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class SpendingController : ControllerBase
    {
        private readonly ISpendingService _spendingService;

        public SpendingController(ISpendingService spendingService)
        {
            _spendingService = spendingService;
        }

        [HttpGet("event/{eventId}/spending")]
        [Authorize]
        public async Task<IActionResult> GetSpendings(int eventId)
        {
            var spendings = await _spendingService.GetSpendingsAsyncByEventId(eventId);

            if (spendings == null || !spendings.Any())
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "No spending data found for this event.",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Spending data fetched successfully.",
                Data = spendings
            });
        }

        //[HttpGet("{id}")]
        //[Authorize]
        //public async Task<IActionResult> GetSpending(int id)
        //{
        //    var spending = await _spendingService.GetSpendingByIdAsync(id);
        //    if (spending == null)
        //    {
        //        return NotFound(new CustomData
        //        {
        //            Success = false,
        //            Message = "Spending not found.",
        //            Data = null
        //        });
        //    }

        //    return Ok(new CustomData
        //    {
        //        Success = true,
        //        Message = "Spending data fetched successfully.",
        //        Data = spending
        //    });
        //}

        [HttpPost("event/{eventId}/spending/add")]
        [Authorize]
        public async Task<IActionResult> AddSpending([FromBody] Spending spending)
        {
            if (spending == null)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Invalid spending data.",
                    Data = null
                });
            }

            var newSpending = await _spendingService.AddSpendingAsync(spending.eventId, spending.category, spending.amount, spending.type);
            return Ok(new CustomData
            {
                Success = true,
                Message = "Spending added successfully.",
                Data = newSpending
            });
        }

        [HttpPut("event/{eventId}/spending/update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSpending(int id, [FromBody] Spending spending)
        {
            var updatedSpending = await _spendingService.UpdateSpendingAsync(id, spending.amount, spending.category);
            if (updatedSpending == null)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "Spending not found.",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Spending updated successfully.",
                Data = updatedSpending
            });
        }

        [HttpDelete("event/{eventId}/spending/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategorySpending(int eventId, int id)
        {
            var success = await _spendingService.RemoveSpendingAsync(eventId, id);
            if (!success)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "Spending not found.",
                    Data = null
                });
            }

            return Ok();
        }
    }
}
