using System;

namespace JoseJWTToken.Error
{
    public class InvalidPublicKey : InvalidRequestException
    {
        public InvalidPublicKey(string message) : base(message)
        {
        }

        public InvalidPublicKey(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidPublicKey(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}