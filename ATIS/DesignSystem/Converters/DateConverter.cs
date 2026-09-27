using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        //if (value != null)
        //{
        //    var test = (DateTime)value;
        //    var date = test.ToString("dd/MM/yyyy");
        //    return (date);
        //}
        //return string.Empty;
        if (value != null)
        {
            var date = (DateTime)value;
            return date.ToShortDateString();
        }
        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // return null;
        var strValue = value as string;
        DateTime resultDateTime;
        if (DateTime.TryParse(strValue, out resultDateTime))
        {
            return resultDateTime;
        }
        return DependencyProperty.UnsetValue;

    }

    //public object Convert(object value, Type targetType, object parameter, string language)
    //{
    //    return DateTime.Parse(value.ToString()).ToLongDateString();
    //}

    //public object ConvertBack(object value, Type targetType, object parameter, string language)
    //{
    //    throw new NotImplementedException();
    //}
}
