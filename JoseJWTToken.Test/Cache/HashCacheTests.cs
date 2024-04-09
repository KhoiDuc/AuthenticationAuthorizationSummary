using JoseJWTToken.Cache;
using JoseJWTToken.Error;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Cache
{
    [TestClass]
    public class HashCacheTests
    {
        [TestMethod]
        public void TestPersistence()
        {
            var cache = new HashCache();
            cache.Put("test_key", "test_value");
            Assert.AreEqual(cache.Get("test_key"), "test_value");
        }

        [TestMethod]
        [ExpectedException(typeof(CacheException))]
        public void TestGetInvalidKey()
        {
            var cache = new HashCache();
            cache.Get("Doesn't exist");
        }

        [TestMethod]
        [ExpectedException(typeof(CacheException))]
        public void TestSetInvalidKey()
        {
            var cache = new HashCache();
            cache.Put(null, "value");
        }
    }
}
