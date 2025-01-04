using E_commerce_Back_end.OPT;
using event_service.Interface;
using event_service.Kafka;
using event_service.Middleware;
using event_service.Model;
using event_service.Service;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace event_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHostedService<EventStatusUpdater>();
            builder.Services.AddHostedService<sentEmail>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<FirebaseService>();

            builder.Services.AddSingleton<FirebaseAuth>(provider =>
            {
                var firebaseApp = provider.GetRequiredService<FirebaseApp>();
                return FirebaseAuth.GetAuth(firebaseApp);
            });

            builder.Services.AddSingleton<FirebaseApp>(provider =>
            {
                try
                {
                    var firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("firebase-credentials.json")
                    });
                    Console.WriteLine("FirebaseApp đã được khởi tạo thành công.");
                    return firebaseApp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi khởi tạo FirebaseApp: {ex.Message}");
                    throw;  // Ném lại ngoại lệ để dừng khởi động ứng dụng nếu có lỗi
                }
            });
            // Add services to the container.
            builder.Services.AddDbContext<EventDbContext>(options =>
               options.UseMySql(
                   builder.Configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(9, 1,0))
               )
            );
            builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(
               op =>
               {
                   op.Authority = "https://securetoken.google.com/event-management-29368";
                   op.Audience = "event-management-29368";
                   op.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var authHeader = context.Request.Headers["Authorization"].ToString();
                           if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                           {
                               var token = authHeader.Substring("Bearer ".Length).Trim();
                               Console.WriteLine(token);
                               context.Token = token.ToString();

                           }
                           return Task.CompletedTask;
                       },
                       OnTokenValidated = async context =>
                       {
                           try
                           {
                               Console.WriteLine("Token validation started...");
                               var token = context.SecurityToken as JwtSecurityToken;
                               //Console.WriteLine(token.RawData);
                               if (token != null)
                               {
                                   var firebaseToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token.RawData);
                                   context.Principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, firebaseToken.Uid)
                               }, JwtBearerDefaults.AuthenticationScheme));
                               }
                               else
                               {
                                   Console.WriteLine("token null");
                                   Console.WriteLine(context.SecurityToken);
                               }
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine($"Error during token validation: {ex.Message}");
                               context.Fail($"Error validating token: {ex.Message}");
                           }
                       },
                       OnAuthenticationFailed = context =>
                       {
                           Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                           return Task.CompletedTask;
                       }
                   };

                   op.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IParticipantsService, ParticipantsService>();
            builder.Services.AddScoped<IEventAttendanceService, EventAttendanceService>();
            builder.Services.AddScoped<ISpecialParticipants, SpecialParticipantsService>();
            builder.Services.AddScoped<INotification, Notification_service>();
            builder.Services.AddScoped<KafkaConsumerService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<LoggingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
