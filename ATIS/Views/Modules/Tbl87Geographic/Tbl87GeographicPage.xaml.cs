
//    Tbl87GeographicPage Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public sealed partial class Tbl87GeographicPage : Page
{

    public Tbl87GeographicViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl87GeographicPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl87GeographicViewModel>();
        DataContext = ViewModel;

        Loaded += GeographicPage_Loaded;
    }

    private Tbl87GeographicViewModel Vm => (Tbl87GeographicViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void GeographicPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= GeographicPage_Loaded;

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

