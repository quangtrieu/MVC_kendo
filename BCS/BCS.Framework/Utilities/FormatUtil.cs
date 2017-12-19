using System;
using System.Globalization;

namespace BCS.Framework.Utilities
{
    public static class FormatUtil
    {
        public static string GetDateTimeFormat(DateTime? datetime, string formatString = "{0:MM/dd/yyyy hh:mm tt}")
        {
            return datetime.HasValue
                ? string.Format(formatString, datetime.Value)
                : string.Empty;
        }

        public static string GetDateFormat(DateTime? datetime)
        {
            return datetime.HasValue ? string.Format("{0:MM/dd/yyyy}", datetime.Value) : string.Empty;
        }

        public static string FormatDate(DateTime? datetime)
        {
            return datetime.HasValue ? string.Format("{0:MM-dd-yyyy}", datetime.Value) : string.Empty;
        }

        public static string GetTimeFormat(DateTime? datetime)
        {
            return datetime.HasValue ? string.Format("{0:hh:mm tt}", datetime.Value) : string.Empty;
        }
        public static string GetWeekDay(DateTime? datetime)
        {
            return datetime.HasValue ? datetime.Value.ToString("dddd") : string.Empty;
        }
        /// <summary>
        /// Displays "Number is 123.123.123
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FormatNumber(object obj)
        {
            return obj != null ? string.Format("{0:##,#}", obj) : string.Empty;
        }

        public static string FormatDecial(object obj, int de = 2)
        {
            return obj != null ? ((decimal) obj).ToString("n"+ de) : "0";
        }
        public static string FormatDouble(object obj)
        {
            return obj != null ? ((double)obj).ToString("n2") : "0";
        }
        /// <summary>
        /// Displays "Time Span Format is HH:mm tt 12:10 PM
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTimeSpanFormat(TimeSpan obj)
        {
            try
            {
                DateTime dateTime = DateTime.MinValue.Add(obj);

                CultureInfo cultureInfo = CultureInfo.InvariantCulture;

                // optional
                //CultureInfo cultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name);
                //cultureInfo.DateTimeFormat.PMDesignator = "PM";

                return dateTime.ToString("hh:mm tt", cultureInfo);
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static string GetResAddress(string add1, string add2, string city, string state, string zipcode, string country)
        {
            var add = string.IsNullOrEmpty(add1) ? add2 : add1;
            city = string.IsNullOrEmpty(city) ? string.Empty : string.Format(", {0}", city);
            state = string.IsNullOrEmpty(state) ? string.Empty : string.Format(", {0}", state);
            zipcode = string.IsNullOrEmpty(zipcode) ? string.Empty : string.Format(", {0}", zipcode);

            return string.Format("{0}{1}{2}{3}, {4}", add, city, state, zipcode, country);
        }
    }
}
