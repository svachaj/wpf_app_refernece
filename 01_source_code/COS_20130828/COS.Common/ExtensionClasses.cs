using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtension
    {
        public static bool IsNullOrEmptyString(this string str)
        {
            bool result = true;

            if (string.IsNullOrEmpty(str))
                result = true;
            else
                result = false;

            return result;
        }

        public static DateTime AddWorkDay(this DateTime date, int days)
        {
            DateTime result = date.AddDays(days);

            int i = 1;
            while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                if (days > 0)
                    result = result.AddDays(days + i);
                else
                    result = result.AddDays(days - i);

                i++;
            }

            return result;
        }

        public static bool IsWorkDay(this DateTime date)
        {
            bool result = true;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                result = false;

            return result;
        }
    }

    public static class DateTimeExtension 
    {
        public static string ToShortTime(this TimeSpan span)
        {
            string result = "";

            result += span.Hours.ToString().PadLeft(2, '0');
            result += ":";
            result += span.Minutes.ToString().PadLeft(2, '0');

            return result;
        }
    }
}
