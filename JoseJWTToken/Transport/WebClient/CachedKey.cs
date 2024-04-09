using System.Security.Cryptography;

namespace JoseJWTToken.Transport.WebClient
{
    public class CachedKey
    {
        public string Thumbprint { get; }
        public RSA KeyData { get; }

        public CachedKey(string thumbprint, RSA keyData)
        {
            Thumbprint = thumbprint;
            KeyData = keyData;
        }
    }
}