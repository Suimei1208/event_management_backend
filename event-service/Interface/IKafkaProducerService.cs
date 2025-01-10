using event_service.DTO;

namespace event_service.Interface
{
   
        public interface IKafkaProducerService
        {
            Task SendMessageAsync(int EventId);
        }
    
}
