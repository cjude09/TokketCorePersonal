//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokType : TokGroup
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public new string Label { get; set; } = "toktype";

        [JsonProperty(PropertyName = "tok_type")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        [JsonProperty(PropertyName = "example", NullValueHandling = NullValueHandling.Ignore)]
        public string Example { get; set; } = null;
    }
}
