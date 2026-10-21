
namespace ATIS.Views.Modules.Tbl90Reference;
public sealed partial class Tbl90ReferencePage : Page
{

    public Tbl90ReferenceViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl90ReferencePage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90ReferenceViewModel>();
        DataContext = ViewModel;

        Loaded += Tbl90ReferencePage_Loaded;
    }
    private Tbl90ReferenceViewModel Vm => (Tbl90ReferenceViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void Tbl90ReferencePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= Tbl90ReferencePage_Loaded;

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
