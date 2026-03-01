using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class PageWidthFractionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double width && double.TryParse(parameter?.ToString(), out double fraction))
            return width * fraction;

        return 300; // fallback
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
