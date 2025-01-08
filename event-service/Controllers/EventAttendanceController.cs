using event_service.DTO;
using event_service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class EventAttendanceController : ControllerBase
    {
        private readonly IEventAttendanceService _eventAttendanceService;

        public EventAttendanceController(IEventAttendanceService eventAttendanceService)
        {
            _eventAttendanceService = eventAttendanceService;
        }

        [HttpGet("event/{eventId}/participants")]
        [Authorize]
        public async Task<IActionResult> GetParticipants(int eventId)
        {
            try
            {
                var participants = await _eventAttendanceService.GetCheckedInAndCheckedOutParticipantsAsync(eventId);

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Participants fetched successfully.",
                    Data = participants
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpPost("event/{EventId}/checkin")]
        [Authorize]
        public async Task<IActionResult> CheckIn(int EventId, [FromBody] CheckInRequest request)
        {
            if (request == null)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Request cannot be null.",
                    Data = null
                });
            }

            if (string.IsNullOrEmpty(request.QRCode))
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "QR code cannot be null or empty.",
                    Data = null
                });
            }

            try
            {
                string qrCode = request.QRCode.Trim('"');

                await _eventAttendanceService.RecordCheckInAsync(qrCode);
                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Check-in successful",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost("event/{EventId}/checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout(int EventId, [FromBody] CheckInRequest request)
        {
            if (request == null)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Request cannot be null.",
                    Data = null
                });
            }

            if (string.IsNullOrEmpty(request.QRCode))
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "QR code cannot be null or empty.",
                    Data = null
                });
            }

            try
            {
                string qrCode = request.QRCode.Trim('"');

                await _eventAttendanceService.RecordCheckOutAsync(qrCode);
                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Check-in successful",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
