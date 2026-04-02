namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusEditControl : UserControl
{
    public Tbl60SubtribusEditControl()
    {
        InitializeComponent();
    }

    public Models.Tbl60Subtribus? Item
    {
        get => (Models.Tbl60Subtribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl60Subtribus),
            typeof(Tbl60SubtribusEditControl),
            new PropertyMetadata(null));
}
