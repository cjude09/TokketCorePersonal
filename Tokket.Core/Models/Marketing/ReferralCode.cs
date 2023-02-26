using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class ReferralCode : BaseModel
    {

        [JsonProperty(PropertyName = "label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; } = "referralcode";

        //FullCode
        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; } = null;

        ///<summary>Id of the admin this referral code belongs to</summary>
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; } = null;

        [JsonProperty(PropertyName = "next_link_number", NullValueHandling = NullValueHandling.Ignore)]
        public int NextLinkNumber { get; set; } = 1;
    }

    public class ReferralCodeRecord : ReferralCode
    {
        ///<summary>Owner ID of the admin that created the code.</summary>
        [JsonProperty(PropertyName = "owner_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerId { get; set; } = null;
    }
}
