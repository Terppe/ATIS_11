using System.ComponentModel;
using ATIS.Views.Modules.Tbl39Infraordo;


//    Tbl36SubordoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl36Subordo;

public sealed partial class Tbl36SubordoPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl36SubordoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl36SubordoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl36SubordoViewModel>();
        DataContext = ViewModel;

        Loaded += SubordoPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubordoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubordoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsInfraordoPivot.Header;
        _referencesHeaderText = ReferencesInfraordoPivot.Header;

        // Start: ausblenden
        UpdateInfraordoTabs(false);

        Vm.InfraordoViewModel.PropertyChanged += InfraordoViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateInfraordoTabs(Vm.InfraordoViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Infraordo VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateInfraordoTabs(bool hasSelection)
    {
        CommentsInfraordoPivot.IsEnabled = hasSelection;
        ReferencesInfraordoPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsInfraordoPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesInfraordoPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Infraordo
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void InfraordoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl39InfraordoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfraordoTabs(Vm.InfraordoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl36SubordoViewModel Vm => (Tbl36SubordoViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl36SubordoViewModel vm)
            vm.InfraordoViewModel.PropertyChanged -= InfraordoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
