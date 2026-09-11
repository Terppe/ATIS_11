
//    Tbl90RefExpertPage Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public sealed partial class Tbl90RefExpertPage : Page
{
    public Tbl90RefExpertViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefExpertPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefExpertViewModel>();
        DataContext = ViewModel;

        Loaded += RefExpertPage_Loaded;
    }

    private Tbl90RefExpertViewModel Vm => (Tbl90RefExpertViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void RefExpertPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RefExpertPage_Loaded;

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
