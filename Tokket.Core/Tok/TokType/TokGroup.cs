//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokGroup : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tokgroup";

        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroupName { get; set; }

        [JsonProperty("toks")]
        public long Toks { get; set; }

        [JsonProperty("points")]
        public long Points { get; set; }

        [JsonProperty("coins")]
        public long Coins { get; set; }

        [JsonProperty("sets")]
        public long Sets { get; set; }

        [JsonProperty("deleted_toks")]
        public long DeletedToks { get; set; }

        [JsonProperty("deleted_points")]
        public long DeletedPoints { get; set; }

        [JsonProperty("deleted_coins")]
        public long DeletedCoins { get; set; }

        [JsonProperty("deleted_sets")]
        public long DeletedSets { get; set; }

        #region Reactions
        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedReactions { get; set; } = null;

        [JsonProperty(PropertyName = "likes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Likes { get; set; } = null;

        [JsonProperty(PropertyName = "dislikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Dislikes { get; set; } = null;

        [JsonProperty(PropertyName = "accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reports { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_likes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedLikes { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_dislikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedDislikes { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;
        #endregion

        [JsonProperty(PropertyName = "followers")]
        public long Followers { get; set; }
    }
}
