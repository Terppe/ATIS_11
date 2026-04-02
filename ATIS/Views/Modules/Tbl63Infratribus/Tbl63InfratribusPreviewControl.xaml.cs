using System.Collections.ObjectModel;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPreviewControl : UserControl
{
    public Tbl63InfratribusPreviewControl()
    {
        InitializeComponent();
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
            typeof(Tbl63InfratribusPreviewControl),
            new PropertyMetadata(null));

}

