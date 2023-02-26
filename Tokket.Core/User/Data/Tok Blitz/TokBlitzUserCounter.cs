using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user counts for the service. Values here change frequently and are needed by the user.</summary>
    public class TokBlitzUserCounter : BaseModel
    {
        private const string _serviceId = "tokblitz";

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
        [JsonProperty("eliminators_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitz { get; set; } = 12;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_tokblitz_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitzDeleted { get; set; } = 0;
    }
}
