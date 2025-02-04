using E_commerce_Back_end.OPT;
using event_service.Service;
using FirebaseAdmin.Auth;

namespace event_service.Middleware
{
    public class sentEmail : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public sentEmail(IServiceScopeFactory scopeFactory)
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
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var _firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();
                    var _events = _context.Events
                        .Where(e => e.Status != "cancel")
                        .ToList();
                    foreach (var ev in _events)
                    {
                        // Kiểm tra thời gian trước một ngày so với StartDate
                        if (DateTime.Now.Date < ev.StartDate.AddDays(-1).Date)
                        {
                            var participants = _context.Participants
                                .Where(p => p.eventId == ev.id && !p.EmailSent)
                                .ToList();
                            foreach (var p in participants)
                            {
                                var emailDto = new EmailDto
                                {
                                    Subject = "Nhắc nhở tham gia sự kiện",
                                };
                                try
                                {
                                    var userEmail = await _firebaseService.GetUserEmailAsync(p.userId);
                                    await _context.SaveChangesAsync();

                                    // Gửi email
                                    emailService.SendEmail(emailDto, userEmail);
                                    Console.WriteLine($"Event {ev.Name}: {ev.Status}");

                                    // Đánh dấu đã gửi email
                                    p.EmailSent = true;
                                    await _context.SaveChangesAsync();
                                }
                                catch (FirebaseAuthException ex)
                                {
                                    Console.WriteLine($"Error fetching user {p.userId}: {ex.Message}");
                                }
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken);
                }
            }
        }
    }
}
