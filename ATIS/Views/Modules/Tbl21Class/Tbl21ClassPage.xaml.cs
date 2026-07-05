using System.ComponentModel;
using ATIS.Views.Modules.Tbl24Subclass;


//    Tbl21ClassPage Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl21Class;

public sealed partial class Tbl21ClassPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl21ClassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl21ClassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl21ClassViewModel>();
        DataContext = ViewModel;

        Loaded += ClassPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void ClassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ClassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSubclassPivot.Header;
        _referencesHeaderText = ReferencesSubclassPivot.Header;

        // Start: ausblenden
        UpdateSubclassTabs(false);

        Vm.SubclassViewModel.PropertyChanged += SubclassViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSubclassTabs(Vm.SubclassViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Subclass VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSubclassTabs(bool hasSelection)
    {
        CommentsSubclassPivot.IsEnabled = hasSelection;
        ReferencesSubclassPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSubclassPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSubclassPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Subclass
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SubclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl24SubclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubclassTabs(Vm.SubclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl21ClassViewModel Vm => (Tbl21ClassViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl21ClassViewModel vm)
            vm.SubclassViewModel.PropertyChanged -= SubclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
