using event_service.Model;

namespace user_services.DTO
{
    public static class UserExtensions
    {
        public static UserDTO ToDTO(this users user)
        {
            if (user == null) return null;  
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };
        }

        public static users ToEntity(this UserDTO userDTO)
        {
            if (userDTO == null) return null;  
            return new users
            {
                Id = userDTO.Id,
                Name = userDTO.Name,
                Email = userDTO.Email,
                Phone = userDTO.Phone,
                Role = userDTO.Role
            };
        }
    }

    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}
