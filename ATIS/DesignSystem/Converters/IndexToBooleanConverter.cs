using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public sealed class IndexToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || parameter == null)
            return false;

        if (int.TryParse(value.ToString(), out int selectedIndex) &&
            int.TryParse(parameter.ToString(), out int currentIndex))
        {
            return selectedIndex == currentIndex;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isChecked &&
            isChecked &&
            parameter != null &&
            int.TryParse(parameter.ToString(), out int index))
        {
            return index;
        }

        return 0;
    }
}
