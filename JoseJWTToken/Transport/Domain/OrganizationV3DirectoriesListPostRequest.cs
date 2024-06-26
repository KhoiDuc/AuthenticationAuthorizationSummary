﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JoseJWTToken.Transport.Domain
{
    public class OrganizationV3DirectoriesListPostRequest
    {
        [JsonProperty("directory_ids")]
        public List<Guid> DirectoryIds { get; }

        public OrganizationV3DirectoriesListPostRequest(List<Guid> directoryIds)
        {
            DirectoryIds = directoryIds;
        }
    }
}