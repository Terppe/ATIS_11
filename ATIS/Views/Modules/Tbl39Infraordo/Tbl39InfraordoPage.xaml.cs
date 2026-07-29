using System.ComponentModel;
using ATIS.Views.Modules.Tbl42Superfamily;


//    Tbl39InfraordoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl39Infraordo;

public sealed partial class Tbl39InfraordoPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl39InfraordoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl39InfraordoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl39InfraordoViewModel>();
        DataContext = ViewModel;

        Loaded += InfraordoPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void InfraordoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfraordoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSuperfamilyPivot.Header;
        _referencesHeaderText = ReferencesSuperfamilyPivot.Header;

        // Start: ausblenden
        UpdateSuperfamilyTabs(false);

        Vm.SuperfamilyViewModel.PropertyChanged += SuperfamilyViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSuperfamilyTabs(Vm.SuperfamilyViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Superfamily VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSuperfamilyTabs(bool hasSelection)
    {
        CommentsSuperfamilyPivot.IsEnabled = hasSelection;
        ReferencesSuperfamilyPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSuperfamilyPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSuperfamilyPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Superfamily
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SuperfamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl42SuperfamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSuperfamilyTabs(Vm.SuperfamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl39InfraordoViewModel Vm => (Tbl39InfraordoViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl39InfraordoViewModel vm)
            vm.SuperfamilyViewModel.PropertyChanged -= SuperfamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
