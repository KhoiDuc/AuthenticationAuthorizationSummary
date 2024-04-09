using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicePolicyPutRequest
    {
        [JsonProperty("service_id")]
        public Guid ServiceId { get; }

        [JsonProperty("policy")]
        public IPolicy Policy { get; }

        public ServicePolicyPutRequest(Guid serviceId, IPolicy policy)
        {
            ServiceId = serviceId;
            Policy = policy;
        }
    }
}