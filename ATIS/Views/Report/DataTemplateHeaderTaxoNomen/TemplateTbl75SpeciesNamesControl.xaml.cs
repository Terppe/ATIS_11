using ATIS.Interfaces;
using ATIS.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Views.Report.DataTemplateHeaderTaxoNomen;

public sealed partial class TemplateTbl75SpeciesNamesControl : UserControl
{
    private readonly IDataService _dataService;

    public TemplateTbl75SpeciesNamesControl()
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
            typeof(TemplateTbl75SpeciesNamesControl),
            new PropertyMetadata(null, OnSpeciesChanged));

    private static async void OnSpeciesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl75SpeciesNamesControl control)
            await control.UpdateDisplayAsync();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await UpdateDisplayAsync();
    }

    private async Task UpdateDisplayAsync()
    {
        if (NamesTextBlock == null || Species == null)
        {
            if (NamesTextBlock != null)
                NamesTextBlock.Text = string.Empty;
            return;
        }

        var names = await _dataService.GetNamesBySpeciesIdAsync(Species.SpeciesId);

        if (names == null)
            return;

        NamesTextBlock.Text = string.Join(
            Environment.NewLine,
            names
                .Where(x => !string.IsNullOrWhiteSpace(x.NameName))
                .Select(x => x.NameName));
    }
}
