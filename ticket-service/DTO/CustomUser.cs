using FirebaseAdmin.Auth;
using System.Text.Json.Serialization;

namespace ticket_service.DTO
{
    public class CustomUser
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string NameFromEmail { get; set; }
        public string email { get; set; }
        public string avtUrl { get; set; }
       
    }
}
