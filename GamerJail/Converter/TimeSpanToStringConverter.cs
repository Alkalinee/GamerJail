using System;
using System.Globalization;
using System.Windows.Data;

namespace GamerJail.Converter
{
    class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan) value;
            return (int) timeSpan.TotalHours + timeSpan.ToString(@"\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
