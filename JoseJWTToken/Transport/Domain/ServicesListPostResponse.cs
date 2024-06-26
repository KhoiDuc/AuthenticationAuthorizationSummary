﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServicesListPostResponse
    {
        public class Service
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("icon")]
            public Uri Icon { get; set; }

            [JsonProperty("callback_url")]
            public Uri CallbackUrl { get; set; }

            [JsonProperty("active")]
            public bool Active { get; set; }
        }

        public List<Service> Services { get; }

        public ServicesListPostResponse(List<Service> services)
        {
            Services = services;
        }
    }
}