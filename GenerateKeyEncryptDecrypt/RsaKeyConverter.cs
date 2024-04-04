using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace GenerateKeyEncryptDecrypt;

public static class RsaKeyConverter
{
    public static string XmlToPem(string xml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(xml);

        AsymmetricCipherKeyPair? keyPair = rsa.GetKeyPair(); // try get private and public key pair
        if (keyPair != null) // if XML RSA key contains private key
        {
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            return FormatPem(privateKeyInfo.GetEncoded().ToBase64(), "RSA PRIVATE KEY");
        }

        RsaKeyParameters? publicKey = rsa.GetPublicKey(); // try get public key
        if (publicKey is null) throw new InvalidKeyException("Invalid RSA Xml Key");

        SubjectPublicKeyInfo publicKeyInfo =
            SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
        return FormatPem(publicKeyInfo.GetEncoded().ToBase64(), "PUBLIC KEY");
    }

    private static RSAParameters GetRsaParameters(string rsaPrivateKey)
    {
        using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(rsaPrivateKey)))
        using (var sr = new StreamReader(ms))
        {
            var pemReader = new PemReader(sr);
            var pem = pemReader.ReadPemObject();
            var privateKey = PrivateKeyFactory.CreateKey(pem.Content);
            return DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)privateKey);
        }
    }

    public static string GetRsaParametersAsXml(string rsaPrivateKey)
    {
        try
        {
            // Extract RSA parameters from the PEM-formatted key string
            RSAParameters rsaParams = GetRsaParameters(rsaPrivateKey);

            // Serialize RSA parameters to XML
            XmlSerializer serializer = new XmlSerializer(typeof(RSAParameters));
            StringBuilder xmlString = new StringBuilder();
            using (StringWriter writer = new StringWriter(xmlString))
            {
                serializer.Serialize(writer, rsaParams);
            }

            return xmlString.ToString();
        }
        catch (Exception ex)
        {
            // Handle potential errors gracefully
            throw new Exception("Failed to convert RSA parameters to XML", ex);
        }
    }

    public static string PemToXml(string pem, bool newSolution = true)
    {
        if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
            || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
        {
            if(newSolution)
            {
                //return GetRsaParametersAsXml(pem);
                var a = GetRsaParameters(pem);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(a);
                return rsa.ToXmlString(true);
            }
            return GetXmlRsaKey(pem, obj =>
            {
                if ((obj as RsaPrivateCrtKeyParameters) != null)
                    return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)obj);
                var keyPair = (AsymmetricCipherKeyPair)obj;
                return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private);
            }, rsa => rsa.ToXmlString(true));
        }

        if (pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
        {
            return GetXmlRsaKey(pem, obj =>
            {
                var publicKey = (RsaKeyParameters)obj;
                return DotNetUtilities.ToRSA(publicKey);
            }, rsa => rsa.ToXmlString(false));
        }

        throw new InvalidKeyException("Unsupported PEM format...");
    }

    private static string GetXmlRsaKey(string pem, Func<object, RSA> getRsa, Func<RSA, string> getKey)
    {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms);
        using var sr = new StreamReader(ms);
        sw.Write(pem);
        sw.Flush();
        ms.Position = 0;
        var pr = new PemReader(sr);
        object keyPair = pr.ReadObject();

        using RSA rsa = getRsa(keyPair);
        string xml = getKey(rsa);
        return xml;
    }

    public static string FormatPem(string pem, string keyType)
    {
        var sb = new StringBuilder();
        sb.Append($"-----BEGIN {keyType}-----\n");

        var line = 1;
        const int width = 64;

        while ((line - 1) * width < pem.Length)
        {
            int startIndex = (line - 1) * width;
            int len = line * width > pem.Length
                ? pem.Length - startIndex
                : width;
            sb.Append($"{pem.Substring(startIndex, len)}\n");
            line++;
        }

        sb.Append($"-----END {keyType}-----\n");
        return sb.ToString();
    }
}
