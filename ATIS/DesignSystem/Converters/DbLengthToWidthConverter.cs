using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class DbLengthToWidthConverter : IValueConverter
{
    public double CharacterWidth { get; set; } = 7.0;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int maxLength)
        {
            var mode = parameter?.ToString();

            if (mode == "Min")
                return maxLength * 0.5 * CharacterWidth; // halbe Länge

            return maxLength * CharacterWidth;           // volle Länge
        }

        return 100;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
