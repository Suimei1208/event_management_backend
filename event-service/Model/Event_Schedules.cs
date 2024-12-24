using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace event_service.Model
{
    public class Event_Schedules
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
    }
}
