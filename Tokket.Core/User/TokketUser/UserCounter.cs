//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class UserCounter
    {
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

        #region Tok Blitz
        [JsonProperty("tokblitz_strikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrikesTokBlitz { get; set; } = null;

        [JsonProperty("tokblitz_deleted_strikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedStrikesTokBlitz { get; set; } = null;

        [JsonProperty("tokblitz_saved", NullValueHandling = NullValueHandling.Ignore)]
        public long? SavedTokBlitz { get; set; } = null;

        [JsonProperty("eliminators_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitz { get; set; } = null;

        [JsonProperty("eliminators_tokblitz_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitzDeleted { get; set; } = null;


        #endregion



        #region Reactions
        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedReactions { get; set; } = null;

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

        [JsonProperty(PropertyName = "reaction_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionPartitions { get; set; } = null;

        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "usercounter";

        [JsonRequired]
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
