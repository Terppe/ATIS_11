using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class ZeroToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => (value is int i && i == 0) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
