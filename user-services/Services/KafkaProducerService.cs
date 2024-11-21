using Confluent.Kafka;
using Newtonsoft.Json;
using user_services.Interface;

namespace user_services.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic = "user-topic";

        public KafkaProducerService()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",  
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendUserRoleToKafka(string userId, string newRole)
        {
            var userRoleEvent = new 
            {
                UserId = userId,
                NewRole = newRole
            };

            var eventMessage = JsonConvert.SerializeObject(userRoleEvent);
            var message = new Message<Null, string> { Value = eventMessage };

            await _producer.ProduceAsync(_topic, message);
        }
    }

}
