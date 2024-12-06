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

            //await _producerService.SendUserRoleToKafka(user.Id, user.Role);

            return Ok(
                new CustomData
                {
                    Message = "User registered successfully",
                    Success = true,
                    Data = user
                });
        }

        // Khi làm update nhớ sử dụng SendUserRoleToKafka

        [HttpGet("login")]
        public async Task<IActionResult> Login(string firebaseIdToken)
        {
            var decodedToken = await _firebaseAuthService.VerifyTokenAsync(firebaseIdToken);         
            var role = _userService.getRole(decodedToken);
            if(role != "None")
            {
                await _producerService.SendUserRoleToKafka(decodedToken.Uid, role);
            }            
            return Ok(new CustomData{ 
                Message = "GET ROLE DONE!",
                Success = true,
                Data =new 
                {
                    //Token = customJwt,
                    role = role
                } 
            });
        }

        [HttpPost("UpdateRole")]
        [Authorize]
        public async Task<IActionResult> updateRole(string role)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            await _producerService.SendUserRoleToKafka(userId, role);

            var updatedUser = await _userService.UpdateRole(role, userId);

            if (updatedUser == null)
            {
                return NotFound("User update failed.");
            }
            return Ok(new CustomData
            {
                Message = "OK",
                Success = true,
                Data = updatedUser
            });
        }


    }
}
