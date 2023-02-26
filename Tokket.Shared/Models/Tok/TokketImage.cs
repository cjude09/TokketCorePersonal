using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models.Tok
{
    /// <summary>Database record of an image posted by a user. Stored in {userId}-images0, {userId}-images1, etc. Stored in Knowledge, even if uploaded from a Tok game.</summary>
    public class TokketImage : BaseModel
    {
        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = "image";

        /// <summary>Image kinds: "standard", "user", "cover", "tok", "set".</summary>
        [JsonProperty("kind", NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "standard";

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>Image url.</summary>
        [JsonProperty("image")]
        public string Image { get; set; }

        /// <summary>Image url.</summary>
        [JsonProperty("image_base64", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageBase64 { get; set; } = null;

        /// <summary>Image Height (in pixels)</summary>
        [JsonProperty(PropertyName = "image_height", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageHeight { get; set; } = null;

        /// <summary>Image Width (in pixels)</summary>
        [JsonProperty(PropertyName = "image_width", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageWidth { get; set; } = null;

        /// <summary>Image Extension (.jpg, .png, the period needs to be included)</summary>
        [JsonProperty(PropertyName = "image_extension", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageExtension { get; set; } = null;

        #region Categorical
        /// <summary>Tok Groups are a grouping that ensures necessary character limits, field names, and number of details are applied - it is a post format. See <see cref="TokTypeList"/> for more info.</summary>
        [JsonProperty(PropertyName = "tok_group", NullValueHandling = NullValueHandling.Ignore)]
        public string TokGroup { get; set; } = null;

        /// <summary>Tok types divide a tok group into more practical and specific groupings. Most of the time the tok type is still broad enough to have a wide variety of <see cref="Category"/></summary>
        [JsonProperty(PropertyName = "tok_type", NullValueHandling = NullValueHandling.Ignore)]
        public string TokType { get; set; } = null;

        /// <summary>Uniquely identifies tok types and must include the tok group. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.TokType"/></summary>
        [JsonProperty(PropertyName = "tok_type_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TokTypeId { get; set; } = null;

        /// <summary>Categorizes the tok into a specific topic. It should be more specific than the <see cref="TokType"/>, and it should not contain anything in <see cref="PrimaryFieldText"/> or <see cref="Details"/>.</summary>
        [MaxLength(100)]
        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; } = null;

        /// <summary>Uniquely identifies categories. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.Category"/></summary>
        [JsonProperty(PropertyName = "category_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryId { get; set; } = null;
        #endregion
    }
}
