using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServiceKeysListPostRequest
    {
        [JsonProperty("service_id")]
        public Guid ServiceId { get; }

        public ServiceKeysListPostRequest(Guid serviceId)
        {
            ServiceId = serviceId;
        }
    }
}