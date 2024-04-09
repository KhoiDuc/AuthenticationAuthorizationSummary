using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServiceV3TotpPostResponse
    {
        public ServiceV3TotpPostResponse(bool valid)
        {
            Valid = valid;
        }
        
        [JsonProperty("valid")]
        public bool Valid { get; }
    }
}