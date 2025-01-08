using event_service.DTO;
using event_service.Interface;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class ReviewService : IReviewService
    {
        private readonly EventDbContext _context;
        private readonly IEventService _eventService;
        public ReviewService(EventDbContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        public async Task<List<dynamic>> getReview(string uid)
        {
            var list = await _eventService.GetEventStatus("Completed");
            List<dynamic> result = new List<dynamic>();
            foreach (var i in list)
            {
                var result1 = await _context.Reviews.FirstOrDefaultAsync(e => e.uid == uid && e.Eventid == i.id);
                var addlist = new
                {
                    id = i.id,
                    Name = i.Name,
                    IdCreate = i.IdCreate,
                    Description = i.Description,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    Location = i.Location,
                    TargetAudience = i.TargetAudience,
                    type = i.type,
                    status = i.status,
                    Banner = i.Banner,
                    eventCode = i.eventCode,
                    access = i.access,
                    allowSelectSchedule = i.allowSelectSchedule,
                    isReview = (result1 == null) ? false : true
                };
                result.Add(addlist);
            }
            return result;
        }

        public async Task AddReview(ReviewDTO reviewDTO)
        {
            await _context.AddAsync(ReviewMapper.ToEntity(reviewDTO));
            await _context.SaveChangesAsync();
        }
    }
}
