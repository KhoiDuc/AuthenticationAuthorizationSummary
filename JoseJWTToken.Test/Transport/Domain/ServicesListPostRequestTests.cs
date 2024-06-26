﻿using System;
using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class ServicesListPostRequestTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var req = new ServicesListPostRequest(new System.Collections.Generic.List<Guid> { TestConsts.DefaultOrgId });

            Assert.IsTrue(req.ServiceIds != null);
            Assert.IsTrue(req.ServiceIds.Count == 1);
            Assert.IsTrue(req.ServiceIds[0] == TestConsts.DefaultOrgId);
        }
    }
}
