using System.ComponentModel;
using ATIS.Views.Modules.Tbl21Class;


//    Tbl18SuperclassPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl18Superclass;

public sealed partial class Tbl18SuperclassPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl18SuperclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl18SuperclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl18SuperclassViewModel>();
        DataContext = ViewModel;

        Loaded += SuperclassPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SuperclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SuperclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        // Wichtig: lokalisierte Header einmal merken
        _commentsHeaderText = CommentsClassPivot.Header;
        _referencesHeaderText = ReferencesClassPivot.Header;

        // Start: ausblenden
        UpdateClassTabs(false);

        Vm.ClassViewModel.PropertyChanged += ClassViewModel_PropertyChanged;

        // Falls schon selektiert, wieder einblenden
        UpdateClassTabs(Vm.ClassViewModel.SelectedItem != null);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Class VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateClassTabs(bool hasSelection)
    {
        CommentsClassPivot.IsEnabled = hasSelection;
        ReferencesClassPivot.IsEnabled = hasSelection;

        // Nicht mehr mit Uid zur Laufzeit arbeiten, sondern mit gecachtem Header und WinUI3Localizer

        CommentsClassPivot.Header = hasSelection ? _commentsHeaderText : string.Empty;
        ReferencesClassPivot.Header = hasSelection ? _referencesHeaderText : string.Empty;

        if (!hasSelection && DetailPivot.SelectedIndex >= 4)
            DetailPivot.SelectedIndex = 3; // Class
    }

    // ---------------------------------------------------------                  
    // PROPERTCHANGED
    // ---------------------------------------------------------                  
    private void ClassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl21ClassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateClassTabs(Vm.ClassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    private Tbl18SuperclassViewModel Vm => (Tbl18SuperclassViewModel)DataContext;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl18SuperclassViewModel vm)
            vm.ClassViewModel.PropertyChanged -= ClassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
