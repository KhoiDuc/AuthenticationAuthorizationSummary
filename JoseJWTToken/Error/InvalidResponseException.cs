using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when the API returns a response which cannot be parsed or understood
    /// </summary>
    [Serializable]
    public class InvalidResponseException : BaseException
    {
        public InvalidResponseException(string message) : base(message)
        {
        }

        public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}