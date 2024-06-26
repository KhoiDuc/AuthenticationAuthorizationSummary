using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class DirectoryV3DevicesPostResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("qrcode")]
        public string QrCode { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

    }
}