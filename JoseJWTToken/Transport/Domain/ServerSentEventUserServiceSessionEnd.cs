using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServerSentEventUserServiceSessionEnd : IServerSentEvent
    {
        [JsonProperty("service_user_hash")]
        public string UserHash { get; set; }

        [JsonProperty("api_time")]
        public DateTime ApiTime { get; set; }
    }
}