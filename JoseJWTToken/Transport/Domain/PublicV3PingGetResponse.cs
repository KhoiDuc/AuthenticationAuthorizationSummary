using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class PublicV3PingGetResponse
    {
        [JsonProperty("api_time")]
        public DateTime ApiTime { get; set; }
    }
}