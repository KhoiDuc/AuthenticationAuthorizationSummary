using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class DirectoryV3TotpPostRequest
    {
        public DirectoryV3TotpPostRequest(string identifier)
        {
            Identifier = identifier;
        }
        
        [JsonProperty("identifier")]
        public string Identifier { get; }
    }
}