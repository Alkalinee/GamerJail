using System;

namespace Jugendschutzprogramm
{
    class ServiceManager : PropertyChangedBase
    {
        private TimeSpan _timePlayedToday;
        private bool _isFreeDay;

        public TimeSpan TimePlayedToday
        {
            get { return _timePlayedToday; }
            set { SetProperty(value, ref _timePlayedToday); }
        }

        public bool IsFreeDay
        {
            get { return _isFreeDay; }
            set { SetProperty(value, ref _isFreeDay); }
        }
    }
}
