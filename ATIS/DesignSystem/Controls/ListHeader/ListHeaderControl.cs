using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Controls.ListHeader;

public sealed partial class ListHeaderControl : Grid
{
    public ListHeaderControl()
    {
        Margin = new Thickness(0, 0, 0, 6);
        Padding = new Thickness(4);
        Background = new SolidColorBrush(Color.FromArgb(255, 0xEF, 0xEF, 0xEF));
        CornerRadius = new CornerRadius(4);
    }
}
