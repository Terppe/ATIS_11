using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace ATIS.Helpers.Converters;

//[ValueConversion(typeof(string), typeof(string))]
public class CharacterCasingConverter : MarkupExtension, IValueConverter
{

    //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    => (value as string)?.ToUpper() ?? value; // If it's a string, call ToUpper(), otherwise, pass it through as-is.

    //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    => throw new NotSupportedException();

    protected override object ProvideValue() => this;

    public object Convert(object value, Type targetType, object parameter, string language)
        => (value as string)?.ToUpper() ?? value; // If it's a string, call ToUpper(), otherwise, pass it through as-is.

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
