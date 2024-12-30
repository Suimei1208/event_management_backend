using Confluent.Kafka;
using event_service.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace event_service.Kafka
{
   

    public class KafkaConsumerService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topic;
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:Consumer:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _topic = configuration["Kafka:Consumer:Topic"];

            _consumer.Subscribe(_topic);
            _serviceProvider = serviceProvider;
        }

        //public async Task<List<CustomParticipants>> ConsumeMessagesAsync(CancellationToken stoppingToken, string eventId)
        //{
        //    var results = new List<CustomParticipants>();

        //    try
        //    {
        //        while (!stoppingToken.IsCancellationRequested)
        //        {
        //            try
        //            {
        //                var consumeResult = _consumer.Consume(stoppingToken);

        //                if (consumeResult != null)
        //                {
        //                    //_logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");

        //                    results = JsonSerializer.Deserialize<List<CustomParticipants>>(consumeResult.Message.Value);
        //                    var key = consumeResult.Message.Key;
                            
        //                    if(key == eventId)
        //                    {
        //                        _consumer.Commit(consumeResult);
        //                    }
        //                    return results;
        //                }
        //            }
        //            catch (ConsumeException ex)
        //            {
        //                _logger.LogError($"Consume error: {ex.Error.Reason}");
        //            }
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        _consumer.Close();
        //    }

        //    return null;
        //}

        public void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}
