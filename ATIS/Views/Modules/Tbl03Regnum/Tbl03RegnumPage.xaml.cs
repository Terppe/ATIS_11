using System.ComponentModel;
using ATIS.Views.Modules.Tbl06Phylum;


//    Tbl03RegnumPage Skriptdatum:  30.12.2024  12:32      

namespace ATIS.Views.Modules.Tbl03Regnum;

public sealed partial class Tbl03RegnumPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl03RegnumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl03RegnumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl03RegnumViewModel>();
        DataContext = ViewModel;

        Loaded += RegnumPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void RegnumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RegnumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsPhylumPivot.Header;
        _referencesHeaderText = ReferencesPhylumPivot.Header;

        // Start: ausblenden
        UpdatePhylumTabs(false);

        Vm.PhylumViewModel.PropertyChanged += PhylumViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdatePhylumTabs(Vm.PhylumViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Phylum VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdatePhylumTabs(bool hasSelection)
    {
        CommentsPhylumPivot.IsEnabled = hasSelection;
        ReferencesPhylumPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsPhylumPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesPhylumPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Phylum
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void PhylumViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl06PhylumViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdatePhylumTabs(Vm.PhylumViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl03RegnumViewModel Vm => (Tbl03RegnumViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl03RegnumViewModel vm)
            vm.PhylumViewModel.PropertyChanged -= PhylumViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
