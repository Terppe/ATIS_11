using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class BoolToBorderThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool hasError && hasError)
            return new Thickness(1.5); // roter Border sichtbar

        return new Thickness(1); // normaler Border
    }


    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
