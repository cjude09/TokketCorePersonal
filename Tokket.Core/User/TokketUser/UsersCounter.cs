using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class UsersCounter
    {
        public string id { get; set; } = "userscounter";
        public long users { get; set; }
        public long deleted_users { get; set; }
        public string pk { get; set; } = "userscounter";
    }
}
