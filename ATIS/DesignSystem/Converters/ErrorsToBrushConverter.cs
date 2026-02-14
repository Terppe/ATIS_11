using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Converters;

public class ErrorsToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        System.Diagnostics.Debug.WriteLine("CONVERTER VALUE = " + (value == null ? "NULL" : value.GetType().ToString()));

        var errors = value as IEnumerable<string>;

        if (value is IEnumerable<string> errs)
        {
            System.Diagnostics.Debug.WriteLine("ERROR COUNT = " + errs.Count());
        }

        if (errors != null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR COUNT = " + errors.Count());
        }

        if (errors != null && errors.Any())
            return new SolidColorBrush(Colors.Red);

        if (value == null)
            return new SolidColorBrush(Colors.Gray);

        return (Application.Current.Resources["AtisTextBoxBorderBrush"] as SolidColorBrush)
               ?? new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
