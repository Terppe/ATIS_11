using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value.ToString() ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => double.TryParse(value?.ToString(), out double d) ? d : 0d;
}

