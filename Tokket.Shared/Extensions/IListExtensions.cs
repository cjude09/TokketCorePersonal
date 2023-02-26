using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Shared.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        ///     Shuffles the element order of the specified list.
        /// </summary>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid() + "-" + new Random().Next(100000, 999999).ToString()).ToList();
        }
    }
}
