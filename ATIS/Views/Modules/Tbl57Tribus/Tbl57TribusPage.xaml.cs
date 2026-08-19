using System.ComponentModel;
using ATIS.Views.Modules.Tbl60Subtribus;


//    Tbl57TribusPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl57Tribus;

public sealed partial class Tbl57TribusPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl57TribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl57TribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl57TribusViewModel>();
        DataContext = ViewModel;

        Loaded += TribusPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void TribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= TribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSubtribusPivot.Header;
        _referencesHeaderText = ReferencesSubtribusPivot.Header;

        // Start: ausblenden
        UpdateSubtribusTabs(false);

        Vm.SubtribusViewModel.PropertyChanged += SubtribusViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSubtribusTabs(Vm.SubtribusViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Subtribus VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSubtribusTabs(bool hasSelection)
    {
        CommentsSubtribusPivot.IsEnabled = hasSelection;
        ReferencesSubtribusPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSubtribusPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSubtribusPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Subtribus
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SubtribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl60SubtribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubtribusTabs(Vm.SubtribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl57TribusViewModel Vm => (Tbl57TribusViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl57TribusViewModel vm)
            vm.SubtribusViewModel.PropertyChanged -= SubtribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
