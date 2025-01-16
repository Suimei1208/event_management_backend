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

        [HttpGet("event/{eventId}/checked-in-n-out-participants")]
        [Authorize]
        public async Task<IActionResult> GetCheckedInNOutParticipants(int eventId)
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

        [HttpGet("event/{eventId}/checked-in-participants")]
        [Authorize]
        public async Task<IActionResult> GetCheckedInParticipants(int eventId)
        {
            try
            {
                var participants = await _eventAttendanceService.GetCheckedInParticipantsAsync(eventId);

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

        [HttpGet("event/{eventId}/checked-out-participants")]
        [Authorize]
        public async Task<IActionResult> GetCheckedOutParticipants(int eventId)
        {
            try
            {
                var participants = await _eventAttendanceService.GetCheckedOutParticipantsAsync(eventId);

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

        [HttpPost("event/{EventId}/checkin/{inputName?}")]
        [Authorize]
        public async Task<IActionResult> CheckIn(int EventId, string? inputName, [FromBody] CheckInRequest? request)
        {
            if (string.IsNullOrEmpty(inputName) && (request == null || string.IsNullOrEmpty(request.QRCode)))
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Either inputName or QR code must be provided.",
                    Data = null
                });
            }

            try
            {
                // Process the check-in based on inputName
                if (!string.IsNullOrEmpty(inputName))
                {
                    await _eventAttendanceService.RecordCheckInManuallyAsync(EventId, inputName);
                    return Ok(new CustomData
                    {
                        Success = true,
                        Message = $"Check-in successful for {inputName}",
                        Data = null
                    });
                }

                // Process the check-in based on QRCode
                if (request != null && !string.IsNullOrEmpty(request.QRCode))
                {
                    string qrCode = request.QRCode.Trim('"');
                    await _eventAttendanceService.RecordCheckInAsync(qrCode, EventId);
                    return Ok(new CustomData
                    {
                        Success = true,
                        Message = "Check-in successful using QR code.",
                        Data = null
                    });
                }

                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Invalid check-in data provided.",
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


        [HttpPost("event/{EventId}/checkout/{inputName?}")]
        [Authorize]
        public async Task<IActionResult> Checkout(int EventId, string? inputName, [FromBody] CheckInRequest? request)
        {
            if (string.IsNullOrEmpty(inputName) && (request == null || string.IsNullOrEmpty(request.QRCode)))
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Either inputName or QR code must be provided.",
                    Data = null
                });
            }

            try
            {
                // Process the check-out based on inputName
                if (!string.IsNullOrEmpty(inputName))
                {
                    await _eventAttendanceService.RecordCheckOutManuallyAsync(EventId, inputName);
                    return Ok(new CustomData
                    {
                        Success = true,
                        Message = $"Check-out successful for {inputName}",
                        Data = null
                    });
                }

                // Process the check-out based on QRCode
                if (request != null && !string.IsNullOrEmpty(request.QRCode))
                {
                    string qrCode = request.QRCode.Trim('"');
                    await _eventAttendanceService.RecordCheckOutAsync(qrCode, EventId);
                    return Ok(new CustomData
                    {
                        Success = true,
                        Message = "Check-out successful using QR code.",
                        Data = null
                    });
                }

                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Invalid check-out data provided.",
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


        [HttpGet("event/{eventId}/stats")]
        [Authorize]
        public async Task<IActionResult> GetEventStats(int eventId)
        {
            try
            {
                var stats = await _eventAttendanceService.GetEventStatisticsAsync(eventId);

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Event statistics fetched successfully.",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }
    }
}
