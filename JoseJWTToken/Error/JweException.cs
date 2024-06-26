﻿using System;

namespace JoseJWTToken.Error
{
    /// <summary>
    /// Thrown when a JWE-related error occurs, including cryptographic problems such as signature errors or other security checks.
    /// </summary>
    [Serializable]
    public class JweException : BaseException
    {
        public JweException(string message) : base(message)
        {
        }

        public JweException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}