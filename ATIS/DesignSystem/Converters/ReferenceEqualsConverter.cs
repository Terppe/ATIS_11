using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class ReferenceEqualsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => ReferenceEquals(value, parameter);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
