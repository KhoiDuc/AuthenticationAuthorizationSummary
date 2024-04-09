using System;

namespace JoseJWTToken.Error
{
    [Serializable]
    public class JsonEncoderException : BaseException
    {
        public JsonEncoderException(string message) : base(message)
        {
        }

        public JsonEncoderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}