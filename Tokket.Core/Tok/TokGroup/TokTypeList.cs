//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Tok Group</summary>
    public class TokTypeList : BaseModel
    {
        /// <summary>Tok Group</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "toktypelist";

        /// <summary>Tok Group Name</summary>
        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "primary_field_name")]
        public string PrimaryFieldName { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "secondary_field_name")]
        public string SecondaryFieldName { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "is_detail_based")]
        public bool IsDetailBased { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "is_mega")]
        public bool IsMega { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "is_read_only")]
        public bool IsReadOnly { get; set; } = false;

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "primary_char_limit")]
        public long PrimaryCharLimit { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "secondary_char_limit")]
        public long SecondaryCharLimit { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "tok_types")]
        public string[] TokTypes { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "tok_type_ids")]
        public string[] TokTypeIds { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "optional_fields")]
        public string[] OptionalFields { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "optional_limits")]
        public long[] OptionalLimits { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "required_fields")]
        public string[] RequiredFields { get; set; }

        //INFORMATIONAL FIELDS
        //Tok Group Description

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>Tok Group</summary>
        //Tok Type Descriptions
        [JsonProperty(PropertyName = "descriptions")]
        public string[] Descriptions { get; set; }

        /// <summary>Tok Group</summary>
        //Tok Group Examples
        [JsonProperty(PropertyName = "example")]
        public string Example { get; set; }

        /// <summary>Tok Group</summary>
        //Tok Type Descriptions
        [JsonProperty(PropertyName = "examples")]
        public string[] Examples { get; set; }

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "has_answer_field", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasAnswerField { get; set; }

        #region Details min, max, and default
        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "details_min")]
        public int DetailsMin { get; set; } = 3;

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "details_max")]
        public int DetailsMax { get; set; } = 5;

        /// <summary>Tok Group</summary>
        [JsonProperty(PropertyName = "details_default")]
        public int DetailsDefault { get; set; } = 5;
        #endregion
    }
}
