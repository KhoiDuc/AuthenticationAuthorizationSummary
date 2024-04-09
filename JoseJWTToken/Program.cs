using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JoseJWTToken
{
    internal class Program
    {
        private const int SECONDS_EXPIRY = 3600;

        private static readonly Org.BouncyCastle.Asn1.X9.X9ECParameters curve = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256r1");
        private static readonly Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        public static string GetPublicKey(string privKey)
        {
            byte[] GetBytesFromPEM(string pemString, string section)
            {
                var header = String.Format("-----BEGIN {0}-----", section);
                var footer = String.Format("-----END {0}-----", section);

                var start = pemString.IndexOf(header, StringComparison.Ordinal);
                if (start < 0)
                    return null;

                start += header.Length;
                var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

                if (end < 0)
                    return null;

                return Convert.FromBase64String(pemString.Substring(start, end));
            }

            Org.BouncyCastle.Math.BigInteger d = new Org.BouncyCastle.Math.BigInteger(GetBytesFromPEM(privKey, "PRIVATE KEY"));
            //var privKeyParameters = new Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters(d, domain);
            Org.BouncyCastle.Math.EC.ECPoint q = domain.G.Multiply(d);
            //var pubKeyParameters = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domain);
            return Convert.ToBase64String(q.GetEncoded());
        }

        public static void GenRsaPrivatePublicKey()
        {
            var rand = new SecureRandom();
            var keyGenParams = new RsaKeyGenerationParameters(new BigInteger("65537"), rand, 1024, 64);
            var rsaKeyGen = new RsaKeyPairGenerator();
            rsaKeyGen.Init(keyGenParams);
            var rsaKeyPair = rsaKeyGen.GenerateKeyPair();
            var rsaPriv = (RsaPrivateCrtKeyParameters)rsaKeyPair.Private;

            // Make a public from the private
            var rsaPub = new RsaKeyParameters(false, rsaPriv.Modulus, rsaPriv.PublicExponent);
        }

        static void Main(string[] args)
        {
            var privateKey = ConfigurationManager.AppSettings["privateKey"];
            var publicKey = ConfigurationManager.AppSettings["publicKey"];
            var appId = ConfigurationManager.AppSettings["APP_ID"];
            var claims = GetClaimsList(appId);
            var token = TokenGenerator.GenerateToken(claims, privateKey);
            Console.WriteLine(token.ToString());
            var tokendecode = TokenGenerator.DecodeToken(token, publicKey);
            Console.WriteLine(tokendecode.ToString());
        }

        private static string ConvertEscapedUnicodeCharecters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{8})",
                code => {
                    var theValue = int.Parse(code.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber);
                    var ret = char.ConvertFromUtf32(theValue);
                    return ret.ToString();
                });
        }

        private static List<Claim> GetClaimsList(string appId)
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var iat = new Claim("iat", ((Int32)t.TotalSeconds).ToString(), ClaimValueTypes.Integer32); // Unix Timestamp for right now
            var application_id = new Claim("application_id", appId); // Current app ID
            var exp = new Claim("exp", ((Int32)(t.TotalSeconds + SECONDS_EXPIRY)).ToString(), ClaimValueTypes.Integer32); // Unix timestamp for when the token expires
            var jti = new Claim("jti", Guid.NewGuid().ToString()); // Unique Token ID
            var claims = new List<Claim>() { iat, application_id, exp, jti };

            return claims;
        }
    }
}
