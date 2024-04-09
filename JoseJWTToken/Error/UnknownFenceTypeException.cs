using System;
namespace JoseJWTToken.Error
{
    public class UnknownFenceTypeException : BaseException
    {
        public UnknownFenceTypeException(string message) : base(message)
        {
        }

        public UnknownFenceTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnknownFenceTypeException(string message, Exception innerException, string errorCode) : base(message, innerException, errorCode)
        {
        }
    }
}
