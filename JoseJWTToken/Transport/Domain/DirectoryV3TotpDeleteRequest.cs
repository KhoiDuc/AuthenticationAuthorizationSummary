using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class DirectoryV3TotpDeleteRequest
    {
        
        public DirectoryV3TotpDeleteRequest(string identifier)
        {
            Identifier = identifier;
        }
        
        [JsonProperty("identifier")]
        public string Identifier { get; }

    }
}