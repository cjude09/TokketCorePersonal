using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> ToDictionary<T>(this T item) => JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(item));
    }
}
