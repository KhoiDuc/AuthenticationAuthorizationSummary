﻿using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class ServiceKeysPostRequest
    {
        [JsonProperty("service_id")]
        public Guid ServiceId { get; }

        [JsonProperty("public_key")]
        public string PublicKey { get; }

        [JsonProperty("date_expires")]
        public DateTime? Expires { get; }

        [JsonProperty("active")]
        public bool Active { get; }

        [JsonProperty("key_type")]
        public int KeyType { get; }

        public ServiceKeysPostRequest(Guid serviceId, string publicKey, DateTime? expires, bool active, int keyType)
        {
            ServiceId = serviceId;
            PublicKey = publicKey;
            Expires = expires;
            Active = active;
            KeyType = keyType;
        }
    }
}