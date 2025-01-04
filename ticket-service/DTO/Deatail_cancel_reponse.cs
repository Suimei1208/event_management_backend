using Confluent.Kafka;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin.Auth;

namespace ticket_service.DTO
{
    public class Deatail_cancel_reponse
    {
        public required int id { get; set; }
        public required int event_id { get; set; }
        public required string uid { get; set; }
        public CustomUser user { get; set; }
        public required DateTime send_at { get; set; }
        public required string reason { get; set; }
        public required string link_image { get; set; }
        public string status { get; set; }      
    }
}
