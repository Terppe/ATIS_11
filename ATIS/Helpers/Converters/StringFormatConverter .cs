using System.Globalization;
using Microsoft.UI.Xaml.Data;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ATIS.Helpers.Converters;

class StringFormatConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
        {
            return null;
        }

        // Retrieve the format string and use it to format the value.
        string formatString = parameter as string;
        if (string.IsNullOrEmpty(formatString))
        {
            // If the format string is null or empty, simply call ToString()
            // on the value.
            return value.ToString();
        }

        try
        {
            CultureInfo culture = string.IsNullOrWhiteSpace(language) ? CultureInfo.InvariantCulture : new CultureInfo(language);
            return string.Format(culture, formatString, value);
        }
        catch
        {
            return value;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
