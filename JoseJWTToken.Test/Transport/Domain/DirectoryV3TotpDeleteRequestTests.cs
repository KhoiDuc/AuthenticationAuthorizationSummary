using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JoseJWTToken.Json;
using JoseJWTToken.Transport.Domain;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class DirectoryV3TotpDeleteRequestTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var deviceGuid = Guid.NewGuid();
            var o = new DirectoryV3TotpDeleteRequest("id");
            Assert.AreEqual(o.Identifier, "id");
        }

        [TestMethod]
        public void ShouldSerializeCorrectly()
        {
            var encoder = new JsonNetJsonEncoder();
            var o = new DirectoryV3TotpDeleteRequest("id");
            var json = encoder.EncodeObject(o);
            Assert.AreEqual("{\"identifier\":\"id\"}", json);
        }
    }
}