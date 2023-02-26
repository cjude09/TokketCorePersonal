using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Helpers
{
    public class DataConvert
    {
        public static T ConvertDictionaryTo<T>(Dictionary<string, string> lem) where T : new()
        {
            T t = new T();

            foreach (var p in typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (lem.TryGetValue(p.Name, out string v))
                {
                    p.SetValue(t, v);
                }
            }

            return t;
        }

        public static T ConvertDictionaryTo<T>(Dictionary<string, object> lem) where T : new()
        {
            T t = new T();

            foreach (var p in typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (lem.TryGetValue(p.Name, out object v))
                {
                    p.SetValue(t, v);
                }
            }

            return t;
        }

      
    }
}
