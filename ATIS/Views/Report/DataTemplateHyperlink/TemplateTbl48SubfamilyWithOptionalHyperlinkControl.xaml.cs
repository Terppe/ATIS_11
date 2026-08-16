using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DetailViewModels;
using ATIS.Views.Taxonomy;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using Microsoft.UI;

namespace ATIS.Views.Report.DataTemplateHyperlink;


public sealed partial class TemplateTbl48SubfamilyWithOptionalHyperlinkControl : UserControl
{
    public Tbl48SubfamilyDetailViewModel? ViewModel => DataContext as Tbl48SubfamilyDetailViewModel;

    public TemplateTbl48SubfamilyWithOptionalHyperlinkControl()
    {
        InitializeComponent();
        Loaded += TemplateTbl48SubfamilyWithOptionalHyperlinkControl_Loaded;
        DataContextChanged += TemplateTbl48SubfamilyWithOptionalHyperlinkControl_DataContextChanged;
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
            typeof(TemplateTbl48SubfamilyWithOptionalHyperlinkControl),
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
            typeof(TemplateTbl48SubfamilyWithOptionalHyperlinkControl),
            new PropertyMetadata(true, OnShowAsHyperlinkChanged));

    private static void OnShowAsHyperlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl48SubfamilyWithOptionalHyperlinkControl control)
        {
            control.UpdateHyperlinkVisualState();
            control.UpdateContentMode();
        }
    }

    private void TemplateTbl48SubfamilyWithOptionalHyperlinkControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateHyperlinkVisualState();
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void TemplateTbl48SubfamilyWithOptionalHyperlinkControl_DataContextChanged(FrameworkElement sender,
        DataContextChangedEventArgs args)
    {
        UpdateHyperlinkVisualState();
        UpdateAdditionalText();
        UpdateContentMode();
    }

    private void UpdateHyperlinkVisualState()
    {
        TbSubfamily.Foreground = ShowAsHyperlink
            ? new SolidColorBrush(Colors.DarkBlue)
            : (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

        TbSubfamily.TextDecorations = ShowAsHyperlink ? TextDecorations.Underline : TextDecorations.None;
        TbSubfamily.IsHitTestVisible = ShowAsHyperlink;
    }

    private void UpdateAdditionalText()
    {
        if (TbAuthorDisplay == null || TbLanguageNamesDisplay == null || TbSubfamilyLine == null)
            return;

        if (DataContext is not TaxonomyLine<Tbl48Subfamily> line || line.Item == null)
        {
            TbAuthorDisplay.Text = string.Empty;
            TbLanguageNamesDisplay.Text = string.Empty;
            TbSubfamilyLine.Text = string.Empty;
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

        if (!string.IsNullOrWhiteSpace(line.Item.SubfamilyName))
            parts.Add(line.Item.SubfamilyName);

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

        TbSubfamilyLine.Text = string.Join(" - ", parts);
    }

    private void UpdateContentMode()
    {
        if (HyperlinkPanel == null || TbSubfamilyLine == null)
            return;

        HyperlinkPanel.Visibility = ShowAsHyperlink
            ? Visibility.Visible
            : Visibility.Collapsed;

        TbSubfamilyLine.Visibility = ShowAsHyperlink
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
            EntityType = "Subfamily",
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
