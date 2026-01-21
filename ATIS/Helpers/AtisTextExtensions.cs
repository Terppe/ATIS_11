using Microsoft.UI.Text;

namespace Atis.Helpers;

public static class AtisTextExtensions
{
    public static readonly DependencyProperty RoleProperty =
        DependencyProperty.RegisterAttached(
            "Role",
            typeof(string),
            typeof(AtisTextExtensions),
            new PropertyMetadata(null, OnRoleChanged));

    public static string GetRole(DependencyObject obj) =>
        (string)obj.GetValue(RoleProperty);

    public static void SetRole(DependencyObject obj, string value) =>
        obj.SetValue(RoleProperty, value);

    private static void OnRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock tb)
            return;

        string role = (string)e.NewValue;

        switch (role)
        {
            case "Headline":
                tb.FontSize = 24;
                tb.FontWeight = FontWeights.SemiBold;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Opacity = 1.0;
                break;

            case "Title":
                tb.FontSize = 20;
                tb.FontWeight = FontWeights.SemiBold;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Opacity = 1.0;
                break;

            case "Subtitle":
                tb.FontSize = 16;
                tb.FontWeight = FontWeights.Medium;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Opacity = 1.0;
                break;

            case "Body":
                tb.FontSize = 14;
                tb.FontWeight = FontWeights.Normal;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Opacity = 1.0;
                break;

            case "Caption":
                tb.FontSize = 12;
                tb.FontWeight = FontWeights.Normal;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Opacity = 0.75;
                break;

            default:
                // Falls jemand einen unbekannten Wert setzt → nichts tun
                break;
        }
    }
}
