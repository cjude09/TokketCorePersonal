//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    public class DailyBonusResponse : BaseModel
    {
        //id: {userid}-tokblitz-dailybonus
        //pk: {userid}

        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "dailybonusresponse";

        /// <summary>Record id: id of the DailyBonusRecord</summary>
        [JsonProperty(PropertyName = "record_id")]
        public string RecordId { get; set; }

        /// <summary>Id of the user</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "";

        /// <summary>True if more than 24 hours has passed</summary>
        [JsonProperty(PropertyName = "is_rewarded")]
        public bool IsRewarded { get; set; } = false;

        /// <summary>True if the check was successful (no code errors)</summary>
        [JsonProperty(PropertyName = "is_checked")]
        public bool IsChecked { get; set; } = false;

        /// <summary>Message to display</summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = null;

        /// <summary>DateTime where daily bonus is applied</summary>
        [JsonProperty(PropertyName = "daily_reset", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime DailyReset { get; set; }

        /// <summary>Hours left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "hours_left", NullValueHandling = NullValueHandling.Ignore)]
        public int? HoursLeft { get; set; } = null;

        /// <summary>Minutes left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "minutes_left", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinutesLeft { get; set; } = null;
    }
}
