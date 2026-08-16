
using System.ComponentModel;
using ATIS.Views.Modules.Tbl75Species;


//    Tbl66GenusPage Skriptdatum:  16.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl66GenusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl66GenusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl66GenusViewModel>();
        DataContext = ViewModel;

        Loaded += GenusPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void GenusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= GenusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSpeciesPivot.Header;
        _referencesHeaderText = ReferencesSpeciesPivot.Header;

        // Start: ausblenden
        UpdateSpeciesTabs(false);

        Vm.SpeciesViewModel.PropertyChanged += SpeciesViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSpeciesTabs(Vm.SpeciesViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Species VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSpeciesTabs(bool hasSelection)
    {
        CommentsSpeciesPivot.IsEnabled = hasSelection;
        ReferencesSpeciesPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSpeciesPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSpeciesPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Species
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SpeciesViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl75SpeciesViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSpeciesTabs(Vm.SpeciesViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl66GenusViewModel Vm => (Tbl66GenusViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl66GenusViewModel vm)
            vm.SpeciesViewModel.PropertyChanged -= SpeciesViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
