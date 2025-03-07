using event_service.Interface;
using event_service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class ScheduleParticipantController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IScheduleParticipants _scheduleparticipantsService;

        public ScheduleParticipantController(IEventService eventService, IScheduleParticipants scheduleparticipantsService)
        {
            _eventService = eventService;
            _scheduleparticipantsService = scheduleparticipantsService;
        }

        [HttpGet("event/{eventId}/schedule-participants")]
        [Authorize]
        public async Task<IActionResult> GetScheduleParticipants(int eventId)
        {
            var participants = await _scheduleparticipantsService.GetScheduleParticipantsAsync(eventId);

            if (participants == null || !participants.Any())
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "No schedule participants found for this event.",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Schedule participants fetched successfully.",
                Data = participants
            });
        }

        [HttpDelete("event/{eventId}/remove-schedule-participant/{participantId}")]
        [Authorize]
        public async Task<IActionResult> RemoveSpecialParticipant(int eventId, string participantId)
        {
            var success = await _scheduleparticipantsService.RemoveScheduleParticipantAsync(eventId, participantId);

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
    }
}
