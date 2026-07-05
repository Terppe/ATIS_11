using System.ComponentModel;
using ATIS.Views.Modules.Tbl12Subphylum;


//    Tbl06PhylumPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl06Phylum;

public sealed partial class Tbl06PhylumPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl06PhylumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl06PhylumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl06PhylumViewModel>();
        DataContext = ViewModel;

        Loaded += PhylumPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void PhylumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= PhylumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSubphylumPivot.Header;
        _referencesHeaderText = ReferencesSubphylumPivot.Header;

        // Start: ausblenden
        UpdateSubphylumTabs(false);

        Vm.SubphylumViewModel.PropertyChanged += SubphylumViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSubphylumTabs(Vm.SubphylumViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Subphylum VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSubphylumTabs(bool hasSelection)
    {
        CommentsSubphylumPivot.IsEnabled = hasSelection;
        ReferencesSubphylumPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSubphylumPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSubphylumPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Subphylum
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SubphylumViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl12SubphylumViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubphylumTabs(Vm.SubphylumViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl06PhylumViewModel Vm => (Tbl06PhylumViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl06PhylumViewModel vm)
            vm.SubphylumViewModel.PropertyChanged -= SubphylumViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
