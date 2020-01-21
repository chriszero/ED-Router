using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ED_Router.UI.Desktop.Converters
{
    public class VisibleAndMultiConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
            {
                return Visibility.Collapsed;
            }

            return values.OfType<Visibility>().All(v => v == Visibility.Visible)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
