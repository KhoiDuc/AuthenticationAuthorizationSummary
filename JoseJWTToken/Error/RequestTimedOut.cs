using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when a timeout (408) occurs unexpectedly
    /// </summary>
    [Serializable]
    public class RequestTimedOut : CommunicationErrorException
    {
        public RequestTimedOut(string message) : base(message)
        {
        }
    }
}