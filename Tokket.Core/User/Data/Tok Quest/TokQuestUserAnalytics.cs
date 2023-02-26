using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user analytics for the service. Values here change frequently but are rarely needed by the user.</summary>
    public class TokQuestUserAnalytics : BaseModel
    {
        private const string _serviceId = "tokquest";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}useranalytics";

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
        /// <summary>Total number of games played.</summary>
        [JsonProperty("games_tokquest", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesTokQuest { get; set; } = 0;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_tokquest", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsTokQuest { get; set; } = 0;
        #endregion

        
    }
}
