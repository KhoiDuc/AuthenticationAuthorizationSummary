using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class PublicV3PublicKeyGetResponseTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var o = new PublicV3PublicKeyGetResponse("key", "finger");
            Assert.AreEqual("key", o.PublicKey);
            Assert.AreEqual("finger", o.PublicKeyFingerPrint);
        }
    }
}
