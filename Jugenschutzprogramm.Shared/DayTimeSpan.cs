namespace Jugenschutzprogramm.Shared
{
    public class DayTimeSpan : PropertyChangedBase
    {
        private double _fromTime;
        private double _toTime;
        private bool _useDayTimeSpan;

        public double FromTime
        {
            get { return _fromTime; }
            set { SetProperty(value, ref _fromTime); }
        }

        public double ToTime
        {
            get { return _toTime; }
            set { SetProperty(value, ref _toTime); }
        }

        public bool UseDayTimeSpan
        {
            get { return _useDayTimeSpan; }
            set { SetProperty(value, ref _useDayTimeSpan); }
        }
    }
}
