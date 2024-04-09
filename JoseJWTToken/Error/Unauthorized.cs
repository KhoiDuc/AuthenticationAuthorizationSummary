using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when a 403 occurs
    /// </summary>
    [Serializable]
    public class Unauthorized : CommunicationErrorException
    {
        public Unauthorized(string message) : base(message)
        {
        }
    }
}