using Microsoft.EntityFrameworkCore;
using user_services.Middleware;

namespace event_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<EventDbContext>(options =>
               options.UseMySql(
                   builder.Configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(9, 0, 1))
               )
            );
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<LoggingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
