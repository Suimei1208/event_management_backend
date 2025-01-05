using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ticket_service.DTO;
using ticket_service.Interface;

namespace ticket_service.Service
{
    public class DetailTicketCancellationService : IDetailTicketCancellationService
    {
        private readonly TicketDbContext _context;
        private static readonly HttpClient client = new HttpClient();

        private static IHttpContextAccessor _httpContextAccessor;
        private readonly ITicketService _ticketService;

        public DetailTicketCancellationService(TicketDbContext context, IHttpContextAccessor httpContext, ITicketService ticketService)
        {
            _context = context;
            _httpContextAccessor = httpContext;
            _ticketService = ticketService;
        }

        private static async Task<CustomUser> GetCustomUserAsync(string uid)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var token = httpContext.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync($"http://user-services:5000/api/Users/GetUserById?userId={uid}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                if (responseData.ContainsKey("success") && responseData["success"] == true)
                {
                    //Console.WriteLine("User Data: " + responseData["data"]);

                    var user = new CustomUser()
                    {
                        id = responseData["data"]["id"],
                        avtUrl = responseData["data"]["userRecord"]["photoUrl"],
                        email = responseData["data"]["userRecord"]["email"],
                        NameFromEmail = responseData["data"]["nameFromEmail"],
                        Name = responseData["data"]["userRecord"]["displayName"]
                    };

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

        public async Task CreateDetailTicketCancellation(detail_ticket_cancellation_period_DTO detail)
        {
            await _context.detail_Ticket_Cancellation_Periods.AddAsync(Tdetail_ticket_cancellation_period_Mapper.ToEntity(detail));
            await _context.SaveChangesAsync();
        }

        public async Task<string> getStatusTicketCancellation(int eventId, string uid)
        {
            var result = await _context.detail_Ticket_Cancellation_Periods.FirstOrDefaultAsync(e => e.event_id == eventId && e.uid == uid);
            if(result == null)
            {
                return null;
            }
            return result.status;
        }

        public async Task<List<Deatail_cancel_reponse>> GetDetailCancelAsync(int eventId, string status)
        {
            var listCancel = await _context.detail_Ticket_Cancellation_Periods
                .Where(e => e.event_id == eventId && e.status == status)
                .ToListAsync();

            var result = new List<Deatail_cancel_reponse>();
            foreach (var i in listCancel)
            {
                var newCancel = new Deatail_cancel_reponse
                {
                    id = i.id,
                    event_id = i.event_id,
                    link_image = i.link_image,
                    send_at = i.send_at,
                    status = i.status,
                    reason = i.reason,
                    uid = i.uid,
                    user = await GetCustomUserAsync(i.uid) 
                };
                result.Add(newCancel);
            }
            return result;
        }

        public async Task UpdteDetailCancelAsync(List<string> uids, string status)
        {
            var list = _context.detail_Ticket_Cancellation_Periods.Where(e => uids.Contains(e.uid)).ToList();
            foreach (var item in list)
            {
                item.status = status;
                if (status == "Accepted")
                {
                    await _ticketService.UpdateStatusTicket(item.uid, item.event_id, "Cancelled");
            }
            }
            await _context.SaveChangesAsync();

           
        }
    }
}
