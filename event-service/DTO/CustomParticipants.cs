using Microsoft.Extensions.Logging;
using System.Data;
using System.Xml.Linq;

namespace event_service.DTO
{
    public class CustomParticipants
    {
        public CustomUser user { get; set; }
        public int EventID { get; set; }
        public string Role { get; set; }
        public override string ToString()
        {
            return $"CustomUser: {user.ToString()}, EventID: {EventID}, Role: {Role}"; 
        }
    }
    public class CustomUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string avtUrl { get; set; }
        public override string ToString()
        {
            return $"id: {id}, name: {name}, Role: {role}, avtUrl: {avtUrl}";
        }
    }
}
