using System.ComponentModel;
using ATIS.Views.Modules.Tbl30Legio;


//    Tbl27InfraclassPage Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl27Infraclass;

public sealed partial class Tbl27InfraclassPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl27InfraclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl27InfraclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl27InfraclassViewModel>();
        DataContext = ViewModel;

        Loaded += InfraclassPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void InfraclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfraclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsLegioPivot.Header;
        _referencesHeaderText = ReferencesLegioPivot.Header;

        // Start: ausblenden
        UpdateLegioTabs(false);

        Vm.LegioViewModel.PropertyChanged += LegioViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateLegioTabs(Vm.LegioViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Legio VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateLegioTabs(bool hasSelection)
    {
        CommentsLegioPivot.IsEnabled = hasSelection;
        ReferencesLegioPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsLegioPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesLegioPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Legio
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void LegioViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl30LegioViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateLegioTabs(Vm.LegioViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl27InfraclassViewModel Vm => (Tbl27InfraclassViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl27InfraclassViewModel vm)
            vm.LegioViewModel.PropertyChanged -= LegioViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
