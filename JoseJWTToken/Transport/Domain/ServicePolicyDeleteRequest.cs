using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicePolicyDeleteRequest
    {
        [JsonProperty("service_id")]
        public Guid ServiceId { get; }

        public ServicePolicyDeleteRequest(Guid serviceId)
        {
            ServiceId = serviceId;
        }
    }
}