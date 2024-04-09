using JoseJWTToken.Error;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JoseJWTToken.Json
{
    public class JsonNetJsonEncoder : IJsonEncoder
    {
        public TResult DecodeObject<TResult>(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<TResult>(data);
            }
            catch (JsonException ex)
            {
                throw new JsonEncoderException("Error deserializing response", ex);
            }
        }

        public string EncodeObject(object obj)
        {
            try
            {
                if (obj == null) return null;
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                settings.NullValueHandling = NullValueHandling.Ignore;
                var encoded = JsonConvert.SerializeObject(obj, settings);
                return encoded;
            }
            catch (JsonException ex)
            {
                throw new JsonEncoderException("Error serializing response", ex);
            }
        }
    }
}