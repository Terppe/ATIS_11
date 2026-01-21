using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value?.ToString();

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => double.TryParse(value?.ToString(), out double d) ? d : 0d;
}

