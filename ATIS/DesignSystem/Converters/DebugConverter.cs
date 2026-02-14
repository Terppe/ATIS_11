using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Converters;

public class DebugConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        System.Diagnostics.Debug.WriteLine("CONVERTER HIT: " + (value ?? "NULL"));
        return new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
