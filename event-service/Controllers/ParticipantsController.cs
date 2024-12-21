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
        //[HttpGet("get")]
        //public async Task<IActionResult> GetParticipants(string eventId)
        //{
        //    await _participantsService.GetParticipants(int.Parse(eventId.ToString()));
        //    return Ok(new CustomData
        //    {
        //        Message = "Get thành công",
        //        Success = true,
        //        Data = null
        //    });
        //}
    }
}
