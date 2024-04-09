using System;

namespace JoseJWTToken.Error
{
    public class InvalidDirectoryIdentifier : InvalidRequestException
    {
        public InvalidDirectoryIdentifier(string message) : base(message)
        {
        }

        public InvalidDirectoryIdentifier(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidDirectoryIdentifier(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}