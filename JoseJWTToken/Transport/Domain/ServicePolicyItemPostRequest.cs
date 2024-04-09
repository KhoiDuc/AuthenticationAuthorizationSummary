using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicePolicyItemPostRequest
    {
        [JsonProperty("service_id")]
        public Guid ServiceId { get; }

        public ServicePolicyItemPostRequest(Guid serviceId)
        {
            ServiceId = serviceId;
        }
    }
}