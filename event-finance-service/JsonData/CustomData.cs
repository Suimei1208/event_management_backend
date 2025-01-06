using Newtonsoft.Json;

namespace user_services.JsonData
{
    public class CustomData
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Object Data { get; set; }
    }
}
