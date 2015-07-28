using System;
using System.Globalization;
using System.Windows.Data;

namespace Jugenschutzprogramm_Installer.Converter
{
    class TimeSpanToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromHours((double) value / 2 + 4).ToString("hh\\:mm") + " Uhr";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}