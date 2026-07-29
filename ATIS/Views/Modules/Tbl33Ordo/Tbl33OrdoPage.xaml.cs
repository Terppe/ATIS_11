using System.ComponentModel;
using ATIS.Views.Modules.Tbl36Subordo;


//    Tbl33OrdoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl33Ordo;

public sealed partial class Tbl33OrdoPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl33OrdoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl33OrdoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl33OrdoViewModel>();
        DataContext = ViewModel;

        Loaded += OrdoPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void OrdoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OrdoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSubordoPivot.Header;
        _referencesHeaderText = ReferencesSubordoPivot.Header;

        // Start: ausblenden
        UpdateSubordoTabs(false);

        Vm.SubordoViewModel.PropertyChanged += SubordoViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSubordoTabs(Vm.SubordoViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Subordo VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSubordoTabs(bool hasSelection)
    {
        CommentsSubordoPivot.IsEnabled = hasSelection;
        ReferencesSubordoPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSubordoPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSubordoPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Subordo
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SubordoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl36SubordoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubordoTabs(Vm.SubordoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl33OrdoViewModel Vm => (Tbl33OrdoViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl33OrdoViewModel vm)
            vm.SubordoViewModel.PropertyChanged -= SubordoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
