using ATIS.Helpers;
using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DetailViewModels;
using ATIS.Views.Taxonomy;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace ATIS.Views.Report.DataTemplateHyperlink;

public sealed partial class TemplateTbl03RegnumWithOptionalHyperlinkControl : UserControl
{
    public Tbl03RegnumDetailViewModel? ViewModel => DataContext as Tbl03RegnumDetailViewModel;

    public TemplateTbl03RegnumWithOptionalHyperlinkControl()
    {
        InitializeComponent();
        Loaded += TemplateTbl03RegnumWithOptionalHyperlinkControl_Loaded;
        DataContextChanged += TemplateTbl03RegnumWithOptionalHyperlinkControl_DataContextChanged;
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
            typeof(TemplateTbl03RegnumWithOptionalHyperlinkControl),
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
            typeof(TemplateTbl03RegnumWithOptionalHyperlinkControl),
            new PropertyMetadata(true, OnShowAsHyperlinkChanged));

    private static void OnShowAsHyperlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TemplateTbl03RegnumWithOptionalHyperlinkControl control)
        {
            control.UpdateHyperlinkVisualState();
            control.UpdateContentMode();
        }
    }

    private void TemplateTbl03RegnumWithOptionalHyperlinkControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateHyperlinkVisualState();
        UpdateContentMode();
        UpdateCombinedText();
    }

    private void TemplateTbl03RegnumWithOptionalHyperlinkControl_DataContextChanged(FrameworkElement sender,
        DataContextChangedEventArgs args)
    {
        UpdateHyperlinkVisualState();
        UpdateContentMode();
        UpdateCombinedText();
    }

    private void UpdateHyperlinkVisualState()
    {
        if (TbRegnumLink == null)
            return;

        if (ShowAsHyperlink)
        {
            TbRegnumLink.Foreground = new SolidColorBrush(Colors.DarkBlue);
            TbRegnumLink.TextDecorations = TextDecorations.Underline;
        }
        else
        {
            TbRegnumLink.Foreground = new SolidColorBrush(Colors.Black);
            TbRegnumLink.TextDecorations = TextDecorations.None;
        }
    }
    private void UpdateContentMode()
    {
        if (TbRegnumLink == null || TbRegnumLine == null)
            return;

        TbRegnumLink.Visibility = ShowAsHyperlink
            ? Visibility.Visible
            : Visibility.Collapsed;

        TbRegnumLine.Visibility = ShowAsHyperlink
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
    private void UpdateCombinedText()
    {
        if (TbRegnumLink == null || TbRegnumLine == null)
            return;

        if (DataContext is not TaxonomyLine<Tbl03Regnum> line || line.Item == null)
        {
            TbRegnumLink.Text = string.Empty;
            TbRegnumLine.Text = string.Empty;
            return;
        }

        var parts = new[]
            {
                line.Item.RegnumName,
                line.Item.Subregnum
            }
            .Where(x => !string.IsNullOrWhiteSpace(x));

        var combinedText = string.Join(" ", parts);

        TbRegnumLink.Text = combinedText;
        TbRegnumLine.Text = combinedText;
    }



    private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (!ShowAsHyperlink) return;
        if (sender is not TextBlock tb) return;
        if (tb.Tag is not int id) return;

        var crumb = new BreadcrumbItem
        {
            Label = tb.Text,
            EntityType = "Regnum",
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


//public sealed partial class TemplateTbl03RegnumWithOptionalHyperlinkControl : UserControl
//{
//    public Tbl03RegnumDetailViewModel? ViewModel => DataContext as Tbl03RegnumDetailViewModel;

//    public TemplateTbl03RegnumWithOptionalHyperlinkControl()
//    {
//        InitializeComponent();
//        Loaded += TemplateTbl03RegnumWithOptionalHyperlinkControl_Loaded;
//        DataContextChanged += TemplateTbl03RegnumWithOptionalHyperlinkControl_DataContextChanged;
//    }

//    public bool ShowAsHyperlink
//    {
//        get => (bool)GetValue(ShowAsHyperlinkProperty);
//        set => SetValue(ShowAsHyperlinkProperty, value);
//    }

//    public static readonly DependencyProperty ShowAsHyperlinkProperty =
//        DependencyProperty.Register(
//            nameof(ShowAsHyperlink),
//            typeof(bool),
//            typeof(TemplateTbl03RegnumWithOptionalHyperlinkControl),
//            new PropertyMetadata(true, OnShowAsHyperlinkChanged));

//    private static void OnShowAsHyperlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        if (d is TemplateTbl03RegnumWithOptionalHyperlinkControl control)
//        {
//            control.UpdateHyperlinkVisualState();
//            control.UpdateContentMode();
//            control.UpdateCombinedText();
//        }
//    }

//    private void TemplateTbl03RegnumWithOptionalHyperlinkControl_Loaded(object sender, RoutedEventArgs e)
//    {
//        UpdateHyperlinkVisualState();
//        UpdateContentMode();
//        UpdateCombinedText();
//    }

//    private void TemplateTbl03RegnumWithOptionalHyperlinkControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
//    {
//        UpdateContentMode();
//        UpdateCombinedText();
//    }

//    private void UpdateHyperlinkVisualState()
//    {
//        if (TbRegnumLink == null)
//            return;

//        if (ShowAsHyperlink)
//        {
//            TbRegnumLink.Foreground = new SolidColorBrush(Colors.DarkBlue);
//            TbRegnumLink.TextDecorations = TextDecorations.Underline;
//        }
//        else
//        {
//            TbRegnumLink.Foreground = new SolidColorBrush(Colors.Black);
//            TbRegnumLink.TextDecorations = TextDecorations.None;
//        }
//    }
//    private void UpdateContentMode()
//    {
//        if (TbRegnumLink == null || TbRegnumLine == null)
//            return;

//        TbRegnumLink.Visibility = ShowAsHyperlink
//            ? Visibility.Visible
//            : Visibility.Collapsed;

//        TbRegnumLine.Visibility = ShowAsHyperlink
//            ? Visibility.Collapsed
//            : Visibility.Visible;
//    }
//    private void UpdateCombinedText()
//    {
//        if (TbRegnumLink == null || TbRegnumLine == null)
//            return;

//        if (DataContext is not TaxonomyLine<Tbl03Regnum> line || line.Item == null)
//        {
//            TbRegnumLink.Text = string.Empty;
//            TbRegnumLine.Text = string.Empty;
//            return;
//        }

//        var parts = new[]
//            {
//                line.Item.RegnumName,
//                line.Item.Subregnum
//            }
//            .Where(x => !string.IsNullOrWhiteSpace(x));

//        var combinedText = string.Join(" ", parts);

//        TbRegnumLink.Text = combinedText;
//        TbRegnumLine.Text = combinedText;
//    }
//    private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
//    {
//        if (!ShowAsHyperlink) return;
//        if (sender is not TextBlock tb) return;
//        if (ViewModel is null) return;

//        await TaxonomyNavigationHelper.NavigateFromTextBlockAsync(
//            tb,
//            entityType: "Regnum", // pro Control anpassen
//            navigateBreadcrumbAsync: ViewModel.NavigateBreadcrumbAsync);
//    }
//}
