using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenerateKeyEncryptDecrypt
{
    class RSACustom
    {
        public static string Encrypt(string textToEncrypt, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            { // New instance of RSACryptoServiceProvider.
                try
                {
                    rsa.FromXmlString(publicKey); // XML string to public key.
                    var bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt); // Converting string to encrypt to bytes.
                    var encryptedData = rsa.Encrypt(bytesToEncrypt, true); // Encrypting bytes.
                    var base64Encrypted = Convert.ToBase64String(encryptedData); // Encoding encrypted bytes to Base64.
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string Decrypt(string textToDecrypt, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            { // New instance of RSACryptoServiceProvider.
                try
                {
                    rsa.FromXmlString(privateKey); // XML string to private key.
                    var resultBytes = Convert.FromBase64String(textToDecrypt); // Decoding Base64 string to bytes.
                    var decryptedBytes = rsa.Decrypt(resultBytes, true); // Decryting bytes.
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes); // Converting decrypted bytes to string.
                    return decryptedData;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
