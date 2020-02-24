using System;

namespace ncl.app.Loyalty.Aloha.Relay.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            return dt.AddDays(-NumberOfDaysFromBeginningOfWeek());

            int NumberOfDaysFromBeginningOfWeek()
            {
                int count = dt.DayOfWeek - startOfWeek;
                if (count < 0)
                {
                    return count + 7;
                }
                return count;
            }
        }
    }
}