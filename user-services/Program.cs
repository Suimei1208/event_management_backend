using event_service;
using user_services.Middleware;
using Microsoft.EntityFrameworkCore;
using user_services.Interface;
using user_services.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using user_services.DTO;

namespace user_services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
            //builder.Services.AddHostedService<KafkaConsumerService>();
            //builder.Services.AddScoped<KafkaProducerService>();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            builder.Services.AddSingleton<FirebaseApp>(provider =>
            {
                return FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("firebase-credentials.json")
                });
            });
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

            builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();
            builder.Services.AddDbContext<UserDbContext>(options =>
               options.UseMySql(
                   builder.Configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(9, 1, 0))
               ).EnableDetailedErrors()
           );
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            // Cấu hình các dịch vụ liên quan đến API
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Cấu hình HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
