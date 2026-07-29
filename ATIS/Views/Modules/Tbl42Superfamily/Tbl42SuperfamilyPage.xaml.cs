using System.ComponentModel;
using ATIS.Views.Modules.Tbl45Family;


//    Tbl42SuperfamilyPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl42Superfamily;

public sealed partial class Tbl42SuperfamilyPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl42SuperfamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl42SuperfamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl42SuperfamilyViewModel>();
        DataContext = ViewModel;

        Loaded += SuperfamilyPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SuperfamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SuperfamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsFamilyPivot.Header;
        _referencesHeaderText = ReferencesFamilyPivot.Header;

        // Start: ausblenden
        UpdateFamilyTabs(false);

        Vm.FamilyViewModel.PropertyChanged += FamilyViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateFamilyTabs(Vm.FamilyViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Family VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateFamilyTabs(bool hasSelection)
    {
        CommentsFamilyPivot.IsEnabled = hasSelection;
        ReferencesFamilyPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsFamilyPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesFamilyPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Family
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void FamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl45FamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateFamilyTabs(Vm.FamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl42SuperfamilyViewModel Vm => (Tbl42SuperfamilyViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl42SuperfamilyViewModel vm)
            vm.FamilyViewModel.PropertyChanged -= FamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
