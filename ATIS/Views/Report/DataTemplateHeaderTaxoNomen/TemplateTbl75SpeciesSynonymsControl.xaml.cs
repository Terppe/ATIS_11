using ATIS.Models;

namespace ATIS.Views.Report.DataTemplateHeaderTaxoNomen;

public sealed partial class TemplateTbl75SpeciesSynonymsControl : UserControl
{
    public TemplateTbl75SpeciesSynonymsControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public Tbl75Species? Species
    {
        get => (Tbl75Species?)GetValue(SpeciesProperty);
        set => SetValue(SpeciesProperty, value);
    }

    public static readonly DependencyProperty SpeciesProperty =
        DependencyProperty.Register(
            nameof(Species),
            typeof(Tbl75Species),
            typeof(TemplateTbl75SpeciesSynonymsControl),
            new PropertyMetadata(null, OnSpeciesChanged));

    private static void OnSpeciesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl75SpeciesSynonymsControl control)
            control.UpdateDisplay();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (SynonymsTextBlock == null)
            return;

        // Platzhalter, bis die Extra-Tabelle angebunden ist
        SynonymsTextBlock.Text = string.Empty;

     //   SynonymsTextBlock.Text = Species?.Synonym ?? string.Empty;
    }
}
