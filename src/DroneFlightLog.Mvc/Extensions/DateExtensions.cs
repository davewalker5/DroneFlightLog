using System;

namespace DroneFlightLog.Mvc.Extensions
{
    public static class DateExtensions
    {
        public static string ToEncodedDateTimeString(this DateTime date, string format)
        {
            return date.ToString(format).Replace(" ", "%20");
        }

        public static string ToEncodedDateTimeString(this DateTime? date, string format, DateTime defaultDate)
        {
            DateTime nonNullDateTime = date ?? defaultDate;
            return nonNullDateTime.ToEncodedDateTimeString(format).Replace(" ", "%20");
        }
    }
}
