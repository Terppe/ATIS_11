using System.ComponentModel;
using ATIS.Views.Modules.Tbl48Subfamily;


//    Tbl45FamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl45Family;

public sealed partial class Tbl45FamilyPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl45FamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl45FamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl45FamilyViewModel>();
        DataContext = ViewModel;

        Loaded += FamilyPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void FamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= FamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSubfamilyPivot.Header;
        _referencesHeaderText = ReferencesSubfamilyPivot.Header;

        // Start: ausblenden
        UpdateSubfamilyTabs(false);

        Vm.SubfamilyViewModel.PropertyChanged += SubfamilyViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSubfamilyTabs(Vm.SubfamilyViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Subfamily VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSubfamilyTabs(bool hasSelection)
    {
        CommentsSubfamilyPivot.IsEnabled = hasSelection;
        ReferencesSubfamilyPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSubfamilyPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSubfamilyPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Subfamily
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SubfamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl48SubfamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubfamilyTabs(Vm.SubfamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl45FamilyViewModel Vm => (Tbl45FamilyViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl45FamilyViewModel vm)
            vm.SubfamilyViewModel.PropertyChanged -= SubfamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
