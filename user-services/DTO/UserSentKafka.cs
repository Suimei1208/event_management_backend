namespace user_services.DTO
{
    public class UserSentKafka
    {
        public CustomUser user{ get; set; }
        public int EventID { get; set; }
        public string Role { get; set; }
    }
}
