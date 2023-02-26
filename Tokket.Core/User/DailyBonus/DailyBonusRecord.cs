//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Contains all methods for tracking a user's daily bonus.
    /// </summary>
    public class DailyBonusRecord : BaseModel
    {
        //id: {userid}-{serviceId}-dailybonus
        //pk: {userid}

        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "dailybonusrecord";

        /// <summary>Id of the user</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "";

        /// <summary>The first day and time the user used the service. In other words, the date the user checks the service for the first time. Same as CreatedTime</summary>
        [JsonProperty(PropertyName = "first_date")]
        public DateTime FirstDate { get; set; }

        /// <summary>The most recent day and time the user used the service</summary>
        [JsonProperty(PropertyName = "recent_date")]
        public DateTime RecentDate { get; set; }

        /// <summary>Days in a row</summary>
        [JsonProperty(PropertyName = "days_consecutive")]
        public long ConsecutiveDays { get; set; } = 0;

        /// <summary>Days total</summary>
        [JsonProperty(PropertyName = "days_cumulative")]
        public long CumulativeDays { get; set; } = 0;

        /// <summary>Longest daily bonus streak</summary>
        [JsonProperty(PropertyName = "longest_consecutive")]
        public long ConsecutiveLongest { get; set; } = 0;
    }
}
