using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BCS.Framework.Commons
{
    /// <summary>
    /// Some common and utitlity extensions
    /// </summary>
    public static class HelperExtension
    {
        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML tag from string with compiled Regex.
        /// </summary>
        public static string StripTag(this string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            return HttpUtility.HtmlDecode(_htmlRegex.Replace(source, string.Empty).Trim(' ', '\t', '\r', '\n'));
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> list, params T[] values)
        {
            foreach (T value in values)
                yield return value;

            foreach (T value in list)
                yield return value;
        }

        public static string Label(this string text, string label)
        {
            return string.Format("<b>{0}:</b> {1}", label, text);
        }

        public static string Label(this string text, string label, int tab)
        {
            string tabString = string.Empty;

            for (int i = 0; i < tab; i++)
            {
                tabString += "&nbsp;&nbsp;";
            }
            return string.Format("{0}<b>{1}:</b> {2}", tabString, label, text);
        }

        public static string Count(this string text, int count)
        {
            return string.Format("{0} ({1})", text, count);
        }

        public static string ToText<T>(this IEnumerable<T> list, string sperate)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (T text in list)
            {
                if (isFirst)
                {
                    sb.Append(text);
                    isFirst = false;
                }
                else
                    sb.Append(sperate + " " + text);
            }

            return sb.ToString();
        }
    }
}