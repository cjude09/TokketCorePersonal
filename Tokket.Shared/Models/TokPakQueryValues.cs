using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;

namespace Tokket.Shared.Models
{
    public class TokPakQueryValues
    {
        public int limit = 20;
        public string kind = "";
        public string itemid = "";
        public string groupid = "";
        public string userid = "";
        public string paginationid = null;
        public string partitionkeybase = "";
        public long? itemtotal = 0;
        public string tokpaktype = "";
        public bool? publicfeed { get; set; }

        #region Search
        public bool? startswith { get; set; }
        public string text { get; set; }
        #endregion
    }
}
