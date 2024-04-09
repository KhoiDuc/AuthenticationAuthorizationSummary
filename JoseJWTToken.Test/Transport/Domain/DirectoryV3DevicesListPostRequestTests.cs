using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoseJWTToken.Json;
using JoseJWTToken.Transport.Domain;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class DirectoryV3DevicesListPostRequestTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var o = new DirectoryV3DevicesListPostRequest("id");
            Assert.AreEqual(o.Identifier, "id");
        }

        [TestMethod]
        public void ShouldSerializeCorrectly()
        {
            var encoder = new JsonNetJsonEncoder();
            var o = new DirectoryV3DevicesListPostRequest("id");
            var json = encoder.EncodeObject(o);
            Assert.AreEqual("{\"identifier\":\"id\"}", json);
        }
    }
}
