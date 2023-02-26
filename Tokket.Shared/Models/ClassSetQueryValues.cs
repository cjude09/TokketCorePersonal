using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class ClassSetQueryValues
    {
        public int limit = 20;
        public string kind = "";
        public string itemid = "";
        public string userid = "";
        public string groupid = "";
        public string paginationid = null;
        public string partitionkeybase = "";
        public long? itemtotal = 0;
        public string label = "classset";
        #region Search
        public bool? startswith { get; set; }
        public string text { get; set; }
        public string name { get; set; }
        public string category { get; set; }

        public bool? publicfeed { get; set; }
        #endregion
    }
}