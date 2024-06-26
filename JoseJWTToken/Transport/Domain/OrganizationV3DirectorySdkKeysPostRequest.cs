﻿using System;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectorySdkKeysPostRequest
    {
        [JsonProperty("directory_id")]
        public Guid DirectoryId { get; }

        public OrganizationV3DirectorySdkKeysPostRequest(Guid directoryId)
        {
            DirectoryId = directoryId;
        }
    }
}