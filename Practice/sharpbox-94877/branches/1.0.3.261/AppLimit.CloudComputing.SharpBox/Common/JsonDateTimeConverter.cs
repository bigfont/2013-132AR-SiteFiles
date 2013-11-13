using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    public class JsonDateTimeConverter
    {
        private enum Month
        {
            Jan = 1,
            Feb,
            Mar,
            Apr,
            May,
            Jun,
            Jul,
            Aug,
            Sep,
            Oct,
            Nov,
            Dec
        }

        private static Month ToMonth(String Input)
        {
            return (Month)Enum.Parse(typeof(Month), Input, true);
        }

        public static DateTime GetDateTimeProperty(string dateTime)
        {
            // Sat, 21 Aug 2010 22:31:20 +0000            

            // check1
            if (dateTime.Length == 0)
                return DateTime.MinValue;

            // check2
            if (dateTime.Length != 31)
                throw new InvalidOperationException();

            // date
            String day = dateTime.Substring(5, 2);
            String month = dateTime.Substring(8, 3);
            Month m = ToMonth(month);
            String year = dateTime.Substring(12, 4);

            // time
            String hour = dateTime.Substring(14, 2);
            String min = dateTime.Substring(17, 2);
            String sec = dateTime.Substring(20, 2);

            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(m), Convert.ToInt32(day),
                                    Convert.ToInt32(hour), Convert.ToInt32(min), Convert.ToInt32(sec),
                                    DateTimeKind.Utc);
        }
    }
}
