using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class ErrorListToStringConverter : IValueConverter
{
    private readonly Dictionary<(object value, string culture), string> _cache
        = new();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var culture = Thread.CurrentThread.CurrentUICulture.Name;
        var key = (value, culture);

        // Cache hit → sofort zurück
        if (_cache.TryGetValue(key, out var cached))
            return cached;

        // Berechnung nur wenn nötig
        string result = string.Empty;

        if (value is IEnumerable<string> list && list.Any())
            result = string.Join("\n", list);

        // Cache speichern
        _cache[key] = result;

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}


//public class ErrorListToStringConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, string language)
//    {
//        if (value is IEnumerable<string> list && list.Any())
//            return string.Join("\n", list);

//        return string.Empty;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, string language)
//        => throw new NotImplementedException();
//}
