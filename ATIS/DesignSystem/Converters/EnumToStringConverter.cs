using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            //DataGridSortDirection.Ascending => "▲",
            //DataGridSortDirection.Descending => "▼",
            _ => ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
