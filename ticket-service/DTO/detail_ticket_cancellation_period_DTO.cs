using Confluent.Kafka;
using ticket_service.Model;

using Newtonsoft.Json;

namespace ticket_service.DTO
{
    public class detail_ticket_cancellation_period_DTO
    {
        public required int id { get; set; }
        public required int event_id { get; set; }
        public required string uid { get; set; }
        public required DateTime send_at { get; set; }
        public required string reason { get; set; }
        public required string link_image { get; set; }
        public string status { get; set; }
    }

    public static class Tdetail_ticket_cancellation_period_Mapper
    {
        
        public static detail_ticket_cancellation_period_DTO ToDTO(detail_ticket_cancellation_period detail)
        {
            return new detail_ticket_cancellation_period_DTO
            {
                id = detail.id,
                event_id = detail.event_id,
                uid = detail.uid,
                send_at = detail.send_at,
                reason = detail.reason,
                link_image = detail.link_image,
                status = detail.status
            };
        }

        public static detail_ticket_cancellation_period ToEntity(detail_ticket_cancellation_period_DTO toEntity)
        {

            return new detail_ticket_cancellation_period
            {
                id = toEntity.id,
                event_id = toEntity.event_id,
                uid = toEntity.uid,
                send_at = toEntity.send_at,
                reason = toEntity.reason,
                link_image = toEntity.link_image,
                status = toEntity.status
            };
        }

       
    }
}
