using JoseJWTToken.Crypto.Jwt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Crypto.Jwt
{
    [TestClass]
    public class JwtDataTests
    {
        [TestMethod]
        public void TestMutability()
        {
            var data = new JwtData("iss", "sub", "aud", "kid");

            Assert.AreEqual(data.Issuer, "iss");
            Assert.AreEqual(data.Subject, "sub");
            Assert.AreEqual(data.Audience, "aud");
            Assert.AreEqual(data.KeyId, "kid");
        }
    }
}
