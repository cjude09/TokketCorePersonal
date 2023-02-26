using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class TokkepediaCounter
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "tokkepediacounter";

        [JsonProperty("pk")]
        public string PartitionKey { get; set; } = "tokkepediacounter";

        [JsonProperty("label")]
        public string Label { get; set; } = "tokkepediacounter";

        [JsonProperty("toks", NullValueHandling = NullValueHandling.Ignore)]
        public long? Toks { get; set; } = null;

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; } = null;

        [JsonProperty("coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; } = null;

        [JsonProperty("strikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Strikes { get; set; } = null;

        [JsonProperty("sets", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sets { get; set; } = null;

        [JsonProperty("deleted_toks", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedToks { get; set; } = null;

        [JsonProperty("deleted_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedPoints { get; set; } = null;

        [JsonProperty("deleted_coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCoins { get; set; } = null;

        [JsonProperty("deleted_strikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedStrikes { get; set; } = null;

        [JsonProperty("deleted_sets", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedSets { get; set; } = null;

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

        [JsonProperty(PropertyName = "followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? Followers { get; set; } = null;

        [JsonProperty(PropertyName = "following", NullValueHandling = NullValueHandling.Ignore)]
        public long? Following { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedFollowers { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_following", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedFollowing { get; set; } = null;
        #endregion

        [JsonProperty("_rid")]
        public string Rid { get; set; }

        [JsonProperty("_self")]
        public string Self { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }

        [JsonProperty("_attachments")]
        public string Attachments { get; set; }

        [JsonProperty("_ts")]
        public long Ts { get; set; }
    }
}
