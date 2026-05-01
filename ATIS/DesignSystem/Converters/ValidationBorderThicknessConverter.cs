using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class ValidationBorderThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string error && !string.IsNullOrEmpty(error))
            return new Thickness(2);
        return new Thickness(1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
