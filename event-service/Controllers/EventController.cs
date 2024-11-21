using event_service.DTO;
using event_service.Interface;
using Microsoft.AspNetCore.Mvc;
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

        // Tạo sự kiện mới
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(EventDto eventDto)
        {
            var newEvent = await _eventService.CreateEventAsync(eventDto);
            return Ok(new CustomData
            {
                Success = true,
                Message = "OK",
                Data = newEvent
            });
        }

        // Lấy thông tin sự kiện theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var eventDto = await _eventService.GetEventAsync(id);
            if (eventDto == null)
            {
                return NotFound();
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
        [HttpDelete("{id}")]
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
    }
}
