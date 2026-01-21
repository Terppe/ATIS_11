using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class NullToBoolConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool result = value != null;
        return Invert ? !result : result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
