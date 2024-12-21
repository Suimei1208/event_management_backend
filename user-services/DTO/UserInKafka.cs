namespace user_services.DTO
{
    public class UserInKafka
    {
        public string UserId { get; set; }
        public int EventId { get; set; }
        public string RoleInEvent { get; set; }
    }
}
