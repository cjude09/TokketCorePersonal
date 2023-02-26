//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Tokkepedia Counters that are generally private.</summary>
    public class TokkepediaUserCounterPersonal : BaseModel
    {
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}usercounterpersonal";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>"user", "subaccount"</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        #region Class Toks
        [JsonProperty("classtoks", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassToks { get; set; } = null;

        [JsonProperty("deleted_classtoks", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedClassToks { get; set; } = null;

        [JsonProperty("classsets", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassSets { get; set; } = null;

        [JsonProperty("deleted_classsets", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedClassSets { get; set; } = null;

        /// <summary>Number of class groups the user owns.</summary>
        [JsonProperty("classgroups", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassGroups { get; set; } = null;

        /// <summary>Number of class groups the user deleted.</summary>
        [JsonProperty("deleted_classgroups", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedClassGroups { get; set; } = null;

        [JsonProperty("classgroupinvitations", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassGroupInvitations { get; set; } = null;

        [JsonProperty("deleted_classgroupinvitations", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedClassGroupInvitations { get; set; } = null;

        [JsonProperty("classgroupsjoined", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassGroupsJoined { get; set; } = null;

        [JsonProperty("deleted_classgroupsjoined", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedClassGroupsJoined { get; set; } = null;
        #endregion

        #region Reactions
        [JsonProperty(PropertyName = "reaction_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionScore { get; set; } = null;

        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedReactions { get; set; } = null;

        [JsonProperty(PropertyName = "reaction_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionPartitions { get; set; } = null;

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reports { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gema", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemA { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemb", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemB { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemc", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemC { get; set; } = null;

        [JsonProperty(PropertyName = "accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        /// <summary>Number of treasures.</summary>
        [JsonProperty(PropertyName = "treasure", NullValueHandling = NullValueHandling.Ignore)]
        public long? Treasures { get; set; } = null;
        #endregion
    }
}
