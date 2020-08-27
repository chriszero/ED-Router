using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ED_Router.UI.Desktop.Converters
{
    public class AnyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue);
            }

            if (value is IEnumerable iterableValue)
            {
                return iterableValue?.OfType<object>().Any();
            }

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This converter is designed to only work one way.");
        }
    }
}
