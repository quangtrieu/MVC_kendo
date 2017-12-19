using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Antlr.StringTemplate;
using BCS.Framework.Constants;

namespace BCS.Framework.Utilities
{
    public static class Utils
    {

        public static void SetValueTest(object model)
        {
            foreach (PropertyInfo p in model.GetType().GetProperties())
            {
                try
                {
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(model, p.Name + "test", null);
                    }
                    else if (p.PropertyType == typeof(bool))
                    {
                        p.SetValue(model, true, null);
                    }
                    else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                    {
                        p.SetValue(model, DateTime.Now, null);

                    }
                    else
                    {
                        p.SetValue(model, 1111111111, null);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public static string CreateLink(string link, string text)
        {
            StringBuilder str = new StringBuilder();
            str.Append("<a href ='");
            str.Append(link);
            str.Append("' >");
            str.Append(text);
            str.Append("</a>");
            return str.ToString();
        }

        public static string GetControlId(object oString)
        {
            var id = ParseUtil.GetString(oString);
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString().Replace("-", "");
            }
            return id;
        }

        public static string GenRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZqwertyuiopasdfghjklzxcvbnm";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public static string GenerateString(int length)
        {
            const string allowedCharacters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();

            var password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                password.Append(allowedCharacters[random.Next(0, allowedCharacters.Length - 1)]);
            }
            return password.ToString();
        }

        /// <summary>
        /// Copy stream to stream
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="output">Output stream</param>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        /// Get setting from AppSettings node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T));
        }

        /// <summary>
        /// Get setting from AppSettings node with default value
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default setting value</param>
        /// <returns></returns>
        public static T GetSetting<T>(string key, T defaultValue)
        {
            try
            {
                string appSetting = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(appSetting)) return defaultValue;

                return (T)Convert.ChangeType(appSetting, typeof(T), CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string GetMsg(string key, object data)
        {
            var template = new StringTemplate(GetSetting(key, string.Empty));

            template.SetAttribute("item", data);

            return template.ToString();
        }

        public static string Stringtmp(string filename, object data)
        {
            var path = HttpContext.Current.Server.MapPath("~/MailTemplates");
            var filepath = Path.Combine(path, filename);
            var result = string.Empty;
            using (var reader = new StreamReader(filepath))
            {
                string sttemplate = reader.ReadToEnd();
                var template = new StringTemplate(sttemplate);
                template.SetAttribute("item", data);
                result = template.ToString();
            }
            return result;

        }

        public static string StringTemplate(string value, object data)
        {

            var template = new StringTemplate(value);
            template.SetAttribute("item", data);
            var result = template.ToString();
            return result;
        }

        /*
         * Page Size Grid view
         */
        public static int PageSize
        {
            get
            {
                return GetSetting<int>("PageSize", 100);
            }

        }

        public static string GetDayOfWeek(int dayOfWeek)
        {
            var day = string.Empty;
            switch (dayOfWeek)
            {
                // Summary:
                ////     Indicates Sunday.
                //Sunday = 0,
                ////
                case 0:
                    day = "Sunday";
                    break;
                //// Summary:
                ////     Indicates Monday.
                //Monday = 1,
                case 1:
                    day = "Monday";
                    break;
                ////
                //// Summary:
                ////     Indicates Tuesday.
                //Tuesday = 2,
                case 2:
                    day = "Tuesday";
                    break;
                ////
                //// Summary:
                ////     Indicates Wednesday.
                //Wednesday = 3,
                case 3:
                    day = "Wednesday";
                    break;
                ////
                //// Summary:
                ////     Indicates Thursday.
                //Thursday = 4,
                case 4:
                    day = "Thursday";
                    break;
                ////
                //// Summary:
                ////     Indicates Friday.
                //Friday = 5,
                case 5:
                    day = "Friday";
                    break;
                ////
                //// Summary:
                ////     Indicates Saturday.
                //Saturday = 6,
                case 6:
                    day = "Saturday";
                    break;
            }
            return day;
        }

        public static string GetCaption(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Contains("SPACE"))
            {
                //column.Caption.Replace("__", "/").Replace("_", " ").Replace("$", "/")
                var sp = Regex.Split(str, "SPACE");
                return sp[0].Replace("__", "/").Replace("_", " ").Replace("$", "/").Replace("DOLAR", "$").Replace("PERCENT", "%").Replace("NUMBER", "");
            }
            else
            {
                return str.Replace("__", "/").Replace("_", " ").Replace("$", "/").Replace("DOLAR", "$").Replace("PERCENT", "%").Replace("NUMBER", "");
            }
        }
    }
}
