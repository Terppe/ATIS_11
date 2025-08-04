using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class DoubleToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value != null)
        {
            return value.ToString();
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return decimal.TryParse((string)value, out var ret) ? ret : 0;
    }

}
