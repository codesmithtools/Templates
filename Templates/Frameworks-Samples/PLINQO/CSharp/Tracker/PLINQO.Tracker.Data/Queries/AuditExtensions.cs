using System;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace PLINQO.Tracker.Data
{
    public static partial class AuditExtensions
    {
        //Add query extension methods here.

        public static string ToAgeString(this DateTime fromDate)
        {
            return ToAgeString(fromDate, DateTime.Now, 0);
        }

        public static string ToAgeString(this DateTime fromDate, int maxSpans)
        {
            return ToAgeString(fromDate, DateTime.Now, maxSpans);
        }

        public static string ToAgeString(this DateTime fromDate, DateTime toDate, int maxSpans)
        {
            TimeSpan ts = toDate.Subtract(fromDate);
            int spanCount = 0;

            int year;
            int month;
            int week;
            int day;
            int increment = 0;

            // Day Calculation
            if (fromDate.Day > toDate.Day)
                increment = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);

            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
                day = toDate.Day - fromDate.Day;

            week = day / 7;
            day = day % 7;

            //month calculation
            if ((fromDate.Month + increment) > toDate.Month)
            {
                month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }

            // year calculation
            year = toDate.Year - (fromDate.Year + increment);
            StringBuilder sb = new StringBuilder();

            if (AppendSpan(sb, "year", year, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "month", month, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "week", week, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "day", day, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "hour", ts.Hours, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "minute", ts.Minutes, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "second", ts.Seconds, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();


            return sb.ToString();
        }

        private static bool AppendSpan(StringBuilder builder, string spanName, int spanValue, ref int total)
        {
            const string spacer = ", ";

            if (spanValue <= 0)
                return false;

            if (builder.Length > 0)
                builder.Append(spacer);

            builder.AppendFormat("{0} {1}{2}", spanValue, spanName, GetTense(spanValue));
            total++;
            return true;
        }

        private static string GetTense(int value)
        {
            return value > 1 ? "s" : string.Empty;
        }

 #region Query
        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Add your compiled queries here. 
        } 
        #endregion
    }
}