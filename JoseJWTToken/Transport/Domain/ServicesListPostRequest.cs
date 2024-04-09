using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicesListPostRequest
    {
        [JsonProperty("service_ids")]
        public List<Guid> ServiceIds { get; }

        public ServicesListPostRequest(List<Guid> serviceIds)
        {
            ServiceIds = serviceIds;
        }
    }
}