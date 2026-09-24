//    Tbl84SynonymPage Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl84Synonym;

public sealed partial class Tbl84SynonymPage : Page
{

    public Tbl84SynonymViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl84SynonymPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl84SynonymViewModel>();
        DataContext = ViewModel;

        Loaded += SynonymPage_Loaded;
    }

    private Tbl84SynonymViewModel Vm => (Tbl84SynonymViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SynonymPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SynonymPage_Loaded;

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
