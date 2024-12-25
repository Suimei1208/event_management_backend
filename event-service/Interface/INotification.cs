namespace event_service.Interface
{
    public interface INotification
    {
        Task<string> SendNotification(string title, string body, string topic);
    }
}
