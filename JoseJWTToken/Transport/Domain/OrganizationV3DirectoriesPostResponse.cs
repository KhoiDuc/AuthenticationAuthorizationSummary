using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectoriesPostResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}