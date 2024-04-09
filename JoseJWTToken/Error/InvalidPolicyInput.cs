using System;

namespace JoseJWTToken.Error
{
    public class InvalidPolicyInput : InvalidRequestException
    {
        public InvalidPolicyInput(string message) : base(message)
        {
        }

        public InvalidPolicyInput(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidPolicyInput(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}