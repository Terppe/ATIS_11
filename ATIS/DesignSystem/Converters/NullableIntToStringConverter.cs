using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class NullableIntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, string language)
    {
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object value, Type targetType,
        object parameter, string language)
    {
        var text = value as string;

        if (string.IsNullOrWhiteSpace(text))
            return null;

        return int.TryParse(text, NumberStyles.Integer,
            CultureInfo.CurrentCulture, out var number)
            ? number
            : DependencyProperty.UnsetValue;
    }
}
