using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Notification
{
    public class NotificationQueryValues
    {
        public int limit = 20;
        public string pagination_id = "";
        public string userid = "";
        public string token = "";
        public string streamtoken = "";
    }
}
