/// <summary>
/// Demo C# application for: 
/// 	- Private/Public RSA Key Generation. Encryption/Decryption and Signing/Verification for messages using those keys.
///     - X509 Certificate generation.
/// 	- PDF document signing.
/// </summary>
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Reflection;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using System.Collections.Generic;
using Jose;

namespace GenerateKeyEncryptDecryptNet4
{
	class Program
	{
		static readonly string SAMPLE_TEXT = "O2OSYS";
		static readonly byte[] SAMPLE_DATA = Encoding.UTF8.GetBytes(SAMPLE_TEXT);
		static readonly int RSA_KEY_BIT_SIZE = 2048;

		static void Main(string[] args)
		{
			// testRSA();
			// testPemXML(args);
			testSimpleGetJWTFromPrivateKey();

		}

		private static void testSimpleGetJWTFromPrivateKey()
		{
			//slorello89
			string jwt = getJWT("218xyz8c-99ec-4xyz2-b2df-d2xyzxyzxb91", "PrivateKeyOld.pem");
			Console.WriteLine(jwt);
		}
		internal static string getJWT(string appid, string privatekeyfile)
		{
			var tokenData = new byte[64];
			var rng = RandomNumberGenerator.Create();
			rng.GetBytes(tokenData);
			var jwtTokenId = Convert.ToBase64String(tokenData);
			var payload = new Dictionary<string, object>
			{
				{ "iat", (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds },
				{ "application_id", appid },
				{ "jti", jwtTokenId }
			};
			string privateKeyString = System.IO.File.ReadAllText(privatekeyfile);
			var rsa = PemParse.DecodePEMKey(privateKeyString);
			var jwtToken = Jose.JWT.Encode(payload, rsa, JwsAlgorithm.RS256);
			return jwtToken;
		}

		private static void testPemXML(string[] args)
		{
			if (args.Length > 0)
			{
				if (args[0].EndsWith(".xml", StringComparison.OrdinalIgnoreCase) && File.Exists(args[0]))
				{
					Console.WriteLine(XmlToPem(File.ReadAllText(args[0])));
				}
				else if (args[0].EndsWith(".pem", StringComparison.OrdinalIgnoreCase) && File.Exists(args[0]))
				{
					Console.WriteLine(PemToXml(File.ReadAllText(args[0])));
				}
				else
				{
					Console.WriteLine("Usage: XmlPemConverter key.pem or XmlPemCoverter key.xml");
				}
			}
			else
			{
				Console.WriteLine("Usage: XmlPemConverter key.pem or XmlPemCoverter key.xml");
			}
		}
		static string XmlToPem(string xml)
		{
			using (var rsa = RSA.Create())
			{
				rsa.FromXmlString(xml);

				try
				{
					var keyPair = DotNetUtilities.GetRsaKeyPair(rsa);
					if (keyPair != null)
					{
						PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
						return FormatPem(Convert.ToBase64String(privateKeyInfo.GetEncoded()), "RSA PRIVATE KEY");
					}
				} catch {
				}

				var publicKey = DotNetUtilities.GetRsaPublicKey(rsa);
				if (publicKey is null)
				{
					throw new InvalidKeyException("Invalid RSA XML Key.");
				}

				var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);

				return FormatPem(Convert.ToBase64String(publicKeyInfo.GetEncoded()), "PUBLIC KEY");
			}
		}

		static string PemToXml(String pem)
		{
			if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----") || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
			{
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

			throw new InvalidKeyException("Unsupported PEM format.");
		}

		static string GetXmlRsaKey(string pem, Func<object, RSA> getRsa, Func<RSA, string> getKey)
		{
			using (var ms = new MemoryStream())
			{
				using (var sw = new StreamWriter(ms))
				{
					using (var sr = new StreamReader(ms))
					{
						sw.Write(pem);
						sw.Flush();
						ms.Position = 0;
						var pr = new PemReader(sr);
						object keyPair = pr.ReadObject();

						using (RSA rsa = getRsa(keyPair))
						{
							var xml = getKey(rsa);
							return xml;
						}
					}
				}
			}
		}

		/// <summary>
		/// Formats the pem.
		/// </summary>
		/// <param name="pem">The pem.</param>
		/// <param name="keyType">The key type.</param>
		/// <returns>A string.</returns>
		static string FormatPem(String pem, String keyType)
		{
			var sb = new StringBuilder();
			sb.Append($"-----BEGIN {keyType}-----\n");

			var line = 1;
			var width = 64;

			while ((line - 1) * width < pem.Length)
			{
				Int32 startIndex = (line - 1) * width;
				Int32 len = line * width > pem.Length ? pem.Length - startIndex : width;
				sb.Append($"{pem.Substring(startIndex, len)}\n");
				line++;
			}

			sb.Append($"-----END {keyType}-----\n");

			return sb.ToString();
		}

		private static void testRSA()
		{
			//
			// 키 로드 또는 생성
			//
			byte[] privateKeyData = null;
			byte[] publicKeyData = null;
			string privateKeyFile = Path.Combine(GetKeyDirectory(), "cs.rsa.private.key");
			string publicKeyFile = Path.Combine(GetKeyDirectory(), "cs.rsa.public.key");
			if (File.Exists(privateKeyFile) && File.Exists(publicKeyFile))
			{
				// 키 파일이 있으면 로드
				privateKeyData = Convert.FromBase64String(File.ReadAllText(privateKeyFile));
				publicKeyData = Convert.FromBase64String(File.ReadAllText(publicKeyFile));
			}
			else
			{
				//
				// 키 파일이 없으면 생성
				//
				var keyGen = new RsaKeyPairGenerator();
				keyGen.Init(new KeyGenerationParameters(new SecureRandom(), RSA_KEY_BIT_SIZE));
				var keyPair = keyGen.GenerateKeyPair();

				// 개인키 데이터 저장
				var privateKeyParam = keyPair.Private as RsaKeyParameters;
				var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
				privateKeyData = privateKeyInfo.GetDerEncoded();
				File.WriteAllText(privateKeyFile, Convert.ToBase64String(privateKeyData));

				// 공개키 데이터 저장
				var publicKeyParam = keyPair.Public as RsaKeyParameters;
				var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKeyParam);
				publicKeyData = publicKeyInfo.GetDerEncoded();
				File.WriteAllText(publicKeyFile, Convert.ToBase64String(publicKeyData));
			}

			//
			// 공개키로 암호화
			//
			byte[] encryptedData = Encrypt(SAMPLE_DATA, publicKeyData);
			string encryptedFile = Path.Combine(GetKeyDirectory(), "cs.rsa.data");
			File.WriteAllText(encryptedFile, Convert.ToBase64String(encryptedData));

			//
			// 개인키로 복호화
			//
			byte[] decryptedData = Decrypt(encryptedData, privateKeyData);

			//
			// 결과 (암/복호화)
			//
			Console.WriteLine($"---------- RSA, Key: C#, Apply: C# ----------");
			Console.WriteLine($"Original Data: {Convert.ToBase64String(SAMPLE_DATA)}");
			Console.WriteLine($"Original Data: {Convert.ToBase64String(decryptedData)}");

			//
			// Signing
			//
			Console.WriteLine($"---------- RSA Signing/Verification, Key: C#, Apply: C# ----------");
			var hasher = HashAlgorithm.Create("SHA384");
			byte[] signature = CreatePrivateRSA(privateKeyData).SignData(SAMPLE_DATA, hasher);
			Console.WriteLine($"Signature Size: {signature.Length}");

			//
			// Verification
			//
			bool verified = CreatePublicRSA(publicKeyData).VerifyData(SAMPLE_DATA, hasher, signature);

			//
			// 결과 (Signing/Verification)
			//
			Console.WriteLine($"Original Data Verification: {verified}");

			// 데이터를 살짝 변경
			byte[] fakeSampleData = new byte[SAMPLE_DATA.Length];
			Array.Copy(SAMPLE_DATA, 0, fakeSampleData, 0, fakeSampleData.Length);
			fakeSampleData[5] += 1;

			//
			// Verification one more time
			// 
			verified = CreatePublicRSA(publicKeyData).VerifyData(fakeSampleData, hasher, signature);
			Console.WriteLine($"---------- RSA Signing/Verification, Key: C#, Apply: C# ----------");
			Console.WriteLine($"Fake Data Verification: {verified}");

			// 자바에서 만든 암호화 파일이 있으면 테스트
			/*
			string javaEncryptedFile = Path.Combine(GetKeyDirectory(), "cs.rsa.data.java");
			if (File.Exists(javaEncryptedFile))
			{
				byte[] encryptedData = Convert.FromBase64String(File.ReadAllText(javaEncryptedFile));

				var decryptor = new RSACryptoServiceProvider();
				decryptor.ImportCspBlob(privateKeyData);
				byte[] decryptedData = decryptor.Decrypt(encryptedData, false);

				Console.WriteLine($"---------- Key: C#, Encryption: Java ----------");
				Console.WriteLine($"Original Data: {Convert.ToBase64String(SAMPLE_DATA)}");
				Console.WriteLine($"Original Data: {Convert.ToBase64String(decryptedData)}");
			}
			*/
		}

		private static byte[] Encrypt(byte[] data, byte[] publicKeyData)
		{
			return CreatePublicRSA(publicKeyData).Encrypt(data, false);
		}

		private static byte[] Decrypt(byte[] data, byte[] privateKeyData)
		{
			return CreatePrivateRSA(privateKeyData).Decrypt(data, false);
		}

		private static RSACryptoServiceProvider CreatePublicRSA(byte[] publicKeyData)
		{
			var bcPublicKeyParam = PublicKeyFactory.CreateKey(publicKeyData) as RsaKeyParameters;
			var rsaPublicKeyParam = DotNetUtilities.ToRSAParameters(bcPublicKeyParam);
			var rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(rsaPublicKeyParam);
			return rsa;
		}

		private static RSACryptoServiceProvider CreatePrivateRSA(byte[] privateKeyData)
		{
			var bcPrivateKeyParam = PrivateKeyFactory.CreateKey(privateKeyData) as RsaPrivateCrtKeyParameters;
			var rsaPrivateKeyParam = DotNetUtilities.ToRSAParameters(bcPrivateKeyParam);
			var rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(rsaPrivateKeyParam);
			return rsa;
		}

		private static string GetKeyDirectory()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

	}
}