using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class IdToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int id && parameter is IDictionary<int, string> lookup)
        {
            return lookup.TryGetValue(id, out var name) ? name : "";
        }
        return "";
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
