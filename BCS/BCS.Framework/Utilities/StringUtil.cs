using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Antlr.StringTemplate;

namespace BCS.Framework.Utilities
{
    public static class StringUtil
    {
        public static string SubLeft(string str, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = str.Substring(0, length);
            //return the result of the operation
            return result;
        }

        public static string SubLeft(string str, string ch)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable

            string result = str.Substring(0, str.IndexOf(ch, System.StringComparison.Ordinal));
            //return the result of the operation
            return result;
        }

        public static string SubRight(string str, string ch)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = str.Substring(str.Length - str.IndexOf(ch, System.StringComparison.Ordinal));
            //return the result of the operation
            return result;
        }

        public static string SubMid(string str, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = str.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string SubMid(string str, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = str.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static string StringTemp(string filename, object data)
        {
            var path = HttpContext.Current.Server.MapPath("~/");
            var filepath = Path.Combine(path, filename);
            var result = string.Empty;
            using (StreamReader reader = new StreamReader(filepath))
            {
                string sttemplate = reader.ReadToEnd();
                StringTemplate template = new StringTemplate(sttemplate);
                template.SetAttribute("item", data);
                result = template.ToString();
            }
            return result;
        }

        public static string StringTemplate(string str, object data)
        {
            var template = new StringTemplate(str);
            template.SetAttribute("item", data);
            return template.ToString();
        }

        public static string GetSubLeft(string str, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = str.Substring(0, length);
            //return the result of the operation
            return result;
        }

        public static string GetSubLeft(string str, string ch)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable

            string result = str.Substring(0, str.IndexOf(ch, System.StringComparison.Ordinal));
            //return the result of the operation
            return result;
        }

        public static string GetSubRight(string str, string ch)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = str.Substring(str.Length - str.IndexOf(ch, System.StringComparison.Ordinal));
            //return the result of the operation
            return result;
        }

        public static string GetSubMid(string str, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = str.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string GetSubMid(string str, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = str.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static string RemoveHtmlTags(string source)
        {
            string expn = "<.*?>";
            return Regex.Replace(source, expn, string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveScriptTag(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return Regex.Replace(text, "<script.*?<" + "/script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
