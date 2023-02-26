using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Tokket.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
        /// <summary>
        /// Returns the input string in lowercase alphanumeric format. All spaces are replaced with underscores.
        /// </summary>
        public static string ToIdFormat(this string item)
        {
            item = item?.Trim().ToLower().Replace("/", "").Replace(" ", "_").Replace("&", "and").Replace("é", "e");
            item = Regex.Replace(item, "[^0-9A-Za-z]", "");
            return item;
        }
        /// <summary>
        /// Returns the input string with the first character converted to uppercase
        /// </summary>
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Returns the input string with the first character converted to uppercase, not to conflict Tokkepedia.Tools
        /// </summary>
        public static string ToUpperCaseFirstLetter(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        /// <summary>
        /// Converts DateTime to Integer
        /// </summary>
        public static long ToUnixTime(this DateTime dateTime)
        {
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)Math.Round((dateTime - d).TotalSeconds);
        }
        /// <summary>
        /// Converts integer to DateTime
        /// </summary>
        public static DateTime ToDateTime(this long dateTime)
        {
            return dateTime.ToDateTime();
        }
        public static string ToKMB(this long num)
        {
            var sss = (num / 1000000000D).ToString("0.##B");
            if (num >= 100000000000)
                return (num / 1000000000D).ToString("0.#B");
            if (num >= 1000000000)
                return (num / 1000000000D).ToString("0.##B");
            if (num >= 100000000)
                return (num / 1000000D).ToString("0.#M");
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##M");
            if (num >= 100000)
                return (num / 1000D).ToString("0k");
            if (num >= 100000)
                return (num / 1000D).ToString("0.#k");
            if (num >= 1000)
                return (num / 1000D).ToString("0.##k");
            return num.ToString("#,0");
        }



        //
        //https://stackoverflow.com/a/63587611
        public static IEnumerable<string> SplitIntoEqualParts(string orgString, int chunkSize, bool wholeWords = true)
        {
            if (wholeWords)
            {
                List<string> result = new List<string>();
                StringBuilder sb = new StringBuilder();

                if (orgString.Length > chunkSize)
                {
                    string[] newSplit = orgString.Split(' ');
                    foreach (string str in newSplit)
                    {
                        if (sb.Length != 0)
                            sb.Append(" ");

                        if (sb.Length + str.Length > chunkSize)
                        {
                            result.Add(sb.ToString());
                            sb.Clear();
                        }

                        sb.Append(str);
                    }

                    result.Add(sb.ToString());
                }
                else
                    result.Add(orgString);

                return result;
            }
            else
                return new List<string>(Regex.Split(orgString, @"(?<=\G.{" + chunkSize + "})", RegexOptions.Singleline));
        }

        /// <summary>
        ///     Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static String UrlEncode(this String str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     Encodes a URL string using the specified encoding object.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="e">The  object that specifies the encoding scheme.</param>
        /// <returns>An encoded string.</returns>
        public static String UrlEncode(this String str, Encoding e)
        {
            return HttpUtility.UrlEncode(str, e);
        }
    }
}
