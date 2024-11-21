using event_service;
using user_services.Middleware;
using Microsoft.EntityFrameworkCore;
using user_services.Interface;
using user_services.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace user_services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<FirebaseApp>(provider =>
            {
                return FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("firebase-credentials.json")
                });
            });

            builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();

            builder.Services.AddDbContext<UserDbContext>(options =>
               options.UseMySql(
                   builder.Configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(9, 0, 1))
               )
           );

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();
            // Cấu hình các dịch vụ liên quan đến API
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Cấu hình HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
