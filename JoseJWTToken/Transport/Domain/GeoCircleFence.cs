using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class GeoCircleFence : IFence
    {
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }

        public GeoCircleFence(string name, double latitude, double longitude,
            double radius)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude; 
            Radius = radius;
            Type = "GEO_CIRCLE";
        }

        public JoseJWTToken.Domain.Service.Policy.IFence FromTransport()
        {
            return new JoseJWTToken.Domain.Service.Policy.GeoCircleFence(
                latitude: Latitude,
                longitude: Longitude,
                radius: Radius,
                name: Name
            );
        }
    }
}