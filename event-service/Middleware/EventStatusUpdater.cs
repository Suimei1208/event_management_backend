using E_commerce_Back_end.OPT;
using event_service.Model;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using event_service.Service;

namespace event_service.Middleware
{
    public class EventStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventStatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<EventDbContext>();
                    //var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    //var _firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();

                    var _events = _context.Events
                        .Where(e => e.Status != "Cancel")
                        .ToList();

                    foreach (var ev in _events)
                    {
                        //var emailDto = new EmailDto
                        //{
                        //    Subject = "Nhắc nhở tham gia sự kiện",
                        //};

                        //try
                        //{
                        //    var user = await _firebaseService.GetUserEmailAsync(ev.IdCreate);
                        //    var userEmail = user;

                            UpdateEventStatus(ev);
                            await _context.SaveChangesAsync();

                            // Gửi email
                            //emailService.SendEmail(emailDto, userEmail);
                            Console.WriteLine($"Event {ev.Name}: {ev.Status}");
                        //}
                        //catch (FirebaseAuthException ex)
                        //{
                        //    Console.WriteLine($"Error fetching user {ev.IdCreate}: {ex.Message}");
                        //}
                    }

                    await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken);
                }
            }
        }

        private void UpdateEventStatus(Events myEvent)
        {
            DateTime now = DateTime.Now;

            if (now < myEvent.StartDate)
            {
                myEvent.Status = "Upcoming";
            }
            else if (now >= myEvent.StartDate && now <= myEvent.EndDate)
            {
                myEvent.Status = "Ongoing";
            }
            else if (now > myEvent.EndDate)
            {
                myEvent.Status = "Completed";
            }
        }
    }
}
