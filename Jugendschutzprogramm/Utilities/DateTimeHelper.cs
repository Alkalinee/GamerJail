using System;

namespace Jugendschutzprogramm.Utilities
{
    public class DateTimeHelper
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public DateTimeHelper()
        {
            var now = DateTime.Now;
            _startDate = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);
            _endDate = _startDate.AddHours(24);
        }

        public bool IsToday(DateTime dateTime)
        {
            return dateTime >= _startDate && dateTime < _endDate;
        }
    }
}