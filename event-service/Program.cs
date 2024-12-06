using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
                   new MySqlServerVersion(new Version(9, 1,0))
               )
            );
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
                            var authHeader = context.Request.Headers["Authorization"];
                            context.Token = authHeader.ToString();
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            try
                            {
                                Console.WriteLine("Token validation started...");
                                var token = context.SecurityToken as JwtSecurityToken;
                                if (token != null)
                                {
                                    var firebaseToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token.RawData);
                                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, firebaseToken.Uid)
                                }, JwtBearerDefaults.AuthenticationScheme));
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseMiddleware<LoggingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
