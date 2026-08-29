using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DetailViewModels;
using ATIS.Views.Taxonomy;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using ATIS.Helpers;
using Microsoft.UI;

namespace ATIS.Views.Report.DataTemplateHyperlink;


public sealed partial class TemplateTbl75SpeciesWithOptionalHyperlinkControl : UserControl
{
    public Tbl75SpeciesDetailViewModel? ViewModel => DataContext as Tbl75SpeciesDetailViewModel;

    public TemplateTbl75SpeciesWithOptionalHyperlinkControl()
    {
        InitializeComponent();
        Loaded += TemplateTbl75SpeciesWithOptionalHyperlinkControl_Loaded;
        DataContextChanged += TemplateTbl75SpeciesWithOptionalHyperlinkControl_DataContextChanged;
    }

    public Func<BreadcrumbItem, Task>? NavigateBreadcrumbAsync
    {
        get => (Func<BreadcrumbItem, Task>?)GetValue(NavigateBreadcrumbAsyncProperty);
        set => SetValue(NavigateBreadcrumbAsyncProperty, value);
    }

    public static readonly DependencyProperty NavigateBreadcrumbAsyncProperty =
        DependencyProperty.Register(
            nameof(NavigateBreadcrumbAsync),
            typeof(Func<BreadcrumbItem, Task>),
            typeof(TemplateTbl75SpeciesWithOptionalHyperlinkControl),
            new PropertyMetadata(null));


    public bool ShowAsHyperlink
    {
        get => (bool)GetValue(ShowAsHyperlinkProperty);
        set => SetValue(ShowAsHyperlinkProperty, value);
    }

    public static readonly DependencyProperty ShowAsHyperlinkProperty =
        DependencyProperty.Register(
            nameof(ShowAsHyperlink),
            typeof(bool),
            typeof(TemplateTbl75SpeciesWithOptionalHyperlinkControl),
            new PropertyMetadata(true, OnShowAsHyperlinkChanged));

    private static void OnShowAsHyperlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl75SpeciesWithOptionalHyperlinkControl control)
        {
            control.UpdateHyperlinkVisualState();
            control.UpdateContentMode();
        }
    }

    private void TemplateTbl75SpeciesWithOptionalHyperlinkControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateHyperlinkVisualState();
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void TemplateTbl75SpeciesWithOptionalHyperlinkControl_DataContextChanged(FrameworkElement sender,
        DataContextChangedEventArgs args)
    {
        UpdateHyperlinkVisualState();
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void UpdateHyperlinkVisualState()
    {
        TbSpecies.Foreground = ShowAsHyperlink
            ? new SolidColorBrush(Colors.DarkBlue)
            : (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

        TbSpecies.TextDecorations = ShowAsHyperlink ? TextDecorations.Underline : TextDecorations.None;
        TbSpecies.IsHitTestVisible = ShowAsHyperlink;
    }

    private void UpdateAdditionalText()
    {
        if (TbSpecies == null || TbAuthorDisplay == null || TbLanguageNamesDisplay == null || TbSpeciesLine == null)
            return;

        if (DataContext is not TaxonomyLine<Tbl75Species> line || line.Item == null)
        {
            TbSpecies.Text = string.Empty;
            TbAuthorDisplay.Text = string.Empty;
            TbLanguageNamesDisplay.Text = string.Empty;
            TbSpeciesLine.Text = string.Empty;
            return;
        }

        var test = line.Item.GenusSpeciesFullName;

        TbAuthorDisplay.Text = TaxonomyDisplayFormatter.BuildPrefixedParts(
            " - ",
            line.Item.Author,
            line.Item.AuthorYear);

        TbLanguageNamesDisplay.Text = string.Empty;

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(line.Item.GenusSpeciesFullName))
            parts.Add(line.Item.GenusSpeciesFullName);

        var authorPart = string.Join(" ",
            new[] { line.Item.Author, line.Item.AuthorYear }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        if (!string.IsNullOrWhiteSpace(authorPart))
            parts.Add(authorPart);

        TbSpeciesLine.Text = string.Join(" - ", parts);
    }

    //{
    //    if (TbAuthorDisplay == null || TbLanguageNamesDisplay == null || TbSpeciesLine == null)
    //        return;

    //    if (DataContext is not TaxonomyLine<Tbl75Species> line || line.Item == null)
    //    {
    //        TbAuthorDisplay.Text = string.Empty;
    //        TbLanguageNamesDisplay.Text = string.Empty;
    //        TbSpeciesLine.Text = string.Empty;
    //        return;
    //    }

    //    TbAuthorDisplay.Text = TaxonomyDisplayFormatter.BuildPrefixedParts(
    //        " - ",
    //        line.Item.Author,
    //        line.Item.AuthorYear);

    //    //TbLanguageNamesDisplay.Text = TaxonomyDisplayFormatter.BuildPrefixedList(
    //    //    " - ",
    //    //    line.Item.EngName,
    //    //    line.Item.GerName,
    //    //    line.Item.FraName,
    //    //    line.Item.PorName);

    //    var parts = new List<string>();

    //    if (!string.IsNullOrWhiteSpace(line.Item.SpeciesName))
    //        parts.Add(line.Item.SpeciesName);

    //    var authorPart = string.Join(" ",
    //        new[] { line.Item.Author, line.Item.AuthorYear }
    //            .Where(x => !string.IsNullOrWhiteSpace(x)));

    //    if (!string.IsNullOrWhiteSpace(authorPart))
    //        parts.Add(authorPart);

    //    //var languagePart = string.Join(", ",
    //    //    new[] { line.Item.EngName, line.Item.GerName, line.Item.FraName, line.Item.PorName }
    //    //        .Where(x => !string.IsNullOrWhiteSpace(x)));

    //    //if (!string.IsNullOrWhiteSpace(languagePart))
    //    //    parts.Add(languagePart);

    //    TbSpeciesLine.Text = string.Join(" - ", parts);
    //}

    private void UpdateContentMode()
    {
        if (HyperlinkPanel == null || TbSpeciesLine == null)
            return;

        HyperlinkPanel.Visibility = ShowAsHyperlink
            ? Visibility.Visible
            : Visibility.Collapsed;

        TbSpeciesLine.Visibility = ShowAsHyperlink
            ? Visibility.Collapsed
            : Visibility.Visible;
    }


    private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (!ShowAsHyperlink) return;
        if (sender is not TextBlock tb) return;
        if (tb.Tag is not int id) return;

        var crumb = new BreadcrumbItem
        {
            Label = tb.Text,
            EntityType = "Species",
            EntityId = id
        };

        // 1) Primär über Delegate
        if (NavigateBreadcrumbAsync != null)
        {
            await NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (NavigateBreadcrumbAsync != null)
        {
            await NavigateBreadcrumbAsync(crumb);
            return;
        }

        await BreadcrumbNavigationRouter.NavigateAsync(crumb);
    }
}
