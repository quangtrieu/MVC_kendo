using System;
using System.Globalization;

namespace BCS.Framework.Utilities
{
    public class ParseUtil
    {
        public static int? GetInt(object oInt)
        {
            if (oInt == null)
            {
                return null;
            }
            try
            {
                return int.Parse(oInt.ToString());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static decimal GetDecimal(object oDecimal)
        {
            if (oDecimal == null)
            {
                return 0;
            }
            try
            {
                return decimal.Parse(oDecimal.ToString());
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static float GetFloat(object oFloat)
        {
            if (oFloat == null)
            {
                return 0;
            }
            try
            {
                return float.Parse(oFloat.ToString());
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static double? GetDouble(object oDouble)
        {
            if (oDouble == null)
            {
                return null;
            }
            try
            {
                return double.Parse(oDouble.ToString());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool GetBool(object oBool)
        {
            if (oBool == null)
            {
                return false;
            }
            try
            {
                return bool.Parse(oBool.ToString());
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetString(object oText)
        {
            if (oText == null)
            {
                return string.Empty;
            }
            try
            {
                return oText.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string Replace(object baseText, object replaceText, object byText)
        {
            if (baseText == null || replaceText == null || byText == null||baseText.ToString().Length == 0 || replaceText.ToString().Length == 0)
            {
                return string.Empty;
            }
            try
            {
                return baseText.ToString().Replace(replaceText.ToString(), byText.ToString());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static DateTime? GetDateTime(object oDateTime)
        {
            if (oDateTime == null)
            {
                return null;
            }
            try
            {
                return DateTime.Parse(GetString(oDateTime));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DateTime? TryParseDate(String date)
        {
            try
            {
                DateTime dateTimeResult;
                if ((DateTime.TryParseExact(date, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult) == true) || (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult) == true))
                {
                    return dateTimeResult;
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

        public static TimeSpan TryParseTimeSpan(string timespanStr)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(timespanStr,
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                return dateTime.TimeOfDay;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static TimeSpan TryParseTimeSpan(DateTime timespanStr)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(timespanStr.TimeOfDay.ToString(),
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                return dateTime.TimeOfDay;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Guid? TryParseGuid(string guidText)
        {
            try
            {
                var output = new Guid();
                Guid.TryParse(guidText, out output);
                return output;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
