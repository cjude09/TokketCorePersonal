//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    /// <summary>The unique title of a user.</summary>
    public class TokketTitle : BaseModel
    {
        /// <summary>Title.</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "title";

        /// <summary>Title kinds: unique, generic, royalty.</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "unique";

        /// <summary>User who currently owns the title.</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>Id of the purchase record.</summary>
        [JsonProperty(PropertyName = "purchase_id")]
        public string PurchaseId { get; set; }

        /// <summary>Title kind: unique and nonunique.</summary>
        [JsonProperty(PropertyName = "unique")]
        public bool IsUnique { get; set; }

        /// <summary>Display of the title. Case sensitive</summary>
        [JsonProperty(PropertyName = "title_display", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleDisplay { get; set; } = null;

        //The id is in the id field

        //Note: Unique titles are stored in a user's or all titles partition (on top of id=pk), nonunique are in the database
        /// <summary>Category of the title (generic only)</summary>
        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; } = null;
    }
}
