using Confluent.Kafka;
using event_service.DTO;
using event_service.Interface;
using System.Text.Json;

namespace event_service.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],             
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
            _topic = configuration["Kafka:Producer:Topic"];
        }

        public async Task SendMessageAsync(int EventId)
        {
            
            var messageValue = JsonSerializer.Serialize(EventId);
            var message = new Message<string, string>
            {
                Value = messageValue,
                Key = messageValue
            };
            try
            {
                var deliveryResult = await _producer.ProduceAsync(_topic, message);
                Console.WriteLine($"Đã gửi thông điệp tới Partition: {deliveryResult.Partition}, Offset: {deliveryResult.Offset}");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Lỗi khi gửi thông điệp: {ex.Error.Reason}");
            }

        }
    }
}
