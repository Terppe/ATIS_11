using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class IndexToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || parameter == null)
            return Visibility.Collapsed;

        if (int.TryParse(value.ToString(), out int selectedIndex) &&
            int.TryParse(parameter.ToString(), out int targetIndex))
        {
            return selectedIndex == targetIndex
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
