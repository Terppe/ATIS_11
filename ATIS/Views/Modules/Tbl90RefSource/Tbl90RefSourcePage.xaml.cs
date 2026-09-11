//    Tbl90RefSourcePage Skriptdatum:   11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefSource;

public sealed partial class Tbl90RefSourcePage : Page
{

    public Tbl90RefSourceViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl90RefSourcePage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefSourceViewModel>();
        DataContext = ViewModel;

        Loaded += RefSourcePage_Loaded;
    }

    private Tbl90RefSourceViewModel Vm => (Tbl90RefSourceViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void RefSourcePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RefSourcePage_Loaded;

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
