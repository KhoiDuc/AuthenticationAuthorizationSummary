using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectorySdkKeysPostResponse
    {
        [JsonProperty("sdk_key")]
        public Guid SdkKey { get; set; }
    }
}