using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class UserQueryValues
    {
        public string accounttype { get; set; } = null;
        public string displayname { get; set; } = null;

        public string pagination_id { get; set; } = null;

        public bool startswith { get; set; } = false;
        public bool displaynameexact { get; set; } = false;

        public int limit { get; set; }
        public int offset { get; set; }
        public bool loadmore { get; set; } = true;
        public string token { get; set; }
    }
}
