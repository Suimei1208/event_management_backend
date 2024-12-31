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
                NameFromEmail = user.NameFromEmail,
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
                NameFromEmail = ExtractNameFromEmail(userDTO.Email),

            };
        }
        private static string ExtractNameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            var atIndex = email.IndexOf('@');
            return atIndex > 0 ? email.Substring(0, atIndex) : null;
        }
    }

    public class UserDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; } = "Default";
        public required string Email { get; set; } = "default@gmail.com";
        public required string Phone { get; set; } = "0000000000";
        public string NameFromEmail { get; set; }
    }
}
