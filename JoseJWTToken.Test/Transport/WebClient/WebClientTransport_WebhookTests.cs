﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using JoseJWTToken.Cache;
using JoseJWTToken.Crypto;
using JoseJWTToken.Crypto.Jwe;
using JoseJWTToken.Crypto.Jwt;
using JoseJWTToken.Error;
using JoseJWTToken.Json;
using JoseJWTToken.Transport;
using JoseJWTToken.Transport.Domain;
using JoseJWTToken.Transport.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JoseJWTToken.Test.Transport.WebClient
{
    /// <summary>
    /// webhooks test, separated from main tests because that was getting cluttered.
    /// </summary>
    [TestClass]
    public class WebClientTransport_WebhookTests
    {
        private readonly string BaseUrl = "https://api.launchkey.com";
        private ITransport Transport;
        private Mock<ICache> PublicKeyCache;
        private Mock<IHttpClient> HttpClient;
        private Mock<IJweService> JweService;
        private Mock<ICrypto> Crypto;
        private Mock<IJsonEncoder> JsonEncoder;
        private EntityIdentifier Issuer;
        private Mock<IJwtService> JwtService;
        private Mock<EntityKeyMap> KeyMap;
        private Mock<HttpResponse> HttpResponse;
        private Mock<JwtClaims> JwtClaims;
        private Mock<JwtClaimsResponse> JwtClaimsResponse;
        private Mock<JwtClaimsRequest> JwtClaimsRequest;
        private Mock<ServiceV3AuthsGetResponseDevice> DeviceResponse;
        private Mock<ServiceV3AuthsGetResponseCore> AuthsGetResponseCore;
        private Mock<ServerSentEventUserServiceSessionEnd> ServiceSessionEnd;
        private Dictionary<String, String> JweHeaders;

        [TestInitialize]
        public void Initialize()
        {
            HttpResponse = new Mock<HttpResponse>();
            HttpResponse.Object.StatusCode = HttpStatusCode.OK;
            HttpResponse.Object.Headers = new WebHeaderCollection
            {
                ["X-IOV-JWT"] = "IOV JWT"
            };
            HttpClient = new Mock<IHttpClient>();
            HttpClient.Setup(client => client.ExecuteRequest(It.IsAny<JoseJWTToken.Transport.WebClient.HttpMethod>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<Dictionary<string, String>>()))
                .Returns(HttpResponse.Object);
            Crypto = new Mock<ICrypto>();
            Crypto.Setup(c => c.DecryptRSA(It.IsAny<byte[]>(), It.IsAny<RSA>())).Returns(System.Text.Encoding.ASCII.GetBytes("Decrypted"));
            Crypto.Setup(c => c.Sha256(It.IsAny<byte[]>())).Returns(new byte[] { 255 });
            PublicKeyCache = new Mock<ICache>();
            PublicKeyCache.Setup(c => c.Get(It.IsAny<String>())).Returns("Public Key");
            Issuer = new EntityIdentifier(EntityType.Directory, default(Guid));
            JwtService = new Mock<IJwtService>();
            JwtService.Setup(s => s.Encode(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns("JWT Encoded");
            JwtClaims = new Mock<JwtClaims>();
            JwtClaimsResponse = new Mock<JwtClaimsResponse>();
            JwtClaimsResponse.Object.StatusCode = 200;
            JwtClaimsResponse.Object.LocationHeader = null;
            JwtClaimsResponse.Object.CacheControlHeader = null;
            JwtClaimsResponse.Object.ContentHash = "ff";
            JwtClaimsResponse.Object.ContentHashAlgorithm = "S256";

            JwtClaimsRequest = new Mock<JwtClaimsRequest>();
            JwtClaimsRequest.Object.ContentHash = "ff";
            JwtClaimsRequest.Object.ContentHashAlgorithm = "S256";
            JwtClaimsRequest.Object.Method = "POST";
            JwtClaimsRequest.Object.Path = "/webhook";


            JwtClaims.Object.Response = JwtClaimsResponse.Object;
            JwtClaims.Object.Request = JwtClaimsRequest.Object;

            JwtService.Setup(s => s.Decode(It.IsAny<RSA>(), It.IsAny<string>(), It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<String>()))
                .Returns(JwtClaims.Object);
            JwtService.Setup(s => s.GetJWTData(It.IsAny<String>()))
                .Returns(new JwtData("lka", "svc:8c3c0268-f692-11e7-bd2e-7692096aba47", "svc:8c3c0268-f692-11e7-bd2e-7692096aba47", "Key ID"));
            JweService = new Mock<IJweService>();
            JweService.Setup(s => s.Decrypt(It.IsAny<String>())).Returns("Decrypted JWE");
            JweService.Setup(s => s.Encrypt(It.IsAny<String>(), It.IsAny<RSA>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns("Encrypted JWE");
            JweHeaders = new Dictionary<string, string>
            {
                ["kid"] = "Public Key ID"
            };
            JweService.Setup(s => s.GetHeaders(It.IsAny<String>())).Returns(JweHeaders);
            JsonEncoder = new Mock<IJsonEncoder>();
            JsonEncoder.Setup(e => e.EncodeObject(It.IsAny<Object>())).Returns("JSON Encoded");
            JsonEncoder.Setup(e => e.DecodeObject<PublicV3PingGetResponse>(It.IsAny<String>())).Returns(new Mock<PublicV3PingGetResponse>().Object);
            AuthsGetResponseCore = new Mock<ServiceV3AuthsGetResponseCore>();
            AuthsGetResponseCore.Object.EncryptedDeviceResponse = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("Encrypted Device Response"));
            AuthsGetResponseCore.Object.JweEncryptedDeviceResponse = null;
            AuthsGetResponseCore.Object.ServiceUserHash = "Service User Hash";
            AuthsGetResponseCore.Object.OrgUserHash = "Org User Hash";
            AuthsGetResponseCore.Object.UserPushId = "User Push ID";
            AuthsGetResponseCore.Object.PublicKeyId = "Public Key ID";
            JsonEncoder.Setup(e => e.DecodeObject<ServiceV3AuthsGetResponseCore>(It.IsAny<String>())).Returns(AuthsGetResponseCore.Object);
            DeviceResponse = new Mock<ServiceV3AuthsGetResponseDevice>();
            JsonEncoder.Setup(e => e.DecodeObject<ServiceV3AuthsGetResponseDevice>(It.IsAny<String>())).Returns(DeviceResponse.Object);
            ServiceSessionEnd = new Mock<ServerSentEventUserServiceSessionEnd>();
            JsonEncoder.Setup(e => e.DecodeObject<ServerSentEventUserServiceSessionEnd>(It.IsAny<String>())).Returns(ServiceSessionEnd.Object);
            KeyMap = new Mock<EntityKeyMap>();
            KeyMap.Object.AddKey(EntityIdentifier.FromString("svc:8c3c0268-f692-11e7-bd2e-7692096aba47"), "Public Key ID", new Mock<RSA>().Object);
            Transport = new WebClientTransport(
                HttpClient.Object,
                Crypto.Object,
                PublicKeyCache.Object,
                BaseUrl,
                Issuer,
                JwtService.Object,
                JweService.Object,
                0,
                0,
                KeyMap.Object,
                JsonEncoder.Object);
        }


        [TestMethod]
        public void HandleServerSentEvent_ShouldHandleAuthPackage()
        {


            var response = Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "jwt" }},
                {"Content-Type", new List<string> {"application/jose" }}
            }, "body");

            Assert.IsTrue(response is ServerSentEventAuthorizationResponse);
        }

        [TestMethod]
        public void HandleServerSentEvent_ShouldHandleSessionEnd()
        {

            var response = Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "jwt" }},
                {"Content-Type", new List<string> {"application/json" }}
            }, "body");

            Assert.IsTrue(response is ServerSentEventUserServiceSessionEnd);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestException))]
        public void HandleServerSentEvent_PrivateClaims_ShouldValidateHash()
        {
            JwtClaimsRequest.Object.ContentHash = "Not the same";
            Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "my-jwt" }},
                {"Content-Type", new List<string> {"application/json" }}
            }, "body");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestException))]
        public void HandleServerSentEvent_PrivateClaims_ShouldValidateMethod()
        {
            var response = Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "my-jwt" }},
                {"Content-Type", new List<string> {"application/json" }}
            }, "body", "GET", "/webhook");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidRequestException))]
        public void HandleServerSentEvent_PrivateClaims_ShouldValidatePath()
        {
            var response = Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "my-jwt" }},
                {"Content-Type", new List<string> {"application/json" }}
            }, "body", "POST", "/webhookwrong");
        }

        [TestMethod]
        public void HandleServerSentEvent_PrivateClaims_ShouldIgnoreNullParameters()
        {
            var response = Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "my-jwt" }},
                {"Content-Type", new List<string> {"application/json" }}
            }, "body", null, null);

            Assert.IsTrue(response is ServerSentEventUserServiceSessionEnd);
        }

        [TestMethod]
        public void HandleServerSentEvent_ShouldReturnJweResponseDataIfPresent()
        {
            Mock<ServiceV3AuthsGetResponseDeviceJWE> deviceResponse = new Mock<ServiceV3AuthsGetResponseDeviceJWE>();
            deviceResponse.Object.DeviceId = "Device ID";
            deviceResponse.Object.ServicePins = new string[] { "PIN1", "PIN2" };
            deviceResponse.Object.Type = "AUTHORIZED";
            deviceResponse.Object.Reason = "APPROVED";
            deviceResponse.Object.DenialReason = "DEN1";
            AuthsGetResponseCore.Object.JweEncryptedDeviceResponse = "Encrypted Device JWE Response";
            JsonEncoder.Setup(d => d.DecodeObject<ServiceV3AuthsGetResponseDeviceJWE>(It.IsAny<String>())).Returns(deviceResponse.Object);
            var actual = (ServerSentEventAuthorizationResponse)Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "jwt" }},
                {"Content-Type", new List<string> {"application/jose" }}
            }, "body");
            Assert.AreEqual("Service User Hash", actual.ServiceUserHash);
            Assert.AreEqual("Org User Hash", actual.OrganizationUserHash);
            Assert.AreEqual("User Push ID", actual.UserPushId);
            Assert.AreEqual("Device ID", actual.DeviceId);
            Assert.AreEqual("PIN1", actual.DevicePins[0]);
            Assert.AreEqual("PIN2", actual.DevicePins[1]);
            Assert.IsTrue(actual.Response);
            Assert.AreEqual("AUTHORIZED", actual.Type);
            Assert.AreEqual("APPROVED", actual.Reason);
            Assert.AreEqual("DEN1", actual.DenialReason);
        }


        [TestMethod]
        public void HandleServerSentEvent_ShouldReturnFalseForResponseIfJweResponseReDataIfPresentAndTypeIsNotAuthorized()
        {
            Mock<ServiceV3AuthsGetResponseDeviceJWE> deviceResponse = new Mock<ServiceV3AuthsGetResponseDeviceJWE>();
            deviceResponse.Object.DeviceId = "Device ID";
            deviceResponse.Object.ServicePins = new string[] { "PIN1", "PIN2" };
            deviceResponse.Object.Type = "NOT AUTHORIZED";
            deviceResponse.Object.Reason = "APPROVED";
            deviceResponse.Object.DenialReason = "DEN1";
            AuthsGetResponseCore.Object.JweEncryptedDeviceResponse = "Encrypted Device JWE Response";
            JsonEncoder.Setup(d => d.DecodeObject<ServiceV3AuthsGetResponseDeviceJWE>(It.IsAny<String>())).Returns(deviceResponse.Object);
            var actual = (ServerSentEventAuthorizationResponse)Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "jwt" }},
                {"Content-Type", new List<string> {"application/jose" }}
            }, "body");
            Assert.IsFalse(actual.Response);
        }

        [TestMethod]
        public void HandleServerSentEvent_ShouldReturnNonJweResponseIfJweResponseNotPresent()
        {
            DeviceResponse.Object.DeviceId = "Device ID";
            DeviceResponse.Object.ServicePins = new string[] { "PIN1", "PIN2" };
            DeviceResponse.Object.Response = true;
            var actual = (ServerSentEventAuthorizationResponse)Transport.HandleServerSentEvent(new Dictionary<string, List<string>>
            {
                {"X-IOV-JWT", new List<string> { "jwt" }},
                {"Content-Type", new List<string> {"application/jose" }}
            }, "body");
            Assert.AreEqual("Service User Hash", actual.ServiceUserHash);
            Assert.AreEqual("Org User Hash", actual.OrganizationUserHash);
            Assert.AreEqual("User Push ID", actual.UserPushId);
            Assert.AreEqual("Device ID", actual.DeviceId);
            Assert.AreEqual("PIN1", actual.DevicePins[0]);
            Assert.AreEqual("PIN2", actual.DevicePins[1]);
            Assert.IsTrue(actual.Response);
            Assert.IsNull(actual.Type);
            Assert.IsNull(actual.Reason);
            Assert.IsNull(actual.DenialReason);
        }
    }
}

