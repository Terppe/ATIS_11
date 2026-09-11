//    Tbl90RefAuthorPage Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public sealed partial class Tbl90RefAuthorPage : Page
{

    public Tbl90RefAuthorViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl90RefAuthorPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefAuthorViewModel>();
        DataContext = ViewModel;

        Loaded += RefAuthorPage_Loaded;
    }

    private Tbl90RefAuthorViewModel Vm => (Tbl90RefAuthorViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void RefAuthorPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RefAuthorPage_Loaded;

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
