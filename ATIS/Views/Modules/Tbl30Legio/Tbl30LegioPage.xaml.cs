using System.ComponentModel;
using ATIS.Views.Modules.Tbl33Ordo;


//    Tbl30LegioPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl30Legio;

public sealed partial class Tbl30LegioPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl30LegioViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl30LegioPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl30LegioViewModel>();
        DataContext = ViewModel;

        Loaded += LegioPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void LegioPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LegioPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsOrdoPivot.Header;
        _referencesHeaderText = ReferencesOrdoPivot.Header;

        // Start: ausblenden
        UpdateOrdoTabs(false);

        Vm.OrdoViewModel.PropertyChanged += OrdoViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateOrdoTabs(Vm.OrdoViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Ordo VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateOrdoTabs(bool hasSelection)
    {
        CommentsOrdoPivot.IsEnabled = hasSelection;
        ReferencesOrdoPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsOrdoPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesOrdoPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Ordo
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void OrdoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl33OrdoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateOrdoTabs(Vm.OrdoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl30LegioViewModel Vm => (Tbl30LegioViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl30LegioViewModel vm)
            vm.OrdoViewModel.PropertyChanged -= OrdoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
