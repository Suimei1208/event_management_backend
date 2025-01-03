using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace event_service.Model
{
    public class Special_Participants
    {
        public int id { get; set; }
        public int eventId { get; set; }
        public DateTime registration_Date { get; set; }
        public string name { get; set; }
        public string role{ get; set; }
        public string description{ get; set; }
        public string photoUrl { get; set; }
    }
}
