using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class BoolToRowBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool)value
            ? Application.Current.Resources["DataGridAlternatingRowBackgroundBrush"]
            : Application.Current.Resources["DataGridRowBackgroundBrush"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
