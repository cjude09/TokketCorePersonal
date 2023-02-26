using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user counts for the service. Values here change frequently and are needed by the user.</summary>
    public class TokQuestUserCounter : BaseModel
    {
        private const string _serviceId = "tokquest";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}usercounter";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>Total number of eliminators owned.</summary>
        [JsonProperty("eliminators_tokquest", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokQuest { get; set; } = 7;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_tokquest_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokQuestDeleted { get; set; } = null;
    }
}
