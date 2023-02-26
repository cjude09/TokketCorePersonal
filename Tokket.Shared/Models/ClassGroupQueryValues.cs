using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class ClassGroupQueryValues
    {
        public int limit = 20;
        public string kind = "";
        public string itemid = "";
        public string userid = "";
        public string paginationid = null;
        public string partitionkeybase = "";
        public long? itemtotal = 0;
        public bool joined;
        public string StringJoined = "";
        public bool? showImage = null; // Both = null, Image = true, Non-Image = false
        public bool? isDescending = null;
        public string groupkind = "";
        #region Search
        public bool? startswith { get; set; }
        public string text { get; set; }

        public string level0 = "";
        public string level1 = "";
        public string level2 = "";
        public string level3 = "";
        public string level4 = "";

        #endregion
    }
}