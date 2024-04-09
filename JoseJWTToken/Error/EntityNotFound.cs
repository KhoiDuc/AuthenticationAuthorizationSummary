using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when an HTTP 404 has occurred.
    /// </summary>
    [Serializable]
    public class EntityNotFound : CommunicationErrorException
    {
        public EntityNotFound(string message) : base(message)
        {
        }
    }
}