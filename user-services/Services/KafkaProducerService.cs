using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using user_services.DTO;
using user_services.Interface;

namespace user_services.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly string _topic;
        private readonly IUserService _userService;

        public KafkaProducerService(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducerService> logger, IUserService userService)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.Value.BootstrapServers
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = kafkaSettings.Value.Producer.Topic;
            _userService = userService;
        }

        public async Task SendMessageAsync(List<UserInKafka> ListUser)
        {
            try
            {
                List<UserSentKafka> userSentKafka = new List<UserSentKafka>();
                foreach (var user in ListUser)
                {
                    var currentUser = new UserSentKafka
                    {
                        user = await _userService.GetUserDetails(user.UserId),
                        EventID = user.EventId,
                        Role = user.RoleInEvent
                    };
                    userSentKafka.Add(currentUser);
                }
                var message = JsonConvert.SerializeObject(userSentKafka);
                var result = await _producer.ProduceAsync(_topic, new Message<string, string> { 
                    Key = userSentKafka[0].EventID.ToString(),  
                    Value = message 
                });
                _logger.LogInformation($"Message sent to {_topic} at offset {result.Offset}");
            }
            catch (ProduceException<Ignore, string> ex)
            {
                _logger.LogError($"Error sending message: {ex.Error.Reason}");
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
