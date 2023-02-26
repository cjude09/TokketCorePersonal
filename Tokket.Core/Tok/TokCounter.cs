//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokCounter : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tokcounter";

        #region Statistics
        //Statistics

        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "users_reacted", NullValueHandling = NullValueHandling.Ignore)]
        public long? UsersReacted { get; set; } = null;

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

        [JsonProperty(PropertyName = "shares", NullValueHandling = NullValueHandling.Ignore)]
        public long? Shares { get; set; } = null;

        [JsonProperty(PropertyName = "views", NullValueHandling = NullValueHandling.Ignore)]
        public long? Views { get; set; } = null;

        //Cosmos DB Helpers
        [JsonProperty(PropertyName = "reaction_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionPartitions { get; set; } = null;
        
        #endregion
    }
}
