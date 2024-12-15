using Confluent.Kafka;
using event_service.Model;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
                    role = role,
                    userId = decodedToken.Uid
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

        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> updateProfile(string name, string phone)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                // Comment out Kafka for now to isolate the issue
                // await _producerService.SendUserProfileToKafka(userId, name, phone);

                var updatedUser = await _userService.UpdateProfile(name, phone, userId);

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
            catch (InvalidOperationException ex)
            {
                // Handle specific known exceptions
                return BadRequest(new CustomData
                {
                    Message = ex.Message,
                    Success = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                // Catch all other exceptions
                return StatusCode(500, new CustomData
                {
                    Message = $"Internal server error: {ex.Message}",
                    Success = false
                });
            }
        }


        [HttpGet("ProfileData")]
        [Authorize]
        public async Task<IActionResult> SendUserDetails(string id)
        {
            try
            {
                var decodedToken = await _firebaseAuthService.VerifyTokenAsync(id);
                var user = _userService.GetUserDetails(decodedToken);
                return Ok(new CustomData
                {
                    Message = "User details fetched successfully.",
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Message = ex.Message,
                    Success = false,
                });
            }
        }
        [HttpGet("SearchUser")]
        [Authorize]
        public async Task<IActionResult> SearchUser(string name)
        {
            try
            {
                List<CustomUser> listCustomUser = await _userService.SearchUser(name);
                return Ok(new CustomData
                {
                    Message = "Search successfully",
                    Success = true,
                    Data = listCustomUser
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomData
                {
                    Message = ex.Message,
                    Success = false,
                });
            }
        }

    }
}
