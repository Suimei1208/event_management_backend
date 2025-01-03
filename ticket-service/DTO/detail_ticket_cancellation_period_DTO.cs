using Confluent.Kafka;
using ticket_service.Model;
using System.Net.Http;
using Newtonsoft.Json;

namespace ticket_service.DTO
{
    public class detail_ticket_cancellation_period_DTO
    {
        public int id { get; set; }
        public int event_id { get; set; }
        public string uid { get; set; }
        public CustomUser customUser { get; set; }
        public DateTime send_at { get; set; }
        public string reason { get; set; }
        public string link_image { get; set; }
    }

    public static class Tdetail_ticket_cancellation_period_Mapper
    {
        private static readonly HttpClient client = new HttpClient();
        public static detail_ticket_cancellation_period_DTO ToDTO(detail_ticket_cancellation_period detail)
        {
            return new detail_ticket_cancellation_period_DTO
            {
                id = detail.id,
                event_id = detail.event_id,
                uid = detail.uid,
                customUser = GetCustomUserAsync(detail.uid).Result,
                send_at = detail.send_at,
                reason = detail.reason,
                link_image = detail.link_image
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
                link_image = toEntity.link_image
            };
        }

        private static async Task<CustomUser> GetCustomUserAsync(string uid)
        {
            HttpResponseMessage response = await client.GetAsync($"https://user-services/api/Users/GetUserById?userId={uid}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                if (responseData.ContainsKey("success") && responseData["success"] == true)
                {

                    var user = JsonConvert.DeserializeObject<CustomUser>(responseData["data"].ToString());
                    Console.WriteLine($"User Data: {user}");

                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
