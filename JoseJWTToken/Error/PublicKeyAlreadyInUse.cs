using System;

namespace JoseJWTToken.Error
{
    public class PublicKeyAlreadyInUse : InvalidRequestException
    {
        public PublicKeyAlreadyInUse(string message) : base(message)
        {
        }

        public PublicKeyAlreadyInUse(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PublicKeyAlreadyInUse(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}