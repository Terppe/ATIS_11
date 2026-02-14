using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class NullOrEmptyToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool hasError = !string.IsNullOrEmpty(value as string);

        if (parameter?.ToString() == "Invert")
            return !hasError;

        return hasError;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
