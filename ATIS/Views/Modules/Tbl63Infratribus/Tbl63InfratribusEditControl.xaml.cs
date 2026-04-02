namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusEditControl : UserControl
{
    public Tbl63InfratribusViewModel ViewModel
    {
        get => (Tbl63InfratribusViewModel)DataContext;
    }

    public Models.Tbl63Infratribus Item
    {
        get => (Models.Tbl63Infratribus)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl63Infratribus),
            typeof(Tbl63InfratribusEditControl),
            new PropertyMetadata(null));

    public Tbl63InfratribusEditControl()
    {
        InitializeComponent();
    }
}
