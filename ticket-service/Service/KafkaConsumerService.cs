using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ticket_service.Service
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

                                var EventId =consumeResult.Message.Value.ToString();

                                //// Tạo scope mới để sử dụng IServiceProvider
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    //    var kafkaProducerService = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
                                    var dbContext = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
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

        private async Task HandleMessage(string eventid, TicketDbContext ticketDbContext)
        {
            _logger.LogInformation("đã nhận thông điệp: " + eventid);
            var ticket = await ticketDbContext.Tickets.Where(x => x.EventId == int.Parse(eventid)).ToListAsync();
            ticketDbContext.Tickets.RemoveRange(ticket);
            var cancellationPeriod = await ticketDbContext.CancellationPeriods.Where(x => x.event_id == int.Parse(eventid)).ToListAsync();
            ticketDbContext.CancellationPeriods.RemoveRange(cancellationPeriod);
            var detailTicketCancellationPeriod = await ticketDbContext.detail_Ticket_Cancellation_Periods.Where(x => x.event_id == int.Parse(eventid)).ToListAsync();
            ticketDbContext.detail_Ticket_Cancellation_Periods.RemoveRange(detailTicketCancellationPeriod);
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
