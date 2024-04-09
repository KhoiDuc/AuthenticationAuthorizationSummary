using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectorySdkKeysListPostRequest
    {
        [JsonProperty("directory_id")]
        public Guid DirectoryId { get; }

        public OrganizationV3DirectorySdkKeysListPostRequest(Guid directoryId)
        {
            DirectoryId = directoryId;
        }
    }
}