using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;

namespace Tokket.Shared.Models
{
    public class ClassTokQueryValues
    {
        public int limit = 20;
        public string kind = "";
        public string groupid = "";
        public string userid = "";
        public string paginationid = null;
        public string partitionkeybase = "";
        public string toktypeid = "";
        public string classsetid = "";
        public long? itemtotal = 0;

        public string category { get; set; }
        public string tokgroup { get; set; }
        public string toktype { get; set; }

        public bool? startswith { get; set; }

        public bool? casesensitive { get; set; }
        public string text { get; set; }

        public bool? publicfeed { get; set; }

        //Search
        public string searchkey { get; set; } = null;
        public string searchvalue { get; set; } = null;

        #region Filter By
        public FilterBy FilterBy = FilterBy.None;
        public bool RecentOnly = true; // Alphabetical Order if false.
        public List<string> FilterItems = new List<string>();
        #endregion

        #region TokChannels
        public string level1 { get; set; }
        public string level2 { get; set; }
        public string level3 { get; set; }
        #endregion


        public bool? classtokmode { get; set; }

        public bool? image { get; set; }
        public string loadmore { get; set; }

        public string orderby { get; set; } = null;
        public bool descending { get; set; } = true;
        #region Tok Share
        public string tokshare { get; set; } = null;
        public string toksharepk { get; set; } = null;
        #endregion
    }
}
