namespace event_service.Interface
{
    public interface IKafkaConsumer
    {
        Task<dynamic> ListenForUserRoleChanges(CancellationToken cancellationToken);
    }
}
