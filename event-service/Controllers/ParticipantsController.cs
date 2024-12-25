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
    }
}
