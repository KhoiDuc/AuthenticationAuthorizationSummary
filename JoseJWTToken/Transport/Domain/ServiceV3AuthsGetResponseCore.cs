using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServiceV3AuthsGetResponseCore
    {
        [JsonProperty("auth_jwe")]
        public string JweEncryptedDeviceResponse { get; set; }

        [JsonProperty("auth", Required = Required.Always)]
        public string EncryptedDeviceResponse { get; set; }

        [JsonProperty("service_user_hash", Required = Required.Always)]
        public string ServiceUserHash { get; set; }

        [JsonProperty("org_user_hash")]
        public string OrgUserHash { get; set; }

        [JsonProperty("user_push_id", Required = Required.Always)]
        public string UserPushId { get; set; }

        [JsonProperty("public_key_id", Required = Required.Always)]
        public string PublicKeyId { get; set; }
    }
}