using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user counts for the service. Values here change frequently and are needed by the user.</summary>
    public class TokBlastUserCounter : BaseModel
    {
        private const string _serviceId = "tokblast";

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

        #region General counts
        /// <summary>Total number of eliminators owned.</summary>
        [JsonProperty("eliminators_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_tokblast_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlastDeleted { get; set; } = null;

        /// <summary>Total number of revealers owned.</summary>
        [JsonProperty("revealers_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? RevealersTokBlast { get; set; } = null;

        /// <summary>Total number of revealers used.</summary>
        [JsonProperty("revealers_tokblast_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? RevealersTokBlastDeleted { get; set; } = null;
        #endregion

    }
}
