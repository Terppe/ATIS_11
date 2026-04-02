
using Microsoft.UI.Xaml.Input;
using static ATIS.Views.Report.PdfModels;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusRowControl : UserControl
{
    public Tbl63InfratribusRowControl()
    {
        InitializeComponent();
    }

    public Models.Tbl63Infratribus? Item =>
        DataContext as Models.Tbl63Infratribus;

    public Models.Tbl63Infratribus Infratribus
    {
        get => (Models.Tbl63Infratribus)GetValue(InfratribusProperty);
        set => SetValue(InfratribusProperty, value);
    }

    public static readonly DependencyProperty InfratribusProperty =
        DependencyProperty.Register(nameof(Infratribus),
            typeof(Models.Tbl63Infratribus),
            typeof(Tbl63InfratribusRowControl),
            new PropertyMetadata(null));

    // ---------------------------------------------------------
    // SELECTION STATE
    // ---------------------------------------------------------
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(Tbl63InfratribusRowControl),
            new PropertyMetadata(false, OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl63InfratribusRowControl)d;
        control.UpdateSelectionState();
    }

    private void UpdateSelectionState()
    {
        VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Normal", true);
    }

    // ---------------------------------------------------------
    // LOOKUP (Subtribus)
    // ---------------------------------------------------------
    public Dictionary<int, string> SubtribusLookup
    {
        get => (Dictionary<int, string>)GetValue(SubtribusLookupProperty);
        set => SetValue(SubtribusLookupProperty, value);
    }

    public static readonly DependencyProperty SubtribusLookupProperty =
        DependencyProperty.Register(
            nameof(SubtribusLookup),
            typeof(Dictionary<int, string>),
            typeof(Tbl63InfratribusRowControl),
            new PropertyMetadata(null));



    private void Root_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (!IsSelected)
            VisualStateManager.GoToState(this, "Hover", true);
    }

    private void Root_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!IsSelected)
            VisualStateManager.GoToState(this, "Normal", true);
    }

    private void Root_Tapped(object sender, TappedRoutedEventArgs e)
    {
        RowTapped?.Invoke(this, Infratribus);
    }

    public event EventHandler<Models.Tbl63Infratribus>? RowTapped;

    public void FocusRow()
    {
        Root.Focus(FocusState.Programmatic);
    }

}
