using System.ComponentModel;
using ATIS.Views.Modules.Tbl27Infraclass;


//    Tbl24SubclassPage Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl24Subclass;

public sealed partial class Tbl24SubclassPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl24SubclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl24SubclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl24SubclassViewModel>();
        DataContext = ViewModel;

        Loaded += SubclassPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsInfraclassPivot.Header;
        _referencesHeaderText = ReferencesInfraclassPivot.Header;

        // Start: ausblenden
        UpdateInfraclassTabs(false);

        Vm.InfraclassViewModel.PropertyChanged += InfraclassViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateInfraclassTabs(Vm.InfraclassViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Infraclass VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateInfraclassTabs(bool hasSelection)
    {
        CommentsInfraclassPivot.IsEnabled = hasSelection;
        ReferencesInfraclassPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsInfraclassPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesInfraclassPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Infraclass
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void InfraclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl27InfraclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfraclassTabs(Vm.InfraclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl24SubclassViewModel Vm => (Tbl24SubclassViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl24SubclassViewModel vm)
            vm.InfraclassViewModel.PropertyChanged -= InfraclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
