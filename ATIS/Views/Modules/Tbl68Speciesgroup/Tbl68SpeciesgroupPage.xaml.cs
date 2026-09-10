
//    Tbl68SpeciesgroupPage Skriptdatum:  10.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl68Speciesgroup;

public sealed partial class Tbl68SpeciesgroupPage : Page
{

    public Tbl68SpeciesgroupViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl68SpeciesgroupPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl68SpeciesgroupViewModel>();
        DataContext = ViewModel;

        Loaded += SpeciesgroupPage_Loaded;
    }

    private Tbl68SpeciesgroupViewModel Vm => (Tbl68SpeciesgroupViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SpeciesgroupPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SpeciesgroupPage_Loaded;

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
