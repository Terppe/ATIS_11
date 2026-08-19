using System.ComponentModel;
using ATIS.Views.Modules.Tbl51Infrafamily;


//    Tbl48SubfamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl48Subfamily;

public sealed partial class Tbl48SubfamilyPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl48SubfamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl48SubfamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl48SubfamilyViewModel>();
        DataContext = ViewModel;

        Loaded += SubfamilyPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubfamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubfamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsInfrafamilyPivot.Header;
        _referencesHeaderText = ReferencesInfrafamilyPivot.Header;

        // Start: ausblenden
        UpdateInfrafamilyTabs(false);

        Vm.InfrafamilyViewModel.PropertyChanged += InfrafamilyViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateInfrafamilyTabs(Vm.InfrafamilyViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Infrafamily VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateInfrafamilyTabs(bool hasSelection)
    {
        CommentsInfrafamilyPivot.IsEnabled = hasSelection;
        ReferencesInfrafamilyPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsInfrafamilyPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesInfrafamilyPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Infrafamily
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void InfrafamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl51InfrafamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfrafamilyTabs(Vm.InfrafamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl48SubfamilyViewModel Vm => (Tbl48SubfamilyViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl48SubfamilyViewModel vm)
            vm.InfrafamilyViewModel.PropertyChanged -= InfrafamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
