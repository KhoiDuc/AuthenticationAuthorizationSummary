﻿using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectoriesPatchRequest
    {
        [JsonProperty("directory_id")]
        public Guid DirectoryId { get; }

        [JsonProperty("active")]
        public bool Active { get; }

        [JsonProperty("android_key")]
        public string AndroidKey { get; }

        [JsonProperty("ios_p12")]
        public string IosP12 { get; }

        [JsonProperty("webhook_url", NullValueHandling=NullValueHandling.Include)]
        public string WebhookUrl { get; }

        [JsonProperty("denial_context_inquiry_enabled", NullValueHandling=NullValueHandling.Ignore)]
        public bool? DenialContextInquiryEnabled { get; }

        public OrganizationV3DirectoriesPatchRequest(Guid directoryId, bool active, string androidKey, string iosP12, bool? denialContextInquiryEnabled, string webhookUrl)
        {
            DirectoryId = directoryId;
            Active = active;
            AndroidKey = androidKey;
            IosP12 = iosP12;
            WebhookUrl = webhookUrl;
            DenialContextInquiryEnabled = denialContextInquiryEnabled;
        }
    }
}