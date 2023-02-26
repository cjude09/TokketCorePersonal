using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;

namespace Tokket.Shared.Models
{
    public class ClassGroupRequestQueryValues
    {
        public int limit = 20;
        public string kind = "";
        public string userid = "";
        public string receiverid = "";
        public string senderid = "";
        public string groupid = "";
        public bool isselfincluded;
        public RequestStatus status = RequestStatus.All;
        public string paginationid = null;
        public string partitionkeybase = "";
        public long? itemtotal = 0;
    }
}
