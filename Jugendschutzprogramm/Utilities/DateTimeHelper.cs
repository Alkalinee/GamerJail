using System;

namespace Jugendschutzprogramm.Utilities
{
    public class DateTimeHelper
    {
        private readonly DateTime _todayStartDate;
        private readonly DateTime _todayEndDate;
        private readonly DateTime _weekStartDate;
        private readonly DateTime _weekEndDate;

        public DateTimeHelper()
        {
            var now = DateTime.Now;
            _todayStartDate = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);
            _todayEndDate = _todayStartDate.AddHours(24);

            _weekStartDate = now.StartOfWeek(DayOfWeek.Monday).AddHours(4);
            _weekEndDate = _weekStartDate.AddDays(7);
        }

        public bool IsToday(DateTime dateTime)
        {
            return dateTime >= _todayStartDate && dateTime < _todayEndDate;
        }

        public bool IsCurrentWeek(DateTime dateTime)
        {
            return dateTime >= _weekStartDate && dateTime < _weekEndDate;
        }
    }
}