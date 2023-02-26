using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class MarketingUserCounter : BaseModel
    {
        //Id format: marketingusercounter-{userid}-1-1-2000
        //Pk: {userid}-marketingusercounter

        private const string _serviceId = "marketing";

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

        /// <summary>"user", "subaccount"</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("month", NullValueHandling = NullValueHandling.Ignore)]
        public int? Month { get; set; } = null;

        [JsonProperty("day", NullValueHandling = NullValueHandling.Ignore)]
        public int? Day { get; set; } = null;

        [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
        public int? Year { get; set; } = null;

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Date { get; set; } = null;

        [JsonProperty("sent_sms", NullValueHandling = NullValueHandling.Ignore)]
        public long? SentSms { get; set; } = null;

        [JsonProperty("opened_sms", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpenedSms { get; set; } = null;

        [JsonProperty("created_sms", NullValueHandling = NullValueHandling.Ignore)]
        public long? CreatedSms { get; set; } = null;

        [JsonProperty("verified_sms", NullValueHandling = NullValueHandling.Ignore)]
        public long? VerifiedSms { get; set; } = null;

        [JsonProperty("sent_email", NullValueHandling = NullValueHandling.Ignore)]
        public long? SentEmail { get; set; } = null;

        [JsonProperty("opened_email", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpenedEmail { get; set; } = null;

        [JsonProperty("created_email", NullValueHandling = NullValueHandling.Ignore)]
        public long? CreatedEmail { get; set; } = null;

        [JsonProperty("verified_email", NullValueHandling = NullValueHandling.Ignore)]
        public long? VerifiedEmail { get; set; } = null;

        [JsonProperty("sent_fb", NullValueHandling = NullValueHandling.Ignore)]
        public long? SentFb { get; set; } = null;

        [JsonProperty("opened_fb", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpenedFb { get; set; } = null;

        [JsonProperty("created_fb", NullValueHandling = NullValueHandling.Ignore)]
        public long? CreatedFb { get; set; } = null;

        [JsonProperty("verified_fb", NullValueHandling = NullValueHandling.Ignore)]
        public long? VerifiedFb { get; set; } = null;

        public void InitializeAllCounts()
        {
            SentSms = 0;
            OpenedSms = 0;
            CreatedSms = 0;
            VerifiedSms = 0;
            SentEmail = 0;
            OpenedEmail = 0;
            CreatedEmail = 0;
            VerifiedEmail = 0;
            SentFb = 0;
            OpenedFb = 0;
            CreatedFb = 0;
            VerifiedFb = 0;
        }

        public void InitializeSampleCounts()
        {
            SentSms = 1;
            OpenedSms = 2;
            CreatedSms = 2;
            SentEmail = 2;
            OpenedEmail = 2;
            CreatedEmail = 2;
            SentFb = 2;
            OpenedFb = 2;
            CreatedFb = 2;
        }

        #region Collection fields

        [JsonProperty("counters", NullValueHandling = NullValueHandling.Ignore)]
        public List<MarketingUserCounter> Counters { get; set; } = null;

        #endregion

        #region UI specific

        [JsonIgnore]
        public bool IsVisible { get; set; } = false;

        [JsonIgnore]
        public int WeekNumber { get; set; } = 0;

        [JsonIgnore]
        public string Text { get; set; } = "";

        #endregion
    }
}
