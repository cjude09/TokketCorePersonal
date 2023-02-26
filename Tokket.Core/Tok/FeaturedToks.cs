//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class FeaturedToks
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "featuredtoks";

        [JsonRequired]
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; } = "featuredtoks";

        [JsonProperty(PropertyName = "toks")]
        public List<string> Toks { get; set; }
    }
}
