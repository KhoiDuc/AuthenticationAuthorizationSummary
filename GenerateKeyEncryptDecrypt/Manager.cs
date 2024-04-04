using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GenerateKeyEncryptDecrypt
{
    class Manager
    {
        public static void Export(RSACryptoServiceProvider rsa, string path)
        {
            RSAParameters publicKey = rsa.ExportParameters(false); // Exporting the public key.
            RSAParameters privateKey = rsa.ExportParameters(true); // Exporting the private key.

            File.WriteAllText(path, // Writing to path.
                Convert.ToBase64String( // Encoding bytes to Base64.
                    Encoding.UTF8.GetBytes( // XML string to bytes.
                        $"{KeyToString(publicKey)}|{KeyToString(privateKey)}"))); // Formatting the public & private key into an XML string.
        }

        public static RSACryptoServiceProvider Import(string path)
        {
            var rsa = new RSACryptoServiceProvider();

            var keys = Encoding.UTF8.GetString( // Bytes to string.
                Convert.FromBase64String( // Decoding from Base64 to bytes.
                    File.ReadAllText(path) // Reading from path.
                    )).Split('|'); // Splitting public & private key.

            rsa.FromXmlString(keys[0]); // Setting the public key into the RSACryptoServiceProvider instance.
            rsa.FromXmlString(keys[1]); // Setting the private key into the RSACryptoServiceProvider instance.
            return rsa;
        }

        public static string KeyToString(RSAParameters key)
        {
            var stringWriter = new StringWriter(); // New instance of string writer.
            var xmlSerializer = new XmlSerializer(typeof(RSAParameters)); // New instance of XML serializer as RSA algorithm type.

            xmlSerializer.Serialize(stringWriter, key); // Formatting key as XML.
            return stringWriter.ToString(); // Covnerting back to a string.
        }

        /// <summary>
        /// Keys the to pem format string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isPublicKey">If true, is public key.</param>
        /// <returns>A string.</returns>
        public static string KeyToPemFormatString(RSAParameters key, bool isPublicKey = true)
        {
            if (isPublicKey == true)
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(key);

                byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                string base64Encoded = Convert.ToBase64String(publicKeyBytes);
                return RsaKeyConverter.FormatPem(base64Encoded, "PUBLIC KEY");
            }
            else
            {
                // Create an RSA instance
                var rsa = new RSACryptoServiceProvider();
                // Import the parameters into the RSA instance
                rsa.ImportParameters(key);
                byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
                string base64Encoded = Convert.ToBase64String(privateKeyBytes);

                return RsaKeyConverter.FormatPem(base64Encoded, "RSA PRIVATE KEY");
            }
        }

        public static RSAParameters GetRsaParametersFromXml(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);

            string exponent = doc.Descendants("Exponent").FirstOrDefault()?.Value;
            string modulus = doc.Descendants("Modulus").FirstOrDefault()?.Value;

            byte[] exponentBytes = Convert.FromBase64String(exponent);
            byte[] modulusBytes = Convert.FromBase64String(modulus);

            RSAParameters rsaParams = new RSAParameters();
            rsaParams.Modulus = modulusBytes;
            rsaParams.Exponent = exponentBytes;

            return rsaParams;
        }

        public static string GetPublicKeyBase64(string xmlString)
        {
            RSAParameters rsaParams = GetRsaParametersFromXml(xmlString);

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);

            byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            string base64Encoded = Convert.ToBase64String(publicKeyBytes);

            return base64Encoded;
        }

        public static string GetPublicKeyBase64(string modulusHex, string exponentHex)
        {
            // Convert hex string to byte array
            byte[] modulusBytes = StringToByteArray(modulusHex);
            byte[] exponentBytes = StringToByteArray(exponentHex);

            // Create RSA key parameters
            RSAParameters rsaParams = new RSAParameters();
            rsaParams.Modulus = modulusBytes;
            rsaParams.Exponent = exponentBytes;

            // Import the public key
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);

            byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();

            // Encode to base64 string
            string base64Encoded = Convert.ToBase64String(publicKeyBytes);

            return base64Encoded;
        }

        public static string GetPemEncodedPrivateKeyFromXml(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);

            string exponent = doc.Descendants("Exponent").FirstOrDefault()?.Value;
            string modulus = doc.Descendants("Modulus").FirstOrDefault()?.Value;
            string d = doc.Descendants("D").FirstOrDefault()?.Value;
            string dp = doc.Descendants("DP").FirstOrDefault()?.Value;
            string dq = doc.Descendants("DQ").FirstOrDefault()?.Value;
            string p = doc.Descendants("P").FirstOrDefault()?.Value;
            string q = doc.Descendants("Q").FirstOrDefault()?.Value;
            string inverseQ = doc.Descendants("InverseQ").FirstOrDefault()?.Value;

            byte[] exponentBytes = Convert.FromBase64String(exponent);
            byte[] modulusBytes = Convert.FromBase64String(modulus);
            byte[] dBytes = Convert.FromBase64String(d);
            byte[] dpBytes = Convert.FromBase64String(dp);
            byte[] dqBytes = Convert.FromBase64String(dq);
            byte[] pBytes = Convert.FromBase64String(p);
            byte[] qBytes = Convert.FromBase64String(q);
            byte[] inverseQBytes = Convert.FromBase64String(inverseQ);

            var privateKeyParams = new RsaPrivateCrtKeyParameters(
                new BigInteger(1, modulusBytes),
                new BigInteger(1, exponentBytes),
                new BigInteger(1, dBytes),
                new BigInteger(1, pBytes),
                new BigInteger(1, qBytes),
                new BigInteger(1, dpBytes),
                new BigInteger(1, dqBytes),
                new BigInteger(1, inverseQBytes));

            // Create an RSA instance
            var rsa = new RSACryptoServiceProvider();
            // Import the parameters into the RSA instance
            rsa.ImportParameters(DotNetUtilities.ToRSAParameters(privateKeyParams));
            byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            // Encode to base64 string
            string base64Encoded = Convert.ToBase64String(privateKeyBytes);

            return base64Encoded;
        }

        private static byte[] StringToByteArray(string hexString)
        {
            int length = hexString.Length;
            byte[] data = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                data[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return data;
        }

        public static string GetPemEncodedPublicKey(string xmlString)
        {
            string base64Encoded = GetPublicKeyBase64(xmlString);

            return RsaKeyConverter.FormatPem(base64Encoded, "PUBLIC KEY");
        }
        public static string GetPemEncodedPrivateKey(string xmlString)
        {
            string base64Encoded = GetPemEncodedPrivateKeyFromXml(xmlString);

            return RsaKeyConverter.FormatPem(base64Encoded, "RSA PRIVATE KEY");
        }
    }
}
