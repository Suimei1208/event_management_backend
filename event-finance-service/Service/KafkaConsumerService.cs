using Confluent.Kafka;
using event_finance_service.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace event_finance_service.Service
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

                                var EventId = consumeResult.Message.Value.ToString();

                                //// Tạo scope mới để sử dụng IServiceProvider
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    //    var kafkaProducerService = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
                                    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
                                    HandleMessage(EventId, dbContext).GetAwaiter().GetResult();
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

        private async Task HandleMessage(string eventid, FinanceDbContext ticketDbContext)
        {
            _logger.LogInformation("đã nhận thông điệp: " + eventid);
           var spending = await ticketDbContext.Spendings.Where(x => x.eventId == int.Parse(eventid)).ToListAsync();
            if (spending.Count == 0)
            {
                _logger.LogInformation("không tìm thấy eventid: " + eventid);
                return;
            }
            ticketDbContext.Spendings.RemoveRange(spending);
            await ticketDbContext.SaveChangesAsync();
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public ConsmerSettings Consumer { get; set; }
    }
    public class ConsmerSettings
    {
        public string GroupId { get; set; }
        public string Topic { get; set; }
        public string AutoOffsetReset { get; set; }
    }
}
