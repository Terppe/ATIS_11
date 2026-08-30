using ATIS.Interfaces;
using ATIS.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Views.Report.DataTemplateHeaderTaxoNomen;

public sealed partial class TemplateTbl75SpeciesSynonymsControl : UserControl
{
    private readonly IDataService _dataService;

    public TemplateTbl75SpeciesSynonymsControl()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
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

    private static async void OnSpeciesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl75SpeciesSynonymsControl control)
            await control.UpdateDisplayAsync();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await UpdateDisplayAsync();
    }

    private async Task UpdateDisplayAsync()
    {
        if (SynonymsTextBlock == null || Species == null)
        {
            if (SynonymsTextBlock != null)
                SynonymsTextBlock.Text = string.Empty;
            return;
        }

        var synonyms = await _dataService.GetSynonymsBySpeciesIdAsync(Species.SpeciesId);

        SynonymsTextBlock.Text = string.Join(
            Environment.NewLine,
            synonyms
                .Where(x => !string.IsNullOrWhiteSpace(x.SynonymName))
                .Select(FormatSynonym));
    }

    private static string FormatSynonym(Tbl84Synonym synonym)
    {
        var name = synonym.SynonymName?.Trim() ?? string.Empty;

        var authorParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(synonym.Author))
            authorParts.Add(synonym.Author.Trim());

        if (!string.IsNullOrWhiteSpace(synonym.AuthorYear))
            authorParts.Add(synonym.AuthorYear.Trim());

        if (authorParts.Count == 0)
            return name;

        return $"{name} - {string.Join(" ", authorParts)}";
    }
}
