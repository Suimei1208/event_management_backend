using event_service.Model;

namespace event_service.DTO
{
    public class ReviewDTO
    {
        public int id { get; set; }
        public int Eventid { get; set; }
        public string uid { get; set; }
        public int rate { get; set; }
        public string? review { get; set; }
    }
    public static class ReviewMapper
    {
        public static Review ToEntity(this ReviewDTO reviewDTO)
        {
            return new Review
            {
                id = reviewDTO.id,
                Eventid = reviewDTO.Eventid,
                uid = reviewDTO.uid,
                rate = reviewDTO.rate,
                review = reviewDTO.review
            };
        }

        public static ReviewDTO ToDto(this Review review)
        {
            return new ReviewDTO
            {
                id = review.id,
                Eventid = review.Eventid,
                uid = review.uid,
                rate = review.rate,
                review = review.review
            };
        }
    }
}
