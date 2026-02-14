
namespace ATIS.Helpers;

public static class WidthHelper
{
    private const double CharWidth = 8.0;
    private const double Padding = 20.0;

    // Mindestbreiten, damit Textboxen niemals verschwinden
    private const double AbsoluteMinWidth = 120.0;   // Textbox immer sichtbar
    private const double AbsoluteMaxWidth = 600.0;   // verhindert Monsterbreiten

    public static (double MinWidth, double MaxWidth) CalculateWidths(
        int maxChars,
        int minChars,
        double pageWidth)
    {
        // Falls PageWidth nicht gesetzt ist → sichere Defaults
        if (pageWidth <= 0 || double.IsNaN(pageWidth))
            return (AbsoluteMinWidth, AbsoluteMinWidth);

        // Fallbacks, falls Model 0 liefert
        int safeMin = Math.Max(minChars, 3);
        int safeMax = Math.Max(maxChars, safeMin);

        // Berechnung
        double fullWidth = safeMax * CharWidth + Padding;
        double halfWidth = safeMin * CharWidth + Padding;

        // Begrenzen auf PageWidth
        double minWidth = Math.Min(halfWidth, pageWidth);
        double maxWidth = Math.Min(fullWidth, pageWidth);

        // HARTE Mindestbreiten erzwingen
        minWidth = Math.Max(minWidth, AbsoluteMinWidth);
        maxWidth = Math.Max(maxWidth, AbsoluteMinWidth);

        // HARTE Maximalbreiten erzwingen
        minWidth = Math.Min(minWidth, AbsoluteMaxWidth);
        maxWidth = Math.Min(maxWidth, AbsoluteMaxWidth);

        return (minWidth, maxWidth);
    }
}
