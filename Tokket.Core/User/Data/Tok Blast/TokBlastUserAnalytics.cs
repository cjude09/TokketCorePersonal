using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user analytics for the service. Values here change frequently but are rarely needed by the user.</summary>
    public class TokBlastUserAnalytics : BaseModel
    {
        private const string _serviceId = "tokblast";

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
        [JsonProperty("games_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesTokBlast { get; set; } = null;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsTokBlast { get; set; } = null;
        #endregion

        #region Difficulty Statistics

        #region Number of games played
        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL1TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL2TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL3TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL4TokBlast { get; set; } = null;
        #endregion

        #region Total Points earned
        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL1TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL2TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL3TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL4TokBlast { get; set; } = null;
        #endregion

        #region Total Eliminators used
        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL1TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL2TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL3TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL4TokBlast { get; set; } = null;
        #endregion

        #region Total seconds elasped across all games
        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL1TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL2TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL3TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL4TokBlast { get; set; } = null;
        #endregion



        #endregion
    }
}
