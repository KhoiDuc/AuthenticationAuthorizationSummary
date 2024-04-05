using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using System.Xml.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Math;

namespace GenerateKeyEncryptDecrypt
{
    public class Program
    {
        public class PemReaderB
        {
            public static RSACryptoServiceProvider GetRSAProviderFromPem(String pemstr)
            {
                CspParameters cspParameters = new()
                {
                    KeyContainerName = "MyKeyContainer"
                };
                RSACryptoServiceProvider rsaKey = new(cspParameters);

                Func<RSACryptoServiceProvider, RsaKeyParameters, RSACryptoServiceProvider> MakePublicRCSP = (RSACryptoServiceProvider rcsp, RsaKeyParameters rkp) =>
                {
                    RSAParameters rsaParameters = DotNetUtilities.ToRSAParameters(rkp);
                    rcsp.ImportParameters(rsaParameters);
                    return rsaKey;
                };

                Func<RSACryptoServiceProvider, RsaPrivateCrtKeyParameters, RSACryptoServiceProvider> MakePrivateRCSP = (RSACryptoServiceProvider rcsp, RsaPrivateCrtKeyParameters rkp) =>
                {
                    RSAParameters rsaParameters = DotNetUtilities.ToRSAParameters(rkp);
                    rcsp.ImportParameters(rsaParameters);
                    return rsaKey;
                };

                PemReader reader = new PemReader(new StringReader(pemstr));
                object kp = reader.ReadObject();

                // If object has Private/Public property, we have a Private PEM
                return (kp.GetType().GetProperty("Private") != null) ? MakePrivateRCSP(rsaKey, (RsaPrivateCrtKeyParameters)(((AsymmetricCipherKeyPair)kp).Private)) : MakePublicRCSP(rsaKey, (RsaKeyParameters)kp);
            }

            public static RSACryptoServiceProvider GetRSAProviderFromPemFile(String pemfile)
            {
                return GetRSAProviderFromPem(File.ReadAllText(pemfile).Trim());
            }
        }
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadAsymmetricKeyParameter(string pemFilename)
        {
            var fileStream = System.IO.File.OpenText(pemFilename);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(fileStream);
            var KeyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)pemReader.ReadObject();
            return KeyParameter;
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to XML key converter! Please select an option:");
                Console.WriteLine("1. XML to PEM\n2. PEM to XML\n\nAny other key to exit...");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("\nReading from file: xmlFile.xml");
                        string wholeXml = File.ReadAllText("Keys\\Public XML 1.txt");
                        if (string.IsNullOrWhiteSpace(wholeXml))
                        {
                            Console.WriteLine("Enter something next time, dumbass!\n\n");
                            break;
                        }

                        Console.WriteLine($"\n{RsaKeyConverter.XmlToPem(wholeXml)}\n\n");
                        break;

                    case ConsoleKey.D2:
                        Console.WriteLine("\nReading from file: pemFile.pem");
                        string wholePem = File.ReadAllText("Keys\\Private Key.pem");
                        if (string.IsNullOrWhiteSpace(wholePem))
                        {
                            Console.WriteLine("Enter something next time, dumbass!\n\n");
                            break;
                        }
                        // XML value file
                        //Console.WriteLine($"\n{RsaKeyConverter.GetRsaParametersAsXml(wholePem)}\n\n");
                        // XML value format
                        Console.WriteLine($"\n{RsaKeyConverter.PemToXml(wholePem)}\n\n");
                        break;

                    default:
                        Console.WriteLine("\nExiting");
                        return;
                }
            }
            //TestRSAFlow();
        }
        static void TestRSAFlow()
        {
            if (!Directory.Exists("Keys"))
                Directory.CreateDirectory("Keys");

            // Exporting
            var rsa = new RSACryptoServiceProvider(2048); // Creates new instance with key size.

            Manager.Export(rsa, "Keys/Keys Base64.txt"); // Converts the public & private key to XML format and saves in specified path.

            var publicKeyXml = Manager.KeyToString(rsa.ExportParameters(false));
            var privateKeyXml = Manager.KeyToString(rsa.ExportParameters(true));

            // Writing public key in XML format
            File.WriteAllText("Keys/Public XML.txt", publicKeyXml);
            // Writing public key in PEM format
            File.WriteAllText("Keys/Public Key.pem", Manager.KeyToPemFormatString(rsa.ExportParameters(false), true));
            // Writing public key in PEM format (using new solution)
            File.WriteAllText("Keys/Public Key Convert From XML.pem", RsaKeyConverter.XmlToPem(publicKeyXml));
            // Writing public key in PEM format (using old solution)
            File.WriteAllText("Keys/Public Key Convert From XML OLD Solution.pem", Manager.GetPemEncodedPublicKey(publicKeyXml));

            // Writing private key in XML format
            File.WriteAllText("Keys/Private XML.txt", privateKeyXml);
            // Writing private key in PEM format
            File.WriteAllText("Keys/Private Key.pem", Manager.KeyToPemFormatString(rsa.ExportParameters(true), false));
            // Writing private key in PEM format (using new solution)
            File.WriteAllText("Keys/Private Key Convert From XML.pem", RsaKeyConverter.XmlToPem(privateKeyXml));
            // Writing private key in PEM format (using old solution)
            File.WriteAllText("Keys/Private Key Convert From XML OLD Solution.pem", Manager.GetPemEncodedPrivateKey(privateKeyXml));

            // Importing
            var importedRsa = Manager.Import("Keys/Keys Base64.txt"); // Reads the public & private key from the XML format.
            var publicKey = Manager.KeyToString(importedRsa.ExportParameters(false)); // Grabbing the public key as an XML string. 
            var privateKey = Manager.KeyToString(importedRsa.ExportParameters(true)); // Grabbing the private key as an XML string.

            string textToEncrypt = "Hello World!"; //Unencrypted string.
            Write("Text to encrypt:", textToEncrypt);

            var encryptedText = RSACustom.Encrypt(textToEncrypt, publicKey); // Encrypting string.
            Write("Encrypted text:", encryptedText);

            var decryptedText = RSACustom.Decrypt(encryptedText, privateKey); // Decrypting string.
            Write("Decrypted text:", decryptedText);


            string xmlString = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <RSAParameters xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
              <Exponent>AQAB</Exponent>
              <Modulus>0Ltwa06xY3sz+QXYnC2VjGXSeRYAjzN7Wfcnz3zlIHyjV4t/xFRzvqqgxglONUklZCFpmRNdzKN0A7Fsi3EDSoMsDxbtHeL2tJlI+8IIxmdkW5AWAHaCFLEEd1aty9pyrMij4m83Pyqo2BAtQkiVPbEBfmOInXLlsnf/Myp9uC/TACiyQTcEEgjCRKS/wvCkMH3Cm7prljTxTfVub6HaDtn1XEBD7nf5ZzQQ0q8qD/U8ZafaJmZxWiBPKP1W8oGU9c0FLnzbTVX5pMJOrdwiD6oNhBpFS5h0AMVxaxIhOFRxac7yNBGT+9k8DKENBo/fp+WAdr7v7BjGXtWkYPzcvQ==</Modulus>
            </RSAParameters>";

            string xmlString2 = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <RSAParameters xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
              <D>KKMFTO7iHCMFSEr6E9XQEqhUJJzC+R4luqsihgOjvAJn7ohLFJb4fbtMV6HCJx+ZdvgGTfX2Qfvkfz+QiUk0QdJtxnZJLZNdQmXBTrpE6ROB902C6w4nXw3jd7RZW6Gox7i8Jz5NvC969a2Ykhn9bChEyS1pkHWpAGjaN0TjevMCluyjbosn7xFNVYYRRbH5uWs7NHDcYmJ5WEOfr2j/N0rObBl0e8DIRXn5DBY18RmHh4Tmxu0oZsVJvDG69KePG1sRdWgOvuRFHF7AOgwwTOQaEXzBjmFYlMaBs+bbXPqs7V7PnCckV1Qso9kp5k5vQNqB4WhsiBdjza9YZTN5xQ==</D>
              <DP>S0OW5KKw3ZMfXNr5eRya9zD8bZNx75H07BmIb4+mLIHpUFf6YlHVKngl+19UCR30yFOSrRGBTKLHw0baXctPll1kVlaFOYoXIg80bC1Av6oafm3Kq2jrVobRZWRCxUny/0AmvmORjB27Xlr9r0t+JahavKY7M0Xhs20krtyvzpU=</DP>
              <DQ>PL1M2/pB4Yh1kym3r6ql9dPi+xCq1vdKCXaCoiHJ4Z5L0CHavxh6fGdMPW/9TksTGmbB/0Lx4Wboo0FXPJ2FsJsoIZa3ot83ydMSRby9/POc72WR6/TUh7I150P5vAvxE7DlqOguhzbQd2e2UfNvRaNKlb+zOHuf7AM5oUiWwrE=</DQ>
              <Exponent>AQAB</Exponent>
              <InverseQ>r+IErkmLOr0wvL6+ixMgQ5sRkz/H/Gvf4pspf3H9iTMRrqIo65U26w/S1tM2yY+aZFIOcGZzlPanI61oAO4xmHge8FJ+GF6b7oakfreKTQ/yoOAtqfH9ZSZyba+c2IZIpc9YZgI7l3CDobpbV23x7nkge4CZk7M0JOpyH7XTVaU=</InverseQ>
              <Modulus>0Ltwa06xY3sz+QXYnC2VjGXSeRYAjzN7Wfcnz3zlIHyjV4t/xFRzvqqgxglONUklZCFpmRNdzKN0A7Fsi3EDSoMsDxbtHeL2tJlI+8IIxmdkW5AWAHaCFLEEd1aty9pyrMij4m83Pyqo2BAtQkiVPbEBfmOInXLlsnf/Myp9uC/TACiyQTcEEgjCRKS/wvCkMH3Cm7prljTxTfVub6HaDtn1XEBD7nf5ZzQQ0q8qD/U8ZafaJmZxWiBPKP1W8oGU9c0FLnzbTVX5pMJOrdwiD6oNhBpFS5h0AMVxaxIhOFRxac7yNBGT+9k8DKENBo/fp+WAdr7v7BjGXtWkYPzcvQ==</Modulus>
              <P>6ZJ9DpccfaLQjYVAwjm0ECV6TBQRlvh3Qq2yFzo7jAOopjzKU2eK/6b6lm4cwneAQgwDZuvIy+Yo8zTM+QN71o2roAaKcCXUdVMFD0UucJQc5/oY/JOWMHeS8yVpXLJAKoPRh4giIft79t01lZR3yGm2NhPUklqv4xYWmJzlTDc=</P>
              <Q>5MZZluE/nUh9o/KBm/0FxjxkgFkRplfR30mBexAQ/8Rnyg79itEtn0ahUftzNEPRAAcCUyty0pQ3O9JAPBj8jyMV9lTha8wiB+2UyUEkv2fXFZN/umuWBJJiiZ7LJwrnZSJje+hVcTB8+PDIeDLdaWFXlb/d/K/4G9Liwj1SrKs=</Q>
            </RSAParameters>";

            string base64Encoded = RsaKeyConverter.XmlToPem(xmlString);
            Write("XML (Public key):", xmlString);
            Write("String (Pem format)", base64Encoded);

            Console.WriteLine("\n");
            string base64Encoded2 = RsaKeyConverter.XmlToPem(xmlString2);
            Write("XML (Private key):", xmlString2);
            Write("String (Pem format)", base64Encoded2);

            Console.ReadKey();
        }

        /// <summary>
        /// Writes the.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        static void Write(string first, string second)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(first);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(second + Environment.NewLine);
        }
    }
}
