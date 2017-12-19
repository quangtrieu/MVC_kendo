using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace BCS.Framework.Commons
{
    /// <summary>
    /// Utility class
    /// </summary>
    public static class Utils
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToShortSentence(this string text, int length)
        {
            if (string.IsNullOrEmpty(text.StripTag())) return string.Empty;

            if (text.StripTag().Length > length)
            {
                text = text.StripTag().Substring(0, length) + "...";
            }

            return text;
        }

        public static int PageSize()
        {
            return 20;
            //return Provider.Resolve<IConfigurationManager>().Get(Constant.ITEM_PER_PAGE, 30);
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
    }
}