using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class KeysPostResponse
    {
        [JsonProperty("key_id")]
        public string Id { get; set; }
    }
}