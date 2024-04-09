using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class OrganizationV3DirectoriesPatchRequestTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var request = new OrganizationV3DirectoriesPatchRequest(TestConsts.DefaultDirectoryId, true, "a", "i", true, TestConsts.DefaultWebhookUrl);

            Assert.AreEqual(true, request.Active);
            Assert.AreEqual("a", request.AndroidKey);
            Assert.AreEqual("i", request.IosP12);
            Assert.AreEqual(TestConsts.DefaultDirectoryId, request.DirectoryId);
            Assert.AreEqual(TestConsts.DefaultWebhookUrl, request.WebhookUrl);
        }
    }
}
