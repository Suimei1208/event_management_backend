namespace event_service.Model
{
    public class Spending
    {
        public int id { get; set; }
        public int eventId { get; set; }
        public double amount { get; set; }
        public string category { get; set; }
        public string type { get; set; }
    }
}
