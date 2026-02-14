using ATIS.Helpers;
using ATIS.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class EfWidthConverter : IValueConverter
{
    public DbContext Db { get; set; }
    public string Entity { get; set; }
    public string Property { get; set; }

    public double CharWidth { get; set; } = 7.0;
    public double Padding { get; set; } = 20.0;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not double pageWidth)
            return 200;

        if (parameter is not string mode)
            return 200;

        // EF‑MaxLength holen
        var entityType = Type.GetType(Entity);
        int maxChars = EfMetadataHelper.GetMaxLength(Db, entityType, Property);

        // MinChars = halbe EF‑Länge
        int minChars = maxChars / 2;

        // Pixelbreiten
        double fullWidth = maxChars * CharWidth + Padding;
        double halfWidth = minChars * CharWidth + Padding;

        // Begrenzung durch PageWidth
        double maxAllowed = pageWidth;

        return mode switch
        {
            "Min" => Math.Min(halfWidth, maxAllowed), // NIE kleiner als halbe EF‑Länge
            "Max" => Math.Min(fullWidth, maxAllowed), // volle EF‑Länge, aber begrenzt
            _ => fullWidth
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
