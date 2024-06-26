﻿using Newtonsoft.Json;
using System;

namespace JoseJWTToken.Transport.Domain
{
    public class ServiceV3AuthsGetResponseDeviceJWE
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("denial_reason")]
        public string DenialReason { get; set; }

        [JsonProperty("auth_request")]
        public Guid AuthorizationRequestId { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("service_pins")]
        public string[] ServicePins { get; set; }

        [JsonProperty("auth_policy")]
        public AuthPolicy.JWEAuthPolicy AuthPolicy { get; set; }

        [JsonProperty("auth_methods")]
        public AuthPolicy.AuthMethod[] AuthMethods { get; set; }
    }
}
