using System.ComponentModel;
using ATIS.Views.Modules.Tbl54Supertribus;


//    Tbl51InfrafamilyPage Skriptdatum:  21.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public sealed partial class Tbl51InfrafamilyPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl51InfrafamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl51InfrafamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl51InfrafamilyViewModel>();
        DataContext = ViewModel;

        Loaded += InfrafamilyPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void InfrafamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfrafamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSupertribusPivot.Header;
        _referencesHeaderText = ReferencesSupertribusPivot.Header;

        // Start: ausblenden
        UpdateSupertribusTabs(false);

        Vm.SupertribusViewModel.PropertyChanged += SupertribusViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSupertribusTabs(Vm.SupertribusViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Supertribus VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSupertribusTabs(bool hasSelection)
    {
        CommentsSupertribusPivot.IsEnabled = hasSelection;
        ReferencesSupertribusPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSupertribusPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSupertribusPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Supertribus
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SupertribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl54SupertribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSupertribusTabs(Vm.SupertribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl51InfrafamilyViewModel Vm => (Tbl51InfrafamilyViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl51InfrafamilyViewModel vm)
            vm.SupertribusViewModel.PropertyChanged -= SupertribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
