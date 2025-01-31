using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace COS.Common
{
    public static class Helpers
    {
        public static DateTime FirstDateOfWeek(int year, int weekNum, CalendarWeekRule rule)
        {
            Debug.Assert(weekNum >= 1);

            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);
            Debug.Assert(firstMonday.DayOfWeek == DayOfWeek.Monday);

            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday);

            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            DateTime result = firstMonday.AddDays(weekNum * 7);

            return result;
        }

       static Dictionary<int, string>  _Days = null;
       public static Dictionary<int, string> Days
       {
           get
           {
               if (_Days == null)
               {
                   _Days = new Dictionary<int, string>();

                   _Days.Add(1, "Pondělí");
                   _Days.Add(2, "Úterý");
                   _Days.Add(3, "Středa");
                   _Days.Add(4, "Čtvrtek");
                   _Days.Add(5, "Pátek");
                   _Days.Add(6, "Sobota");
                   _Days.Add(0, "Neděle");
               }

               return _Days;
           }
       }
    }
}
