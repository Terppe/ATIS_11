
namespace ATIS.DesignSystem.Controls.TextBox;

public sealed class LabeledInfoField : Control
{
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(LabeledInfoField), new PropertyMetadata(null));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(LabeledInfoField), new PropertyMetadata(null));

    public LabeledInfoField()
    {
        DefaultStyleKey = typeof(LabeledInfoField);
    }
}
