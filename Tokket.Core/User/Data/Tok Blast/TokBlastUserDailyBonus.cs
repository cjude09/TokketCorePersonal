using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user's daily bonus for the service.</summary>
    public class TokBlastUserDailyBonus : DailyBonusRecord
    {
        [JsonIgnore]
        private const string _serviceId = "tokblast";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public new string Label { get; set; } = $"{_serviceId}userdailybonus";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public new string ServiceId { get; set; } = _serviceId;
    }
}