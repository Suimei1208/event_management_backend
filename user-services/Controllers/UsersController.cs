using Confluent.Kafka;
using event_service.Model;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using user_services.DTO;
using user_services.Interface;
using user_services.JsonData;
using user_services.Request;
using user_services.Services;

namespace user_services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IUserService _userService;
        private readonly IKafkaProducerService _producerService;
        private readonly JwtService _jwtService;

        public UsersController(IFirebaseAuthService firebaseAuthService, 
            IUserService userService, IKafkaProducerService kafkaProducerService, JwtService jwtService)
        {
            _firebaseAuthService = firebaseAuthService;
            _userService = userService;
            _producerService = kafkaProducerService;
            _jwtService = jwtService;   
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            var firebaseToken = user.Id;
            var decodedToken = await _firebaseAuthService.VerifyTokenAsync(firebaseToken);

            var existingUser = await _userService.RegisterUserAsync(decodedToken, user);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            await _producerService.SendUserRoleToKafka(user.Id, user.Role);

            return Ok(
                new CustomData
                {
                    Message = "User registered successfully",
                    Success = true,
                    Data = user
                });
        }

        // Khi làm update nhớ sử dụng SendUserRoleToKafka

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string firebaseIdToken)
        {
            var decodedToken = await _firebaseAuthService.VerifyTokenAsync(firebaseIdToken);
            var customJwt = _jwtService.GenerateJwtToken(decodedToken.Uid, decodedToken.Claims["email"].ToString());
            return Ok(new { Token = customJwt });
        }
    }
}
