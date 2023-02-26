using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core.Analytics
{
    public class GetClassToksAnalyticsModel
    {
        public int Count { get; set; } = 0;
        public string Feed { get; set; } = ""; //Public | My | TokChannel
        public string Group { get; set; } = "";
        public DateTime TimeElapsed { get; set; } = DateTime.Now;
    }
}
