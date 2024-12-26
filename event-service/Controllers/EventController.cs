using event_service.DTO;
using event_service.Interface;
using event_service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly INotification _notification;

        public EventController(IEventService eventService, INotification notification)
        {
            _eventService = eventService;
            _notification = notification;
        }
        [HttpGet("getid")]
        [Authorize]
        public async Task<IActionResult> getidEvent(string idCreate, string name)
        {
            return Ok(new CustomData
            {
                Success = true,
                Message = "Event created successfully",
                Data = await _eventService.GetIdEvent(idCreate, name)
            });
        }

        // Tạo sự kiện mới
        [HttpPost("create-event")]
        [Authorize]
        public async Task<ActionResult<CustomData>> CreateEvent([FromBody] EventDto eventDto)
        {
            if (eventDto == null)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new CustomData
                {
                    Success = false,
                    Message = "User is not authorized",
                    Data = null
                });
            }
            eventDto.IdCreate = userId;

            var newEvent = await _eventService.CreateEventAsync(eventDto);

            return Ok(new CustomData
            {
                Success = true,
                Message = "Event created successfully",
                Data = newEvent
            });
        }


        // Lấy thông tin sự kiện theo ID
        [HttpGet("get-event")]
        [Authorize]
        public async Task<ActionResult<EventDto>> GetEvent()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var eventDto = await _eventService.GetEventAsync(userId);
            if (eventDto == null)
            {
                return NoContent();
            }
            return Ok(new CustomData
            {
                Success = true,
                Message = "OK",
                Data = eventDto
            });
        }

        // Lọc sự kiện theo loại hình
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByCategory(string category)
        {
            var events = await _eventService.GetEventsByCategoryAsync(category);
            return Ok(events);
        }

        // Chỉnh sửa sự kiện
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventWithParticipantsDto eventDto)
        {
            var updated = await _eventService.UpdateEventAsync(id, eventDto);
            if (!updated)
            {
                return NotFound();
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Edit done",
                Data = updated,
            });
        }


        // Xóa sự kiện
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok(new CustomData
            {
                Success = true,
                Message = "Delete done",
                Data = NoContent()
            });
        }

        [HttpGet]
        public IActionResult GetSecureData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok($"Hello, user {userId}");
        }

        [HttpGet("event/{id}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventWithParticipants = await _eventService.GetEventByIdAsync(id);

                if (eventWithParticipants == null)
                {
                    return NotFound(new CustomData
                    {
                        Success = false,
                        Message = "Event not found",
                        Data = null
                    });
                }

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Event retrieved successfully",
                    Data = eventWithParticipants
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CustomData
                {
                    Success = false,
                    Message = "An internal error occurred. Please try again later.",
                    Data = null
                });
            }
        }


        [HttpGet("get-register-event")]
        [Authorize]
        public async Task<IActionResult> GetEventHomePage(string uid, string role)
        {
            var result = await _eventService.GetEventHomePage(uid, role);
            if (result == null)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "No events register",
                    Data = null
                });
            }
            return Ok(new CustomData
            {
                Success = true,
                Message = "Fetch successfully",
                Data = result
            });
        }

        // Create a schedule for an event
        [HttpPost("event/{eventId}/create-schedule")]
        [Authorize]
        public async Task<ActionResult<CustomData>> CreateSchedule(int eventId, [FromBody] ScheduleDto scheduleDto)
        {
            if (scheduleDto == null)
            {
                return BadRequest(new CustomData
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });
            }

            var schedule = await _eventService.CreateScheduleAsync(eventId, scheduleDto);

            return Ok(new CustomData
            {
                Success = true,
                Message = "Schedule created successfully",
                Data = schedule
            });
        }

        // Get schedules for an event
        [HttpGet("event/{eventId}/schedules")]
        [Authorize]
        public async Task<ActionResult<CustomData>> GetSchedulesForEvent(int eventId)
        {
            var schedules = await _eventService.GetSchedulesForEventAsync(eventId);

            return Ok(new CustomData
            {
                Success = true,
                Message = "Schedules fetched successfully",
                Data = schedules
            });
        }

        // Update a schedule
        [HttpPut("event/{scheduleId}/schedules")]
        [Authorize]
        public async Task<ActionResult<CustomData>> UpdateSchedule(int scheduleId, [FromBody] ScheduleDto scheduleDto)
        {
            var updated = await _eventService.UpdateScheduleAsync(scheduleId, scheduleDto);
            if (!updated)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "Schedule not found",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Schedule updated successfully",
                Data = updated
            });
        }

        // Delete a schedule
        [HttpDelete("schedule/{scheduleId}")]
        [Authorize]
        public async Task<ActionResult<CustomData>> DeleteSchedule(int scheduleId)
        {
            var deleted = await _eventService.DeleteScheduleAsync(scheduleId);
            if (!deleted)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "Schedule not found",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Schedule deleted successfully",
                Data = null
            });
        }
       [HttpPost("notification")]
        [Authorize]
        public async Task<IActionResult> SendNotification(string topic, string title, string body)
        {
            var result = await _notification.SendNotification(title,body,topic);
            return Ok(new CustomData
            {
                Success = true,
                Message = "sent successfully",
                Data = result
            }); 
        }

        [HttpGet("event/can-register")]
        //[Authorize]
        public async Task<IActionResult> getEventCanRegister()
        {
            var listEvent = await _eventService.GetEventStatus("Upcoming");
            return Ok(new CustomData
            {
                Success = true,
                Message = "successfully",
                Data = listEvent
            }); 
        }

        [HttpGet("event/{eventId}/participants/{role}")]
        [Authorize]
        public async Task<IActionResult> GetParticipantsByEventIdAndRole(int eventId, string role)
        {
            try
            {
                var participants = await _eventService.GetParticipantsByEventIdAndRoleAsync(eventId, role);

                if (participants == null || !participants.Any())
                {
                    return NotFound(new CustomData
                    {
                        Success = false,
                        Message = "No participants found for the given event and role",
                        Data = null
                    });
                }

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Participants fetched successfully",
                    Data = participants
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CustomData
                {
                    Success = false,
                    Message = "An internal error occurred. Please try again later.",
                    Data = null
                });
            }
        }

        [HttpDelete("event/{eventId}/participants/{participantId}/role/{role}")]
        [Authorize]
        public async Task<IActionResult> DeleteParticipant(int eventId, int participantId, string role)
        {
            try
            {
                var result = await _eventService.DeleteParticipantAsync(eventId, participantId, role);

                if (!result)
                {
                    return NotFound(new CustomData
                    {
                        Success = false,
                        Message = "Participant not found or couldn't be deleted.",
                        Data = null
                    });
                }

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Participant deleted successfully.",
                    Data = null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CustomData
                {
                    Success = false,
                    Message = "An internal error occurred while deleting the participant. Please try again later.",
                    Data = null
                });
            }
        }

        [HttpGet("event/{eventId}/participants/pending")]
        [Authorize]
        public async Task<IActionResult> GetPendingParticipants(int eventId)
        {
            try
            {
                var participants = await _eventService.GetPendingParticipantsAsync(eventId);

                if (participants == null)
                {
                    return NotFound(new CustomData
                    {
                        Success = false,
                        Message = "No pending participants found for the given event",
                        Data = null
                    });
                }

                return Ok(new CustomData
                {
                    Success = true,
                    Message = "Participants fetched successfully",
                    Data = participants
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new CustomData
                {
                    Success = false,
                    Message = "An internal error occurred. Please try again later.",
                    Data = null
                });
            }
        }

        [HttpPut("event/{eventId}/participants/{participantId}/approve")]
        [Authorize]
        public async Task<IActionResult> ApproveParticipant(int eventId, int participantId)
        {
            var result = await _eventService.ApproveParticipantAsync(eventId, participantId);
            if (!result)
            {
                return NotFound(new CustomData
                {
                    Success = false,
                    Message = "Participant not found or already approved",
                    Data = null
                });
            }

            return Ok(new CustomData
            {
                Success = true,
                Message = "Participant approved successfully",
                Data = null
            });
        }
    }
}
