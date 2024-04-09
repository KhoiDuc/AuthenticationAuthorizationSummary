using System.Security.Cryptography;
using JoseJWTToken.Transport.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.WebClient
{
    [TestClass]
    public class CachedKeyTests
    {
        [TestMethod]
        public void TestMutability()
        {
            var rsa = new RSACryptoServiceProvider();
            var key = new CachedKey("test", rsa);

            Assert.AreEqual(key.Thumbprint, "test");
            Assert.AreEqual(key.KeyData, rsa);
        }
    }
}
