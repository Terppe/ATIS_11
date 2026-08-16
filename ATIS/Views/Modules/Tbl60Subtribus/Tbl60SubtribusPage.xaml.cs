using System.ComponentModel;
using ATIS.Views.Modules.Tbl63Infratribus;


//    Tbl60SubtribusPage Skriptdatum:  16.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl60SubtribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl60SubtribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl60SubtribusViewModel>();
        DataContext = ViewModel;

        Loaded += SubtribusPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubtribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsInfratribusPivot.Header;
        _referencesHeaderText = ReferencesInfratribusPivot.Header;

        // Start: ausblenden
        UpdateInfratribusTabs(false);

        Vm.InfratribusViewModel.PropertyChanged += InfratribusViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateInfratribusTabs(Vm.InfratribusViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Infratribus VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateInfratribusTabs(bool hasSelection)
    {
        CommentsInfratribusPivot.IsEnabled = hasSelection;
        ReferencesInfratribusPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsInfratribusPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesInfratribusPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Infratribus
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void InfratribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl63InfratribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfratribusTabs(Vm.InfratribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl60SubtribusViewModel Vm => (Tbl60SubtribusViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl60SubtribusViewModel vm)
            vm.InfratribusViewModel.PropertyChanged -= InfratribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
