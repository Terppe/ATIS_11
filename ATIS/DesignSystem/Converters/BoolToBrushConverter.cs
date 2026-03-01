using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public Brush TrueBrush { get; set; } = new SolidColorBrush(Colors.Red);
    public Brush FalseBrush { get; set; } = new SolidColorBrush(Colors.Transparent);

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
            return b ? TrueBrush : FalseBrush;

        return FalseBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
