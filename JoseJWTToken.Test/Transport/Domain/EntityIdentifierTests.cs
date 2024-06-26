﻿using System;
using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class EntityIdentifierTests
    {
        Guid _testGuid = Guid.Parse("f8329e0e-dd7a-11e7-98b4-469158467b1a");
        string _testGuidString = "f8329e0e-dd7a-11e7-98b4-469158467b1a";

        [TestMethod]
        public void TestDirectoryToString()
        {
            var eid = new EntityIdentifier(EntityType.Directory, _testGuid);
            Assert.AreEqual($"dir:{_testGuidString}", eid.ToString());
        }

        [TestMethod]
        public void TestOrganizationToString()
        {
            var eid = new EntityIdentifier(EntityType.Organization, _testGuid);
            Assert.AreEqual($"org:{_testGuidString}", eid.ToString());
        }

        [TestMethod]
        public void TestServiceToString()
        {
            var eid = new EntityIdentifier(EntityType.Service, _testGuid);
            Assert.AreEqual($"svc:{_testGuidString}", eid.ToString());
        }

        [TestMethod]
        public void TestParseOrg()
        {
            var eid = EntityIdentifier.FromString($"org:{_testGuidString}");

            Assert.AreEqual(eid.Id, _testGuid);
            Assert.AreEqual(eid.Type, EntityType.Organization);
        }

        [TestMethod]
        public void TestParseDirectory()
        {
            var eid = EntityIdentifier.FromString($"dir:{_testGuidString}");

            Assert.AreEqual(eid.Id, _testGuid);
            Assert.AreEqual(eid.Type, EntityType.Directory);
        }

        [TestMethod]
        public void TestParseService()
        {
            var eid = EntityIdentifier.FromString($"svc:{_testGuidString}");

            Assert.AreEqual(eid.Id, _testGuid);
            Assert.AreEqual(eid.Type, EntityType.Service);
        }

        [TestMethod]
        public void TestEncodeAndDecode()
        {
            var eid1 = new EntityIdentifier(EntityType.Organization, _testGuid);
            var eid1Str = eid1.ToString();
            var eid2 = EntityIdentifier.FromString(eid1Str);

            Assert.AreEqual(eid1, eid2);
        }
    }
}
