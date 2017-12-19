using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BCS.Framework.Utilities
{
    public static  class ConvertUtil
    {
        public static int ToInt32(object obj)
        {
            int retVal = 0;

            try
            {
                retVal = Convert.ToInt32(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static int ToInt32(string obj)
        {
            int retVal = 0;

            try
            {
                retVal = Convert.ToInt32(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static int ToInt32(object obj, int defaultValue)
        {
            int retVal;

            try
            {
                retVal = Convert.ToInt32(obj);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        public static long ToInt64(object obj)
        {
            long retVal = 0;

            try
            {
                retVal = Convert.ToInt64(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static long ToInt64(string obj)
        {
            long retVal = 0;

            try
            {
                retVal = Convert.ToInt64(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static long ToInt64(object obj, int defaultValue)
        {
            long retVal;

            try
            {
                retVal = Convert.ToInt64(obj);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        public static string ToString(object obj)
        {
            string retVal;

            try
            {
                retVal = Convert.ToString(obj);
            }
            catch
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        public static bool ToBoolean(object obj)
        {
            bool retVal;

            try
            {
                retVal = Convert.ToBoolean(obj);
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        public static double ToDouble(object obj)
        {
            double retVal;

            try
            {
                retVal = Convert.ToDouble(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static double ToDouble(object obj, double defaultValue)
        {
            double retVal;

            try
            {
                retVal = Convert.ToDouble(obj);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

       
        public static decimal ToDecimal(object obj,int round)
        {
            decimal retVal = 0;

            try
            {
                retVal = Convert.ToDecimal(obj);
            }
            catch
            {
                retVal = 0;
            }

            return Math.Round(Convert.ToDecimal(retVal), round); ;
        }

        public static decimal ToDecimal(object obj)
        {
            decimal retVal = 0;

            try
            {
                retVal = Convert.ToDecimal(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static string ToDecimals(object obj)
        {
            try
            {
                return obj != null ? Convert.ToDecimal(obj).ToString() : "NULL";
            }
            catch
            {
                return "NULL";
            }
        }

        public static string ToDateTime()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static string ConvertToLink(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Regex reg = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$");
                if (reg.IsMatch(text))
                {
                    return text;
                }
                else
                {
                    return "http://" + text;
                }
            }
            return string.Empty;
        }

        public static DateTime ToDateTime(object obj)
        {
            DateTime retVal;
            try
            {
                retVal = Convert.ToDateTime(obj);
            }
            catch
            {
                retVal = DateTime.Now;
            }
            if (retVal == new DateTime(1, 1, 1)) return DateTime.Now;

            return retVal;
        }

        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            DateTime retVal;
            try
            {
                retVal = Convert.ToDateTime(obj);
            }
            catch
            {
                retVal = DateTime.Now;
            }
            if (retVal == new DateTime(1, 1, 1)) return defaultValue;

            return retVal;
        }

        public static DateTime? ToDateTimes(object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? TryParseDateTimes(string obj)
        {
            try
            {
                DateTime dateTime;
                DateTime.TryParse(obj, out dateTime);
                return dateTime;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert DateTime To UTC DateTime
        /// </summary>
        /// <param name="str"></param>
        public static string ToUtcDateTime(string str)
        {

            string val = string.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    DateTime reval = Convert.ToDateTime(str).ToUniversalTime();

                    val = reval.ToString();
                }
                catch (Exception)
                {
                    val = string.Empty;
                }
            }

            return val;
        }

        /// <summary>
        /// Convert DateTime To Local DateTime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLocalDateTime(string str)
        {
            string val = string.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    DateTime reval = Convert.ToDateTime(str).ToLocalTime();

                    val = string.Format("{0:yyyy/MM/dd}", reval);
                }
                catch (Exception)
                {
                    val = string.Empty;
                }
            }

            return val;

        }

        /// <summary>
        /// Convert String toDate
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? StringToDate(String date)
        {
            try
            {
                DateTime dateTemp;
                if (DateTime.TryParse(date, out dateTemp))
                {
                    return dateTemp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Convert DateTime To UTC DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToUtcDateTime(DateTime? dateTime)
        {
            try
            {
                return dateTime.HasValue ? (DateTime?) dateTime.Value.ToUniversalTime() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Convert DateTime To Local DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToLocalDateTime(DateTime? dateTime)
        {
            try
            {
                return dateTime.HasValue ? (DateTime?) dateTime.Value.ToLocalTime() : null;
            }
            catch (Exception)
            {
                return null; ;
            }

        }

        /// <summary>
        /// Convert DateTime To UTC DateTime
        /// </summary>
        /// <param name="str"></param>
        public static string ToUtcDateTimeStr(string str)
        {
            string val = string.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    DateTime reval = System.Convert.ToDateTime(str).ToUniversalTime();

                    val = reval.ToString();
                }
                catch (Exception)
                {
                    val = string.Empty;
                }
            }

            return val;
        }

        /// <summary>
        /// Convert DateTime To Local DateTime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLocalDateTimeStr(string str)
        {
            string val = string.Empty;

            if (!String.IsNullOrEmpty(str))
            {
                try
                {
                    DateTime reval = System.Convert.ToDateTime(str).ToLocalTime();

                    val = string.Format("{0:yyyy/MM/dd}", reval);
                }
                catch (Exception)
                {
                    val = String.Empty;
                }
            }

            return val;

        }

        public static string ObjectToStringJson(object json)
        {
            return JsonConvert.SerializeObject(json, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
