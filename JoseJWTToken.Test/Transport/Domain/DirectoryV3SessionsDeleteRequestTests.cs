using JoseJWTToken.Json;
using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class DirectoryV3SessionsDeleteRequestTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var o = new DirectoryV3SessionsDeleteRequest("id");
            Assert.AreEqual(o.Identifier, "id");
        }

        [TestMethod]
        public void ShouldSerializeCorrectly()
        {
            var encoder = new JsonNetJsonEncoder();
            var o = new DirectoryV3SessionsDeleteRequest("id");
            var json = encoder.EncodeObject(o);
            Assert.AreEqual("{\"identifier\":\"id\"}", json);
        }
    }
}
