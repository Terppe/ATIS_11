

namespace ATIS.DesignSystem.Controls;

public sealed class AtisCard : ContentControl
{
    public AtisCard()
    {
        DefaultStyleKey = typeof(AtisCard);
    }

    public bool IsElevated
    {
        get => (bool)GetValue(IsElevatedProperty);
        set => SetValue(IsElevatedProperty, value);
    }

    public static readonly DependencyProperty IsElevatedProperty =
        DependencyProperty.Register(nameof(IsElevated), typeof(bool),
            typeof(AtisCard), new PropertyMetadata(false, OnIsElevatedChanged));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateElevationState();
    }

    private static void OnIsElevatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AtisCard card)
            card.UpdateElevationState();
    }

    private void UpdateElevationState()
    {
        var state = IsElevated ? "Elevated" : "Normal";
        VisualStateManager.GoToState(this, state, true);
    }
}
