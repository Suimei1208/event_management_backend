using event_service;
using event_service.Model;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using user_services.DTO;
using user_services.Interface;
using user_services.Request;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace user_services.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly IFirebaseAuthService _firebaseAuthService;

        public UserService(UserDbContext context, IFirebaseAuthService firebaseAuthService)
        {
            _context = context;
            _firebaseAuthService = firebaseAuthService;
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

        public UserDTO GetUserDetails(FirebaseToken token)
        {
            var user = _context.Users.FirstOrDefault(a => a.Id == token.Uid);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user.ToDTO();
        }

        public async Task<UserDTO> UpdateProfile(string name, string phone, string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            user.Name = name;
            user.Phone = phone;
            await _context.SaveChangesAsync();
            UserDTO currentUser = user.ToDTO();
            return currentUser;
        }     

        public async Task<CustomUser> GetUserDetails(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            if(user == null)
            {
                return new CustomUser();
            }
            var user1 = new CustomUser
            {
                id = user.Id,
                NameFromEmail = user.NameFromEmail,
                userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(user.Id)

            };
            return user1;

        }

        public async Task<List<CustomUser>> SearchUser(string name)
        {
            var listUser = await _context.Users
                .Where(user => user.Name.Contains(name) || user.NameFromEmail.Contains(name))
                .Select(user => new CustomUser
                {
                    id = user.Id,
                    NameFromEmail = user.NameFromEmail,
                })
                .ToListAsync();
            var customListUser = new List<CustomUser>();

            foreach (var user in listUser)
            {
                try
                {
                    customListUser.Add(new CustomUser
                    {
                        id = user.id,
                        NameFromEmail = user.NameFromEmail,
                        userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(user.id)

                    });
                }
                catch (FirebaseAuthException ex)
                {
                    Console.WriteLine($"Firebase error for user {user.id}: {ex.Message}");
                }
            }
            return customListUser;
        }

        public async Task<CustomUser> GetUserByStudentIdAsync(string studentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.NameFromEmail == studentId);

            if (user == null)
                throw new InvalidOperationException("User not found.");

            return new CustomUser
            {
                id = user.Id,
                NameFromEmail = user.NameFromEmail,
            };
        }


        //public async Task<CustomUser> GetUserDetails(string id)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
        //    if (user == null)
        //    {
        //        return new CustomUser();
        //    }
        //    var customUser = new CustomUser();
        //    try
        //    {
        //        customUser.id = user.Id;
        //        customUser.name = user.Name;
        //        //customUser.role = user.Role;
        //        customUser.avtUrl = await _firebaseAuthService.GetUserPhotoUrl(user.Id);
        //    }
        //    catch (FirebaseAuthException ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    return customUser;
        //}

    }
}
