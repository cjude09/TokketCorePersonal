//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokTypeListCounter : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "toktypelistcounter";

        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        [JsonProperty(PropertyName = "group_count")]
        public long GroupCount { get; set; }

        [JsonProperty(PropertyName = "tok_types")]
        public string[] TokTypes { get; set; }

        [JsonProperty(PropertyName = "tok_type_ids")]
        public string[] TokTypeIds { get; set; }

        [JsonProperty(PropertyName = "type_counts")]
        public long[] TokTypeCounts { get; set; }

        [JsonProperty(PropertyName = "set_counts")]
        public long[] SetCounts { get; set; }

        [JsonProperty(PropertyName = "group_count_sets")]
        public long GroupCountSets { get; set; }
    }
}
