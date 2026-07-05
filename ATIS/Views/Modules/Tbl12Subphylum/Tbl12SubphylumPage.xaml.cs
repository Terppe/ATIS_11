using System.ComponentModel;
using ATIS.Views.Modules.Tbl18Superclass;


//    Tbl12SubphylumPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl12Subphylum;

public sealed partial class Tbl12SubphylumPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl12SubphylumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl12SubphylumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl12SubphylumViewModel>();
        DataContext = ViewModel;

        Loaded += SubphylumPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubphylumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubphylumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsSuperclassPivot.Header;
        _referencesHeaderText = ReferencesSuperclassPivot.Header;

        // Start: ausblenden
        UpdateSuperclassTabs(false);

        Vm.SuperclassViewModel.PropertyChanged += SuperclassViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateSuperclassTabs(Vm.SuperclassViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Superclass VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSuperclassTabs(bool hasSelection)
    {
        CommentsSuperclassPivot.IsEnabled = hasSelection;
        ReferencesSuperclassPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsSuperclassPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesSuperclassPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Superclass
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void SuperclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl18SuperclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSuperclassTabs(Vm.SuperclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl12SubphylumViewModel Vm => (Tbl12SubphylumViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl12SubphylumViewModel vm)
            vm.SuperclassViewModel.PropertyChanged -= SuperclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
