using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DetailViewModels;
using ATIS.Views.Taxonomy;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace ATIS.Views.Report.DataTemplateHyperlink;
public sealed partial class TemplateTbl12SubphylumWithOptionalHyperlinkControl : UserControl
{
    public Tbl12SubphylumDetailViewModel? ViewModel => DataContext as Tbl12SubphylumDetailViewModel;

    public TemplateTbl12SubphylumWithOptionalHyperlinkControl()
    {
        InitializeComponent();
        Loaded += TemplateTbl12SubphylumWithOptionalHyperlinkControl_Loaded;
        DataContextChanged += TemplateTbl12SubphylumWithOptionalHyperlinkControl_DataContextChanged;
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
            typeof(TemplateTbl12SubphylumWithOptionalHyperlinkControl),
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
            typeof(TemplateTbl12SubphylumWithOptionalHyperlinkControl),
            new PropertyMetadata(true, OnShowAsHyperlinkChanged));

    private static void OnShowAsHyperlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl12SubphylumWithOptionalHyperlinkControl control)
        {
            control.UpdateHyperlinkVisualState();
            control.UpdateContentMode();
        }
    }

    private void TemplateTbl12SubphylumWithOptionalHyperlinkControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateHyperlinkVisualState();
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void TemplateTbl12SubphylumWithOptionalHyperlinkControl_DataContextChanged(FrameworkElement sender,
        DataContextChangedEventArgs args)
    {
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void UpdateHyperlinkVisualState()
    {
        if (TbSubphylum == null)
            return;

        if (ShowAsHyperlink)
        {
            TbSubphylum.Foreground = new SolidColorBrush(Colors.DarkBlue);
            TbSubphylum.TextDecorations = TextDecorations.Underline;
        }
        else
        {
            TbSubphylum.Foreground = new SolidColorBrush(Colors.Black);
            TbSubphylum.TextDecorations = TextDecorations.None;
        }
    }

    private void UpdateAdditionalText()
    {
        if (TbAuthorDisplay == null || TbLanguageNamesDisplay == null || TbSubphylumLine == null)
            return;

        if (DataContext is not TaxonomyLine<Tbl12Subphylum> line || line.Item == null)
        {
            TbAuthorDisplay.Text = string.Empty;
            TbLanguageNamesDisplay.Text = string.Empty;
            TbSubphylumLine.Text = string.Empty;
            return;
        }

        TbAuthorDisplay.Text = TaxonomyDisplayFormatter.BuildPrefixedParts(
            " - ",
            line.Item.Author,
            line.Item.AuthorYear);

        TbLanguageNamesDisplay.Text = TaxonomyDisplayFormatter.BuildPrefixedList(
            " - ",
            line.Item.EngName,
            line.Item.GerName,
            line.Item.FraName,
            line.Item.PorName);

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(line.Item.SubphylumName))
            parts.Add(line.Item.SubphylumName);

        var authorPart = string.Join(" ",
            new[] { line.Item.Author, line.Item.AuthorYear }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        if (!string.IsNullOrWhiteSpace(authorPart))
            parts.Add(authorPart);

        var languagePart = string.Join(", ",
            new[] { line.Item.EngName, line.Item.GerName, line.Item.FraName, line.Item.PorName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        if (!string.IsNullOrWhiteSpace(languagePart))
            parts.Add(languagePart);

        TbSubphylumLine.Text = string.Join(" - ", parts);
    }

    private void UpdateContentMode()
    {
        if (HyperlinkPanel == null || TbSubphylumLine == null)
            return;

        HyperlinkPanel.Visibility = ShowAsHyperlink
            ? Visibility.Visible
            : Visibility.Collapsed;

        TbSubphylumLine.Visibility = ShowAsHyperlink
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
            EntityType = "Subphylum",
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
