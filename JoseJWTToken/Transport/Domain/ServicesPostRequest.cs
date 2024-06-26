﻿using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicesPostRequest
    {
        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("icon")]
        public Uri Icon { get; }

        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("callback_url")]
        public Uri CallbackUrl { get; }

        [JsonProperty("active")]
        public bool Active { get; }

        public ServicesPostRequest(string name, string description, Uri icon, Uri callbackUrl, bool active)
        {
            Name = name;
            Description = description;
            Icon = icon;
            CallbackUrl = callbackUrl;
            Active = active;
        }
    }
}