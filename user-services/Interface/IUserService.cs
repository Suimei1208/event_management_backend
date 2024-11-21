using event_service.Model;
using FirebaseAdmin.Auth;
using user_services.DTO;

namespace user_services.Interface
{
    public interface IUserService
    {
        Task<users> RegisterUserAsync(FirebaseToken token, UserDTO user);
    }
}
