using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user analytics for the service. Values here change frequently but are rarely needed by the user.</summary>
    public class AlphaGuessUserAnalytics : BaseModel
    {
        private const string _serviceId = "alphaguess";

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

        //[JsonProperty("room_purchased_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        //public bool? IsRoomPurchasedAlphaGuess { get; set; } = null;

        #region General counts
        /// <summary>Total number of games played.</summary>
        [JsonProperty("games_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesAlphaGuess { get; set; } = null;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsAlphaGuess { get; set; } = null;
        #endregion

        #region Sequence Statistics

        #region Number of games played in the sequence
        /// <summary>Total number of games played in the sequence.</summary>
        [JsonProperty("games_standard_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesStandardAlphaGuess { get; set; } = null;

        /// <summary>Total number of games played in the sequence.</summary>
        [JsonProperty("games_reverse_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesReverseAlphaGuess { get; set; } = null;
        #endregion

        #region Total Points earned
        /// <summary>Total number of points earned in the sequence.</summary>
        [JsonProperty("points_standard_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsStandardAlphaGuess { get; set; } = null;

        /// <summary>Total number of points earned in the sequence.</summary>
        [JsonProperty("points_reverse_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsReverseAlphaGuess { get; set; } = null;
        #endregion

        #region Total Eliminators used
        /// <summary>Total number of eliminators used in the sequence.</summary>
        [JsonProperty("eliminators_standard_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsStandardAlphaGuess { get; set; } = null;

        /// <summary>Total number of eliminators used in the sequence.</summary>
        [JsonProperty("eliminators_reverse_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsReverseAlphaGuess { get; set; } = null;
        #endregion

        #region Total seconds elasped across all games
        /// <summary>Total number of seconds elasped when playing in the sequence.</summary>
        [JsonProperty("seconds_standard_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsStandardAlphaGuess { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the sequence.</summary>
        [JsonProperty("seconds_reverse_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsReverseAlphaGuess { get; set; } = null;
        #endregion



        #endregion
    }
}
