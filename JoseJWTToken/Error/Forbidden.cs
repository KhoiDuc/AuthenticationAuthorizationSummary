using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when an HTTP 403 occurs.
    /// </summary>
    [Serializable]
    public class Forbidden : CommunicationErrorException
    {
        public Forbidden(string message) : base(message)
        {
        }
    }
}