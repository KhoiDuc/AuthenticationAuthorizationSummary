using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Error when performing caching functions
    /// </summary>
    [Serializable]
    public class CacheException : BaseException
    {
        public CacheException(string message) : base(message)
        {
        }

        public CacheException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}