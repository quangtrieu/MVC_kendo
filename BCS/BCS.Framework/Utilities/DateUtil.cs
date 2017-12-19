using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Framework.Utilities
{
    public static class DateUtil
    {
        public static int GetDays(int month)
        {
            try
            {
                var year = DateTime.UtcNow.Year;

                return DateTime.DaysInMonth(year, month);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int GetDays(int year, int month)
        {
            try
            {
                return DateTime.DaysInMonth(year, month);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// /// Get LastDateTime of Year
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetLastDateOfYear(DateTime dt)
        {
            //Select the first day of the month by using the DateTime class
            dt = new DateTime(dt.Year, 12, 1);
            //Add one month to our adjusted DateTime
            dt = dt.AddMonths(1);
            //Subtract one day from our adjusted DateTime
            dt = dt.AddDays(-1);
            //Return the DateTime, now set to the last day of the month
            return dt;
        }

        /// <summary>
        /// Get LastDateTime of Month
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetLastDateOfMonth(DateTime dt)
        {
            //Select the first day of the month by using the DateTime class
            dt = new DateTime(dt.Year, dt.Month, 1);
            //Add one month to our adjusted DateTime
            dt = dt.AddMonths(1);
            //Subtract one day from our adjusted DateTime
            dt = dt.AddDays(-1);
            //Return the DateTime, now set to the last day of the month
            return dt;
        }

        public static DateTime GetStartDateOfWeek(DateTime dt, int startOfWeek)
        {
            System.DayOfWeek fdow = System.DayOfWeek.Friday;
            switch (startOfWeek)
            {
                case 0:
                    // Summary:
                    //     Indicates Sunday.
                    fdow = System.DayOfWeek.Sunday;
                    break;
                case 1:
                    //
                    // Summary:
                    //     Indicates Monday.
                    fdow = System.DayOfWeek.Monday;
                    break;
                case 2:
                    //
                    // Summary:
                    //     Indicates Tuesday.
                    fdow = System.DayOfWeek.Tuesday;
                    break;
                case 3:
                    //
                    // Summary:
                    //     Indicates Wednesday.
                    fdow = System.DayOfWeek.Wednesday;
                    break;
                case 4:
                    //
                    // Summary:
                    //     Indicates Thursday.
                    fdow = System.DayOfWeek.Thursday;
                    break;
                case 5:
                    //
                    // Summary:
                    //     Indicates Friday.
                    fdow = System.DayOfWeek.Friday;
                    break;
                case 6:
                    //
                    // Summary:
                    //     Indicates Saturday.
                    fdow = System.DayOfWeek.Saturday;
                    break;
            }
            System.DayOfWeek today = dt.DayOfWeek;
            return DateTime.Now.AddDays(-(today - fdow)).Date;
        }

        public static DateTime? StringToDate(String date)
        {
            DateTime? dateTimeResult = null;
            DateTime dateTemp;
            if (DateTime.TryParse(date, out dateTemp))
            {
                return dateTemp;
            }
            else
            {
                return dateTimeResult;
            }
        }

        /// <summary>
        /// Convert DateTime To UTC DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToUtcDateTime(DateTime? dateTime)
        {

            if (!dateTime.HasValue)
            {
                return null;
            }
            else
            {
                DateTime reval = dateTime.Value.ToUniversalTime();

                return reval;
            }
        }


        public static string ToUtcDateTimeDisplay(DateTime? dateTime)
        {

            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            else
            {
                DateTime reval = dateTime.Value.ToUniversalTime();

                return string.Format("{0:MM/dd/yyyy}", reval);
            }
        }

        /// <summary>
        /// Convert DateTime To Local DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToLocalDateTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime reval = dateTime.Value.ToLocalTime();

                return reval;
            }
            else
            {
                return null;
            }

        }

        public static string ToLocalDateTimeDisplay(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime reval = dateTime.Value.ToLocalTime();

                return string.Format("{0:MM/dd/yyyy}", reval);
            }
            else
            {
                return string.Empty;
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
        public static string ToLocalDateTimeStr(string str)
        {
            string val = string.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    DateTime reval = Convert.ToDateTime(str).ToLocalTime();

                    val = string.Format("{0:MM/dd/yyyy}", reval);
                }
                catch (Exception)
                {
                    val = string.Empty;
                }
            }

            return val;

        }


        public static DateTime? TryParseDate(String date)
        {
            DateTime dateTimeResult;
            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult) == true)
            {
                return dateTimeResult;
            }
            else
            {
                return null;
            }
        }
    }
}



