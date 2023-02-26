using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Helpers
{
    public class SetQueryValues
    {
        public string order { get; set; }
        public string text { get; set; }
        public string userid { get; set; }
        public string loadmore { get; set; }
        public string token { get; set; }
        public int offset { get; set; } = 25; //MaxItemCount, default is 25
        public string toktypeid { get; set; }
        public string gamename { get; set; } = "tokblitz";
    }
}
