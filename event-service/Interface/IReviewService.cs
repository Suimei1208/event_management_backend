using event_service.DTO;

namespace event_service.Interface
{
    public interface IReviewService
    {
        Task<List<dynamic>> getReview(string uid);
        Task AddReview(ReviewDTO reviewDTO);
    }
}
