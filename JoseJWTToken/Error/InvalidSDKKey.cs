using System;

namespace JoseJWTToken.Error
{
    public class InvalidSDKKey : InvalidRequestException
    {
        public InvalidSDKKey(string message) : base(message)
        {
        }

        public InvalidSDKKey(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidSDKKey(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}