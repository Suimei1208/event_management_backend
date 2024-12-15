using event_service.Model;
using FirebaseAdmin.Auth;
using user_services.DTO;

namespace user_services.Interface
{
    public interface IUserService
    {
        Task<users> RegisterUserAsync(FirebaseToken token, UserDTO user);
        string getRole(FirebaseToken token);
        Task<UserDTO> UpdateRole(string role, string id);
        UserDTO GetUserDetails(FirebaseToken token);
        Task<UserDTO> UpdateProfile(string name, string phone, string id);
        Task<List<CustomUser>> SearchUser(string name);

    }
}
