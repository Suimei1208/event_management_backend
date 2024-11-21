using Confluent.Kafka;
using event_service.Interface;
using Newtonsoft.Json;

namespace event_service.Service
{
    public class KafkaConsumer: IKafkaConsumer
    {
        private readonly IConsumer<Null, string> _consumer;
        public KafkaConsumer()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "event-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _consumer.Subscribe("user-role-changed-topic");
        }

        //Nhận thông tin id người dùng với role
        public async Task<dynamic> ListenForUserRoleChanges(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    dynamic userRoleEvent = JsonConvert.DeserializeObject(consumeResult.Message.Value);

                    return userRoleEvent;
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error: {e.Error.Reason}");
                }
            }
            return null;
        }


    }
}
