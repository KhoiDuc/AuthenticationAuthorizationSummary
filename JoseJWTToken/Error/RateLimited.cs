using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when the API has begun rejecting our requests due to excessive hits
    /// </summary>
    [Serializable]
    public class RateLimited : CommunicationErrorException
    {
        public RateLimited(string message) : base(message)
        {
        }
    }
}