using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Controls.Headers;

public class HeaderBase : Grid
{
    public HeaderBase()
    {
        // Standard-Designsystem-Werte
        Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xEF, 0xEF, 0xEF));
        CornerRadius = new CornerRadius(4);
        Padding = new Thickness(4);
        Margin = new Thickness(0, 0, 0, 6);
    }
}
