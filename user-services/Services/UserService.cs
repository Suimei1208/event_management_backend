using event_service;
using event_service.Model;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using user_services.DTO;
using user_services.Interface;
using user_services.Request;

namespace user_services.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<users> RegisterUserAsync(FirebaseToken token, UserDTO user)
        {
            if (token == null || string.IsNullOrEmpty(token.Uid))
            {
                throw new ArgumentException("Invalid Firebase token.");
            }

            var newUser = user.ToEntity();
            newUser.Id = token.Uid;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public string getRole(FirebaseToken token)
        {
            if (token == null || string.IsNullOrEmpty(token.Uid))
            {
                throw new ArgumentException("Invalid Firebase token.");
            }

            // Tìm người dùng trong cơ sở dữ liệu dựa trên Uid
            var user = _context.Users.FirstOrDefault(a => a.Id == token.Uid);

            if (user == null)
            {
                throw new InvalidOperationException("User not found for the given token.");
            }

            return user.Role;
        }

        public async Task<UserDTO> UpdateRole(string role, string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            user.Role = role;
            await _context.SaveChangesAsync();
            UserDTO currentUser = user.ToDTO();
            return currentUser;

        }
    }
}
