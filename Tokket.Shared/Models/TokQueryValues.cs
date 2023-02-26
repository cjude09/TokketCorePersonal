using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    /// <summary>
    /// Values for querying toks.
    /// </summary>
    public class TokQueryValues
    {
        public string order { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string category { get; set; }
        public string tokgroup { get; set; }
        public string toktype { get; set; }
        public string userid { get; set; }
        public string itemid { get; set; }
        public string loadmore { get; set; }
        public string token { get; set; }
        public string streamtoken { get; set; }
        public string detailnumber { get; set; } = "-1";
        public string offset { get; set; }
        public string toktotal { get; set; }
        public bool? image { get; set; }
        public bool? video { get; set; }
        public string tagid { get; set; }

        public string eventstarttimedayofweek { get; set; } = "0"; //Sunday

        public string sortby { get; set; } = "standard";

        #region Search
        public bool? startswith { get; set; }
        public string text { get; set; }

        public bool? casesensitive { get; set; }

        //Search
        public string searchkey { get; set; } = null;
        public string searchvalue { get; set; } = null;
        #endregion

        public string serviceid = "tokkepedia";
        public string itemsbase = "toks";
        public string groupid { get; set; }

        public string itemssuffix { get; set; }

        #region YearBook
        public string yearbook_type { get; set; } = "";
        public string yearbook_schoolname { get; set; } = "";
        public DateTime yearbook_graduationmonthyear { get; set; }
        public string yearbook_grouptype { get; set; } = "";
        public string yearbook_timing { get; set; } = "";
        public string yearbook_tiletype { get; set; } = "";
        #endregion

        #region Opportunities
        public string opportunity_type { get; set; } = "";
        public string application_deadline { get; set; }
        public string amount { get; set; }
        public string awards_available { get; set; }
        public string description { get; set; } = "";

        public string email_address { get; set; } = "";

        public string address { get; set; } = "";

        public string phone_number { get; set; } = "";
        public string about_company { get; set; } = "";
        public string requirements { get; set; } = "";
        public string website { get; set; } = "";

        public string pagination_id { get; set; } = "";
        #endregion

        public string training_tok { get; set; }
        public string training_type { get; set; } = "";

        #region Program
        public string outline { get; set; }
        public string cost { get; set; }
        public string schedule { get; set; }
        public string level { get; set; }
        public string about { get; set; }
        public string duration { get; set; }
        public string relatedskills { get; set; }
        public string students { get; set; }
        public string projects { get; set; }
        public string assignments { get; set; }
        public string instructorinfo { get; set; }

        #endregion

        #region Tutor
        public string educaion { get; set; }
        public string experience { get; set; }
        #endregion
    }
}
