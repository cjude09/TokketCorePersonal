using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class CategoryModel : Category
    {
        [JsonProperty("classtoks")]
        public long ClassToks { get; set; }
        public List<string> categoryList { get; set; } = new List<string>();
        public string filterCategory { get; set; }
        public int toks_count { get; set; }
    }
}
