using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GenerateKeyEncryptDecryptNet4.helper
{
    public class RSA
    {
        private static void Print(string text)
        {
            Console.WriteLine(String.Format("\n[{0}] {1}", DateTime.Now.ToString(), text));
        }

        public class KeysCouple
        {
            public string PrivateKey { get; set; }
            public string PublicKey { get; set; }

            public KeysCouple(string privateKey, string publicKey)
            {
                this.PrivateKey = privateKey;
                this.PublicKey = publicKey;
            }
        }
        public static RSACryptoServiceProvider CreateRSAKey(int keySize, bool signature = true)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = Guid.NewGuid().ToString();
            // Must provide these details otherwise the generated RSA key will not work very well (if at all)
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            cspParams.ProviderType = 1;

            cspParams.Flags = CspProviderFlags.NoPrompt;
            if (signature)
            {
                cspParams.KeyNumber = (int)KeyNumber.Signature;
            }
            else
            {
                cspParams.KeyNumber = (int)KeyNumber.Exchange;
            }

            return new RSACryptoServiceProvider(keySize, cspParams);
        }
        public static RSACryptoServiceProvider CreateRSAKey1(int keySize, bool signature = true)
        {
            // Generate a signing key.
            CspParameters csp = new CspParameters(1, "DummyContainer");
            csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
            csp.KeyNumber = (int)KeyNumber.Signature;

            if (!String.IsNullOrEmpty("password"))
            {
                csp.KeyPassword = new SecureString();
                foreach (var ch in "password".ToCharArray())
                {
                    csp.KeyPassword.AppendChar(ch);
                }
            }

            RSACryptoServiceProvider Key = new RSACryptoServiceProvider(csp);

            return new RSACryptoServiceProvider(keySize, csp);
        }
        public static KeysCouple Keys(string publicKeyFileName, string privateKeyFileName, string password, int keySize = 4096)
        {
            CspParameters cspParams = null;
            string publicKey = "";
            string privateKey = "";

            cspParams = new CspParameters();
            

            cspParams.ProviderType = 1;
            cspParams.KeyContainerName = "DummyContainer";
            cspParams.ProviderName = "";

            //* Convert password into secure string.
            //SecureString keyPassword = new SecureString();
            //foreach (char c in password)
            //{
            //    keyPassword.AppendChar(c);
            //}
            string passphrase = "password";
            char[] passPhrase = passphrase.ToCharArray();
            SecureString keyPassword = new SecureString();

            for (int i = 0; i < passPhrase.Length; i++)
            {
                keyPassword.AppendChar(passPhrase[i]);
            }
                
            cspParams.KeyPassword = keyPassword;
            //cspParams.Flags = CspProviderFlags.UseArchivableKey | CspProviderFlags.NoFlags;
            cspParams.Flags = CspProviderFlags.UseDefaultKeyContainer;
            cspParams.KeyNumber = (int)KeyNumber.Signature;

            var rsaProvider = new RSACryptoServiceProvider(keySize, cspParams);
            {
                try
                {
                    publicKey = rsaProvider.ToXmlString(false);
                    privateKey = rsaProvider.ToXmlString(true);
                }
                catch (Exception ex)
                {
                    Print(String.Format("[GENERATING KEYS] Exception: {0}", ex.Message));
                }
                finally
                {
                    rsaProvider.PersistKeyInCsp = false;
                    rsaProvider.Dispose();
                    keyPassword.Dispose();
                }
            }

            return new KeysCouple(privateKey, publicKey);
        }

        public static string Sign(string message, string privateKey)
        {
            byte[] signedBytes = null;

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                byte[] originalData = new UTF8Encoding().GetBytes(message);

                try
                {
                    rsaProvider.FromXmlString(privateKey);
                    signedBytes = rsaProvider.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (Exception ex)
                {
                    Print(String.Format("[SIGN] Exception: {0}", ex.Message));
                }
                finally
                {
                    rsaProvider.PersistKeyInCsp = false;
                    rsaProvider.Dispose();
                }
            }

            return Convert.ToBase64String(signedBytes);
        }

        public static bool Verify(string originalMessage, string signedMessage, string publicKey)
        {
            bool success = false;

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                byte[] bytesToVerify = new UTF8Encoding().GetBytes(originalMessage);
                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    rsaProvider.FromXmlString(publicKey);
                    success = rsaProvider.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
                }
                catch (Exception ex)
                {
                    Print(String.Format("[VERIFY] Exception: {0}", ex.Message));
                }
                finally
                {
                    rsaProvider.PersistKeyInCsp = false;
                    rsaProvider.Dispose();
                }
            }

            return success;
        }

        public static string Encrypt(string publicKey, string textToEncrypt)
        {
            byte[] plainBytes = null;
            byte[] encryptedBytes = null;
            string encryptedText = "";

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                try
                {
                    rsaProvider.FromXmlString(publicKey);

                    plainBytes = Encoding.Unicode.GetBytes(textToEncrypt);
                    encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
                    encryptedText = Convert.ToBase64String(encryptedBytes);
                }
                catch (Exception ex)
                {
                    Print(String.Format("[ENCRYPT] Exception: {0}", ex.Message));
                }
                finally
                {
                    rsaProvider.PersistKeyInCsp = false;
                    rsaProvider.Dispose();
                }
            }

            return encryptedText;
        }

        public static string Decrypt(string privateKey, string encryptedTextBase64)
        {
            CspParameters cspParams = null;
            string decryptedText = "";
            byte[] plainBytes = null;

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                try
                {
                    rsaProvider.FromXmlString(privateKey);

                    plainBytes = rsaProvider.Decrypt(Convert.FromBase64String(encryptedTextBase64), false);
                    decryptedText = Encoding.Unicode.GetString(plainBytes);

                }
                catch (Exception ex)
                {
                    Print(String.Format("[DECRYPT] Exception: {0}", ex.Message));
                }
                finally
                {
                    rsaProvider.PersistKeyInCsp = false;
                    rsaProvider.Dispose();
                }
            }

            return decryptedText;
        }
    }
}
