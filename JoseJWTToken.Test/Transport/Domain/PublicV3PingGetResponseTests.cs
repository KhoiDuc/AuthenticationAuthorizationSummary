﻿using JoseJWTToken.Transport.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace JoseJWTToken.Test.Transport.Domain
{
    [TestClass]
    public class PublicV3PingGetResponseTests
    {
        [TestMethod]
        public void ShouldDeserialize()
        {
            var json = "{\"api_time\": \"2018-01-11T05:22:17Z\"}";
            var obj = JsonConvert.DeserializeObject<PublicV3PingGetResponse>(json);

            Assert.AreEqual(new DateTime(2018, 1, 11, 5, 22, 17, DateTimeKind.Utc), obj.ApiTime);
        }
    }
}
