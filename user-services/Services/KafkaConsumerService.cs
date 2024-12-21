using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using user_services.DTO;

namespace user_services.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        private readonly IServiceProvider _serviceProvider; // Thêm IServiceProvider

        public KafkaConsumerService(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.Value.BootstrapServers,
                GroupId = kafkaSettings.Value.Consumer.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _topic = kafkaSettings.Value.Consumer.Topic;

            _consumer.Subscribe(_topic);
            _serviceProvider = serviceProvider; // Gán IServiceProvider
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(stoppingToken);

                            if (consumeResult != null)
                            {
                                _logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");

                                var userEvent = JsonSerializer.Deserialize<List<UserInKafka>>(consumeResult.Message.Value);

                                // Tạo scope mới để sử dụng IServiceProvider
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var kafkaProducerService = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
                                    HandleMessage(userEvent, kafkaProducerService).GetAwaiter().GetResult();
                                }

                                _consumer.Commit(consumeResult);
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"Consume error: {ex.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _consumer.Close();
                }
            }, stoppingToken);
        }

        private async Task HandleMessage(List<UserInKafka> userEvent, KafkaProducerService kafkaProducerService)
        {
            await kafkaProducerService.SendMessageAsync(userEvent);
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
