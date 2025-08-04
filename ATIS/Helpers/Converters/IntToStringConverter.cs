using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class IntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return int.TryParse((string)value, out var ret) ? ret : 0;
    }
}
