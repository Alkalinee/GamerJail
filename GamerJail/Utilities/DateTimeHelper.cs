using System;

namespace GamerJail.Utilities
{
    public class DateTimeHelper
    {
        private readonly DateTime _todayStartDate;
        private readonly DateTime _todayEndDate;

        private readonly DateTime _weekStartDate;
        private readonly DateTime _weekEndDate;

        private readonly DateTime _now;

        public DateTimeHelper()
        {
            _now = DateTime.Now;
            _todayStartDate = _now.Hour < 4
                ? new DateTime(_now.Year, _now.Month, _now.Day, 4, 0, 0).AddDays(-1)
                : new DateTime(_now.Year, _now.Month, _now.Day, 4, 0, 0);

            _todayEndDate = _todayStartDate.AddHours(24);
            NextDay = _todayEndDate;

            _weekStartDate = _now.StartOfWeek(DayOfWeek.Monday).AddHours(4);
            _weekEndDate = _weekStartDate.AddDays(7);
            NextWeek = _weekEndDate;
        }

        public DateTime NextWeek { get; }
        public DateTime NextDay { get; }

        public bool IsToday(DateTime dateTime)
        {
            return dateTime >= _todayStartDate && dateTime < _todayEndDate;
        }

        public bool IsCurrentWeek(DateTime dateTime)
        {
            return dateTime >= _weekStartDate && dateTime < _weekEndDate;
        }

        public bool IsCurrentMonth(DateTime dateTime)
        {
            return dateTime.Month == _now.Month;
        }

        public bool IsCurrentYear(DateTime dateTime)
        {
            return dateTime.Year == _now.Year;
        }

        public static DateTime GetToday()
        {
            var now = DateTime.Now;
            if (now.Hour < 4)
                return now.AddDays(-1).Date;
            return DateTime.Today;
        }

        public static DateTime GetDateTime(DateTime dateTime)
        {
            if (dateTime.Hour < 4)
                return dateTime.AddDays(-1).Date;
            return dateTime.Date;
        }
    }
}