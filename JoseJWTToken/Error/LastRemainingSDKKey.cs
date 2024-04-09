using System;

namespace JoseJWTToken.Error
{
    public class LastRemainingSDKKey : InvalidRequestException
    {
        public LastRemainingSDKKey(string message) : base(message)
        {
        }

        public LastRemainingSDKKey(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LastRemainingSDKKey(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}