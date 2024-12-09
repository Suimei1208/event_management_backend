namespace user_services.Interface
{
    public interface IKafkaProducerService
    {
        Task SendUserRoleToKafka(string userId, string newRole);
        Task SendUserProfileToKafka(string userId, string newName, string newPhone);
    }

}
