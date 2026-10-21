

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentPage : Page
{

    public Tbl93CommentViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl93CommentPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl93CommentViewModel>();
        DataContext = ViewModel;

        Loaded += Tbl93CommentPage_Loaded;
    }
    private Tbl93CommentViewModel Vm => (Tbl93CommentViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void Tbl93CommentPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= Tbl93CommentPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   
    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

        await ViewModel.ApplyFilterAsync(sender.Text);
    }

}

