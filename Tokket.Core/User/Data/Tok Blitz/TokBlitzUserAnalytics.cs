using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user analytics for the service. Values here change frequently but are rarely needed by the user.</summary>
    public class TokBlitzUserAnalytics : BaseModel
    {
        private const string _serviceId = "tokblitz";

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
        [JsonProperty("games_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesTokBlitz { get; set; } = 0;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsTokBlitz { get; set; } = 0;
        #endregion

        #region Difficulty Statistics

        #region Number of games played
        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL1TokBlitz { get; set; } = 0;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL2TokBlitz { get; set; } = 0;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL3TokBlitz { get; set; } = 0;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL4TokBlitz { get; set; } = 0;
        #endregion

        #region Total Points earned
        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL1TokBlitz { get; set; } = 0;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL2TokBlitz { get; set; } = 0;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL3TokBlitz { get; set; } = 0;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL4TokBlitz { get; set; } = 0;
        #endregion

        #region Total Eliminators used
        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL1TokBlitz { get; set; } = 0;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL2TokBlitz { get; set; } = 0;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL3TokBlitz { get; set; } = 0;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL4TokBlitz { get; set; } = 0;
        #endregion

        #region Total seconds elasped across all games
        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL1TokBlitz { get; set; } = 0;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL2TokBlitz { get; set; } = 0;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL3TokBlitz { get; set; } = 0;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL4TokBlitz { get; set; } = 0;
        #endregion

        #endregion
    }
}
