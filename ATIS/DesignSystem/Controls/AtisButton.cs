using ATIS.DesignSystem.Enums;

namespace ATIS.DesignSystem.Controls;

public sealed class AtisButton : Button
{
    public AtisButton()
    {
        DefaultStyleKey = typeof(AtisButton);
    }

    public AtisButtonKind Kind
    {
        get => (AtisButtonKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(AtisButtonKind),
            typeof(AtisButton), new PropertyMetadata(AtisButtonKind.Primary));
}
