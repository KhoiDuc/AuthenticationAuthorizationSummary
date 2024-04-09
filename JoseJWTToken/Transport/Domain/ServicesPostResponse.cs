using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicesPostResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}