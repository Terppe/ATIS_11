using System.ComponentModel;
using ATIS.Views.Modules.Tbl66Genus;


//    Tbl63InfratribusPage Skriptdatum:  16.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl63InfratribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl63InfratribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl63InfratribusViewModel>();
        DataContext = ViewModel;

        Loaded += InfratribusPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void InfratribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfratribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsGenusPivot.Header;
        _referencesHeaderText = ReferencesGenusPivot.Header;

        // Start: ausblenden
        UpdateGenusTabs(false);

        Vm.GenusViewModel.PropertyChanged += GenusViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateGenusTabs(Vm.GenusViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Genus VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateGenusTabs(bool hasSelection)
    {
        CommentsGenusPivot.IsEnabled = hasSelection;
        ReferencesGenusPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsGenusPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesGenusPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Genus
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void GenusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl66GenusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateGenusTabs(Vm.GenusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl63InfratribusViewModel Vm => (Tbl63InfratribusViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl63InfratribusViewModel vm)
            vm.GenusViewModel.PropertyChanged -= GenusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
