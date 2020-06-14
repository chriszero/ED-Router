using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ED_Router.UI.Desktop.Converters
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNotNull = value != null;
            var param = parameter as string;
            var isInverted = string.Equals(param, "I", StringComparison.OrdinalIgnoreCase);
            
            if (isInverted)
            {
                return !isNotNull
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            return isNotNull
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}