namespace user_services.DTO
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public ProducerSettings Producer { get; set; }
        public ConsmerSettings Consumer { get; set; }
    }

    public class ProducerSettings
    {
        public string Topic { get; set; }
        public string Acks { get; set; }
        public int Retries { get; set; }
        public int BatchSize { get; set; }
    }

    public class ConsmerSettings {
        public string GroupId { get; set; }
        public string Topic { get; set; }
        public string AutoOffsetReset { get; set; }
}
}
