using event_service.DTO;
using event_service.Interface;
using event_service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class ParticipantsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IParticipantsService _participantsService;

        public ParticipantsController(IEventService eventService, IParticipantsService participantsService)
        {
            _eventService = eventService;
            _participantsService = participantsService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddParticipants([FromBody] List<ParticipantsDto> participantsDtos)
        {
            if (participantsDtos == null || participantsDtos.Count == 0)
            {
                return BadRequest("Danh sách tham gia không hợp lệ.");
            }

            await _participantsService.AddParticipants(participantsDtos);

            return Ok(new CustomData
            {
                Message = "Add thành công",
                Success = true,
                Data = null
            });
        }
        [HttpGet("get-event-register-pending")]
        [Authorize]
        public async Task<IActionResult> GetEventParticipantsRegisterStatus(string uid)
        {
            var result = await _participantsService.getEventRegisterPending(uid);
            return Ok(new CustomData
            {
                Message = "Get thành công",
                Success = true,
                Data = result
            });
        }
        
        [HttpDelete("participants/unregister")]
        public async Task<IActionResult> unregister(string uid, string eventid)
        {
            await _participantsService.UnregisterEvent(eventid, uid);
            return Ok(new CustomData
            {
                Message = "Get thành công",
                Success = true,
                Data = null
            });
        }

        [HttpPost("event/add-participants-excel/{eventId}")]
        public async Task<IActionResult> AddParticipantsFromExcel(int eventId, [FromBody] List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return BadRequest("User IDs are required.");
            }

            try
            {
                var result = await _participantsService.AddParticipantsFromExcelAsync(eventId, userIds);
                return Ok(new CustomData
                {
                    Message = "Add from excel thành công",
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("particpant/get-role")]
        [Authorize]
        public async Task<IActionResult> GetParticipantRole(string userId, int eventId)
        {
            var result = await _participantsService.GetParticipantRoleByUserIdAsync(userId, eventId);

            if (result == null)
            {
                return NotFound(new CustomData
                {
                    Message = "Participant not found or not registered for the event",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Message = "Role retrieved successfully",
                Success = true,
                Data = result
            });
        }
    }
}
