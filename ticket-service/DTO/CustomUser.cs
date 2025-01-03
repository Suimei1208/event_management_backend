using FirebaseAdmin.Auth;

namespace ticket_service.DTO
{
    public class CustomUser
    {
        public string id { get; set; }
        public string NameFromEmail { get; set; }
        public UserRecord userRecord { get; set; }
    }
}
