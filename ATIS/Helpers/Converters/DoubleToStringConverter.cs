using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class DoubleToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        //if (value is double doubleValue)
        //{
        //    return doubleValue.ToString(CultureInfo.InvariantCulture);
        //}
        //return value;

        //if (value != null)
        //{
        //    return value.ToString();
        //}

        //return null;

        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      //  return decimal.TryParse((string)value, out var ret) ? ret : 0;
        return double.TryParse((string)value, out var ret) ? ret : 0;
    }

}
