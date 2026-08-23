using ATIS.Helpers;
using WinUI3Localizer;

namespace ATIS.Views.Report.DataTemplateHeaderTaxoNomen;


public sealed partial class TemplateTbl63InfratribusTaxoNomenHeaderControl : UserControl
{
    public TemplateTbl63InfratribusTaxoNomenHeaderControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTexts();

        Localizer.Get().LanguageChanged -= OnLanguageChanged;
        Localizer.Get().LanguageChanged += OnLanguageChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Localizer.Get().LanguageChanged -= OnLanguageChanged;
    }

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        UpdateTexts();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        var itemProperty = DataContext?.GetType().GetProperty("Item");
        var item = itemProperty?.GetValue(DataContext);

        if (item == null)
        {
            ValidityTextBlock.Text = string.Empty;
            NamesTextBlock.Text = string.Empty;
            return;
        }

        var valid = item.GetType().GetProperty("Valid")?.GetValue(item) as bool?;
        var validYear = item.GetType().GetProperty("ValidYear")?.GetValue(item)?.ToString();
        var engName = item.GetType().GetProperty("EngName")?.GetValue(item)?.ToString();
        var gerName = item.GetType().GetProperty("GerName")?.GetValue(item)?.ToString();
        var fraName = item.GetType().GetProperty("FraName")?.GetValue(item)?.ToString();
        var porName = item.GetType().GetProperty("PorName")?.GetValue(item)?.ToString();

        ValidityTextBlock.Text = ValidityFormatter.Format(valid, validYear);
        NamesTextBlock.Text = NamesFormatter.Format(engName, gerName, fraName, porName);
    }
}
