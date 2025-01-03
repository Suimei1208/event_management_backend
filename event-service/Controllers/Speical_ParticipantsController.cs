using event_service.DTO;
using event_service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class Speical_ParticipantsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ISpecialParticipants _specialparticipantsService;

        public Speical_ParticipantsController(IEventService eventService, ISpecialParticipants specialparticipantsService)
        {
            _eventService = eventService;
            _specialparticipantsService = specialparticipantsService;
        }

        [HttpGet("event/{eventId}/special-participants")]
        [Authorize]
        public async Task<IActionResult> GetSpecialParticipants(int eventId)
        {
            var participants = await _specialparticipantsService.GetSpecialParticipantsAsync(eventId);

            if (participants == null || !participants.Any())
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "No special participants found for this event.",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Special participants fetched successfully.",
                Data = participants
            });
        }

        [HttpDelete("event/{eventId}/remove-special-participant/{participantId}")]
        [Authorize]
        public async Task<IActionResult> RemoveSpecialParticipant(int eventId, int participantId)
        {
            var success = await _specialparticipantsService.RemoveSpecialParticipantAsync(eventId, participantId);

            if (!success)
            {
                return StatusCode(500, new CustomData
                {
                    Success = false,
                    Message = "An error occurred while removing the participant",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Participant removed successfully",
                Data = null
            });
        }


        [HttpPost("event/{eventId}/add-special-participant")]
        [Authorize]
        public async Task<IActionResult> AddSpecialParticipant(int eventId, [FromBody] Special_ParticipantsDto participantDto)
        {
            var success = await _specialparticipantsService.AddSpecialParticipantAsync(
                eventId,
                participantDto.name,
                participantDto.role,
                participantDto.description,
                participantDto.photoUrl
            );


            return Ok(new 
            {
                Success = true,
                Message = "Special participant added successfully to the event",
                
            });
        }
    }
}
