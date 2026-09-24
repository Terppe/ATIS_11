
//    Tbl78NamePage Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl78Name;

public sealed partial class Tbl78NamePage : Page
{

    public Tbl78NameViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl78NamePage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl78NameViewModel>();
        DataContext = ViewModel;

        Loaded += NamePage_Loaded;
    }
    private Tbl78NameViewModel Vm => (Tbl78NameViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void NamePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= NamePage_Loaded;

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

