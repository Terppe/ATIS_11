using System.ComponentModel;
using ATIS.Views.Modules.Tbl78Name;


//    Tbl75SpeciesPage Skriptdatum:  15.12.2025  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl75SpeciesViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl75SpeciesPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl75SpeciesViewModel>();
        DataContext = ViewModel;

        Loaded += SpeciesPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SpeciesPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SpeciesPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsNamePivot.Header;
        _referencesHeaderText = ReferencesNamePivot.Header;

        // Start: ausblenden
        UpdateNameTabs(false);

        Vm.NameViewModel.PropertyChanged += NameViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateNameTabs(Vm.NameViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Name VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateNameTabs(bool hasSelection)
    {
        CommentsNamePivot.IsEnabled = hasSelection;
        ReferencesNamePivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsNamePivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesNamePivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Name
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void NameViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl78NameViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateNameTabs(Vm.NameViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl75SpeciesViewModel Vm => (Tbl75SpeciesViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl75SpeciesViewModel vm)
            vm.NameViewModel.PropertyChanged -= NameViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
