using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class AuthenticatedUser : BaseUser
    {
        [JsonProperty(PropertyName = "role", NullValueHandling = NullValueHandling.Ignore)]
        public string? Role { get; set; }

        #region Basic Admin / Intern

        [JsonProperty(PropertyName = "school", NullValueHandling = NullValueHandling.Ignore)]
        public string? School { get; set; }

        [JsonProperty(PropertyName = "first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? LastName { get; set; }

        [JsonProperty(PropertyName = "internship_start", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? InternshipStart { get; set; }

        [JsonProperty(PropertyName = "internship_referral_message", NullValueHandling = NullValueHandling.Ignore)]
        public string? InternshipReferralMessage { get; set; }

        [JsonProperty(PropertyName = "major", NullValueHandling = NullValueHandling.Ignore)]
        public string? Major { get; set; }

        [JsonProperty(PropertyName = "degree_type", NullValueHandling = NullValueHandling.Ignore)]
        public string? DegreeType { get; set; }

        [JsonProperty(PropertyName = "graduation_month", NullValueHandling = NullValueHandling.Ignore)]
        public string? GraduationMonth { get; set; }

        [JsonProperty(PropertyName = "graduation_year", NullValueHandling = NullValueHandling.Ignore)]
        public string? GraduationYear { get; set; }

        [JsonProperty(PropertyName = "graduation_month_year", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? GraduationMonthYearDateTime { get; set; }

        #endregion

        #region Referral Code

        //FullCode
        /// <summary>Code that was used, if any, during sign up. Example: AB12 </summary>
        [JsonProperty(PropertyName = "referralcode_value", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferralCodeValue { get; set; } = null;

        /// <summary>Source of the code that was used, if any, during sign up. Values: "sms" | "email" | "fb" , etc.</summary>
        [JsonProperty(PropertyName = "referralcode_source", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferralCodeSource { get; set; } = null;

        /// <summary>Code for admins who can send referrals. </summary>
        [JsonProperty(PropertyName = "admin_referralcode_value", NullValueHandling = NullValueHandling.Ignore)]
        public string AdminReferralCodeValue { get; set; } = null;

        ///<summary>Can only be "Q1", "Q2", "Q3", "Q4"</summary>
        //[JsonProperty(PropertyName = "referralcode_quarter", NullValueHandling = NullValueHandling.Ignore)]
        //public string ReferralCodeQuarter { get; set; } = null;

        ///<summary>Year i.e. 2022 </summary>
        //[JsonProperty(PropertyName = "referralcode_year", NullValueHandling = NullValueHandling.Ignore)]
        //public string ReferralCodeYear { get; set; } = null;

        #endregion

        /// <summary>
        /// Resets the user to before they were given a role. Does not delete intern/admin generated data
        /// </summary>
        public void RemoveAllRoleFields()
        {
            this.Role = null;
            this.School = null;
            this.FirstName = null;
            this.LastName = null;
            this.InternshipStart = null;
            this.InternshipReferralMessage = null;
            this.Major = null;
            this.DegreeType = null;
            this.GraduationMonth = null;
            this.GraduationYear = null;
            this.GraduationMonthYearDateTime = null;
            this.AdminReferralCodeValue = null;
        }
    }
}
