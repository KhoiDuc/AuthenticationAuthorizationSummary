using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectoriesPostRequest
    {
        [JsonProperty("name")]
        public string Name { get; }

        public OrganizationV3DirectoriesPostRequest(string name)
        {
            Name = name;
        }
    }
}