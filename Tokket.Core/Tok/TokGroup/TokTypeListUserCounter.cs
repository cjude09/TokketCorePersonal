//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    /// <summary>  
    /// Stores user tok count and type/group points
    /// </summary>
    public class TokTypeListUserCounter : BaseModel
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "toktypelistusercounter";

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; } = "user";

        [JsonProperty(PropertyName = "userDisplayName")]
        public string UserDisplayName { get; set; } = "User Name";

        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        [JsonProperty(PropertyName = "group_count")]
        public long GroupCount { get; set; }

        [JsonProperty(PropertyName = "group_points")]
        public long GroupPoints { get; set; }

        [JsonProperty(PropertyName = "group_reports")]
        public long GroupReports { get; set; }

        [JsonProperty(PropertyName = "group_reports_current")]
        public long GroupReportsCurrent { get; set; }

        [JsonProperty(PropertyName = "tok_types")]
        public string[] TokTypes { get; set; }

        [JsonProperty(PropertyName = "tok_type_ids")]
        public string[] TokTypeIds { get; set; }

        [JsonProperty(PropertyName = "type_counts")]
        public long[] TokTypeCounts { get; set; }

        [JsonProperty(PropertyName = "type_points")]
        public long[] TokTypePoints { get; set; }

        [JsonProperty(PropertyName = "type_reports")]
        public long[] TokTypeReports { get; set; }

        [JsonProperty(PropertyName = "type_reports_current")]
        public long[] TokTypeReportsCurrent { get; set; }

        [JsonProperty(PropertyName = "created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
