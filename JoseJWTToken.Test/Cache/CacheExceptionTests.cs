﻿using JoseJWTToken.Error;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JoseJWTToken.Test.Cache
{
    [TestClass]
    public class CacheExceptionTests
    {
        [TestMethod]
        public void InnerExceptionAndMessageIsSet()
        {
            var inner = new Exception("inner");
            var cacheException = new CacheException("special message", inner);
            Assert.AreEqual(cacheException.Message, "special message");
            Assert.AreEqual(cacheException.InnerException, inner);
        }

        [TestMethod]
        public void MessageIsSet()
        {
            var cacheException = new CacheException("special message");
            Assert.AreEqual(cacheException.Message, "special message");
            Assert.AreEqual(cacheException.InnerException, null);
        }
    }
}