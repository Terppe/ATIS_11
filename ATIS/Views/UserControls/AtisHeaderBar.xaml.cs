using Windows.Foundation;

namespace ATIS.Views.UserControls;

public sealed partial class AtisHeaderBar : UserControl
{
    public AtisHeaderBar()
    {
        InitializeComponent(); 
    }
    

    public string PageTitle
    {
        get => (string)GetValue(PageTitleProperty);
        set => SetValue(PageTitleProperty, value);
    }
    public static readonly DependencyProperty PageTitleProperty =
        DependencyProperty.Register(nameof(PageTitle), typeof(string), typeof(AtisHeaderBar), new PropertyMetadata(""));

    public string DatasetText
    {
        get => (string)GetValue(DatasetTextProperty);
        set => SetValue(DatasetTextProperty, value);
    }
    public static readonly DependencyProperty DatasetTextProperty =
        DependencyProperty.Register(nameof(DatasetText), typeof(string), typeof(AtisHeaderBar), new PropertyMetadata(""));

    public string DatasetCount
    {
        get => (string)GetValue(DatasetCountProperty);
        set => SetValue(DatasetCountProperty, value);
    }
    public static readonly DependencyProperty DatasetCountProperty =
        DependencyProperty.Register(nameof(DatasetCount), typeof(string), typeof(AtisHeaderBar), new PropertyMetadata(""));

    public string SearchPlaceholder
    {
        get => (string)GetValue(SearchPlaceholderProperty);
        set => SetValue(SearchPlaceholderProperty, value);
    }
    public static readonly DependencyProperty SearchPlaceholderProperty =
        DependencyProperty.Register(nameof(SearchPlaceholder), typeof(string), typeof(AtisHeaderBar), new PropertyMetadata(""));

    public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> SearchQuerySubmitted;
    public void FocusSearchBox()
    {
        SearchBox.Focus(FocusState.Programmatic);
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        SearchQuerySubmitted?.Invoke(sender, args);
    }
}


//using Windows.Foundation;

//namespace ATIS.Views.UserControls;

//public sealed partial class AtisHeaderBar : UserControl
//{
//    public AtisHeaderBar()
//    {
//        InitializeComponent();

//    }
// //   public string Title => PageTitleLocalizerHelper.Text;

//    public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

//    public void FocusSearchBox()
//    {
//        SearchBox.Focus(FocusState.Programmatic);
//    }
//    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
//    {
//        QuerySubmitted?.Invoke(sender, args);
//    }

//    public string DatasetCountValue
//    {
//        get => (string)GetValue(DatasetCountValueProperty);
//        set => SetValue(DatasetCountValueProperty, value);
//    }

//    public static readonly DependencyProperty DatasetCountValueProperty =
//        DependencyProperty.Register(
//            nameof(DatasetCountValue),
//            typeof(string),
//            typeof(AtisHeaderBar),
//            new PropertyMetadata(""));

//    public string PageTitle { get => (string)GetValue(PageTitleProperty); set => SetValue(PageTitleProperty, value); }
//    public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(nameof(PageTitle), typeof(string), typeof(AtisHeaderBar), new PropertyMetadata(string.Empty));
//}
