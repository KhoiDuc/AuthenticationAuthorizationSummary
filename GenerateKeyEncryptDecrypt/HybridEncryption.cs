// Wihler Ruben
// 05.12.2023
using System.Security.Cryptography;

namespace GenerateKeyEncryptDecrypt
{

    //Resources:
    //Doc :
    //  Symmetric encryption (AES): https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-5.0
    //  Asymmetric encryption (RSA): https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data#asymmetric-encryption
    //Other:
    //AES | initialization vector (IV) : https://www.techtarget.com/whatis/definition/initialization-vector-IV
    public static class HybridEncryption
    {
        /// <summary>
        /// Generate a private/public RSA key and return it in XML string
        /// </summary>
        /// <returns></returns>
        public static (string publicKey, string privateKey) GenerateRSAKeys()
        {
            var rsa = new RSACryptoServiceProvider()
            {
                KeySize = 1024
            };

            var private_key = rsa.ToXmlString(true);
            var public_key = rsa.ToXmlString(false);

            return (public_key, private_key);
        }

        /// <summary>
        /// Encrypt a text with a combinaison of RSA and AES.
        /// We will generate AES key and IV for encrypt the text.
        /// The AES key and IV will be encrypted with the entered RSA public key.
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="publicKey">the target RSA public key in XML format</param>
        /// <returns></returns>
        public static (string text, string aesKey, string aesIV) Encrypt(string text, string publicKey)
        {
            (var key, var iv) = GenerateAESKey();

            var keyb64 = Convert.ToBase64String(key);
            var ivb64 = Convert.ToBase64String(iv);

            var encrypted_msg = AesEncrypt(text, key, iv);
            var encrypted_key = RsaEncrypt(keyb64, publicKey);
            var encrypted_iv = RsaEncrypt(ivb64, publicKey);

            return (Convert.ToBase64String(encrypted_msg),
                Convert.ToBase64String(encrypted_key),
                Convert.ToBase64String(encrypted_iv));
        }
        /// <summary>
        /// Decrypt the target encrypted text.
        /// It will decrypt the AES key and IV with the private RSA key.
        /// With the founded AES key and IV we will decrypt the text.
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesIV"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string aesKey, string aesIV, string privateKey)
        {
            var textbyte = Convert.FromBase64String(encryptedText);

            var decrypted_key = RsaDecrypt(aesKey, privateKey);
            var decrypted_IV = RsaDecrypt(aesIV, privateKey);
            var decrypted_text = AesDecrypt(textbyte, decrypted_key, decrypted_IV);

            return decrypted_text;
        }

        private static byte[] RsaEncrypt(string data, string publicKey)
        {
            var data_b64 = Convert.FromBase64String(data);

            using var rsa = new RSACryptoServiceProvider(1024);
            try
            {
                rsa.FromXmlString(publicKey.ToString());
                return rsa.Encrypt(data_b64, true);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
        private static byte[] RsaDecrypt(string data, string privateKey)
        {
            var data_b64 = Convert.FromBase64String(data);

            using var rsa = new RSACryptoServiceProvider(1024);
            try
            {
                rsa.FromXmlString(privateKey);
                return rsa.Decrypt(data_b64, true);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        private static byte[] AesEncrypt(string data, byte[] key, byte[] IV)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = IV;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted_data;

            using (MemoryStream ms = new MemoryStream())
            {
                using CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }

                encrypted_data = ms.ToArray();
            }

            return encrypted_data;
        }
        private static string AesDecrypt(byte[] cipherText, byte[] key, byte[] IV)
        {
            string data = string.Empty;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using var ms = new MemoryStream(cipherText);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                data = sr.ReadToEnd();
            }

            return data;
        }

        private static (byte[] key, byte[] IV) GenerateAESKey()
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            return (aes.Key, aes.IV);
        }
    }
}