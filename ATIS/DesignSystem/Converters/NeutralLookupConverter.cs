using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class NeutralLookupConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int id &&
            parameter is Dictionary<int, string> lookup &&
            lookup.TryGetValue(id, out var name))
        {
            return name;
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
