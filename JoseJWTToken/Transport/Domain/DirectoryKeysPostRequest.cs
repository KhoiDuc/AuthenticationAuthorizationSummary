﻿using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class DirectoryKeysPostRequest
    {
        [JsonProperty("directory_id")]
        public Guid DirectoryId { get; }

        [JsonProperty("public_key")]
        public string PublicKey { get; }

        [JsonProperty("date_expires")]
        public DateTime? Expires { get; }

        [JsonProperty("active")]
        public bool Active { get; }

        [JsonProperty("key_type")]
        public int KeyType { get; }

        public DirectoryKeysPostRequest(Guid directoryId, string publicKey, DateTime? expires, bool active, int keyType)
        {
            DirectoryId = directoryId;
            PublicKey = publicKey;
            Expires = expires;
            Active = active;
            KeyType = keyType;
        }
    }
}