
//    Tbl81ImagePage Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl81Image;

public sealed partial class Tbl81ImagePage : Page
{

    public Tbl81ImageViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl81ImagePage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl81ImageViewModel>();
        DataContext = ViewModel;

        Loaded += ImagePage_Loaded;
    }

    private Tbl81ImageViewModel Vm => (Tbl81ImageViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void ImagePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ImagePage_Loaded;

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

