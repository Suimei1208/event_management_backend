using event_service;
using event_service.Model;
using FirebaseAdmin.Auth;
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
    }
}
