using event_service.DTO;
using event_service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using user_services.JsonData;

namespace event_service.Controllers
{
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
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
        public async Task<IActionResult> UpdateEvent(int id, EventDto eventDto)
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
                Data = NoContent()
            }) ;
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

    }
}
