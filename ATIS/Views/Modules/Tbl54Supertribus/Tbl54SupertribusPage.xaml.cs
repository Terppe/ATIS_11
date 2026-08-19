using System.ComponentModel;
using ATIS.Views.Modules.Tbl57Tribus;


//    Tbl54SupertribusPage Skriptdatum:  18.18.2026  10:32    

namespace ATIS.Views.Modules.Tbl54Supertribus;

public sealed partial class Tbl54SupertribusPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl54SupertribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl54SupertribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl54SupertribusViewModel>();
        DataContext = ViewModel;

        Loaded += SupertribusPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SupertribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SupertribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsTribusPivot.Header;
        _referencesHeaderText = ReferencesTribusPivot.Header;

        // Start: ausblenden
        UpdateTribusTabs(false);

        Vm.TribusViewModel.PropertyChanged += TribusViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateTribusTabs(Vm.TribusViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Tribus VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateTribusTabs(bool hasSelection)
    {
        CommentsTribusPivot.IsEnabled = hasSelection;
        ReferencesTribusPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsTribusPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesTribusPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Tribus
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void TribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl57TribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateTribusTabs(Vm.TribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl54SupertribusViewModel Vm => (Tbl54SupertribusViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl54SupertribusViewModel vm)
            vm.TribusViewModel.PropertyChanged -= TribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
