﻿using System;
using JoseJWTToken.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JoseJWTToken.Test.Json
{
    [TestClass]
    public class UnixTimestampJsonDateConverterTests
    {
        class TestPoco
        {
            [JsonProperty("when")]
            [JsonConverter(typeof(UnixTimestampJsonDateConverter))]
            public DateTime When { get; set; }
        }

        [TestMethod]
        public void ShouldEncodeUnixTimestamps()
        {
            var obj = new TestPoco
            {
                When = new DateTime(2018, 1, 11, 4, 57, 54, DateTimeKind.Utc)
            };
            var serialized = JsonConvert.SerializeObject(obj);
            Assert.AreEqual("{\"when\":1515646674}", serialized);
        }

        [TestMethod]
        public void ShouldDecodeUnixTimestamps()
        {
            var expected = new DateTime(2018, 1, 11, 4, 57, 54, DateTimeKind.Utc);
            var json = "{\"when\":1515646674}";
            var obj = JsonConvert.DeserializeObject<TestPoco>(json);
            Assert.AreEqual(expected, obj.When);
        }
    }
}
