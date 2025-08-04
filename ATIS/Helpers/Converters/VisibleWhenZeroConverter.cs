using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;
public class VisibleWhenZeroConverter : IValueConverter
{
    public object Convert(object v, Type t, object p, string l) =>
        Equals(0d, (int)v) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object v, Type t, object p, string l) => null;
}
