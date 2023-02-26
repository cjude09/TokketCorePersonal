using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Tokket.Core.Tools
{
    public static class RandomExtensions<T>
    {
        static Random _random = new Random();

        public static T[] Randomize(T[] arr)
        {
            List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>>();
            // Add all strings from array
            // Add new random int each time
            foreach (T s in arr)
            {
                list.Add(new KeyValuePair<int, T>(_random.Next(), s));
            }
            // Sort the list by the random number
            var sorted = from item in list
                            orderby item.Key
                            select item;
            // Allocate new string array
            T[] result = new T[arr.Length];
            // Copy values to array
            int index = 0;
            foreach (KeyValuePair<int, T> pair in sorted)
            {
                result[index] = pair.Value;
                index++;
            }
            // Return copied array
            return result;
        }
    }
}
