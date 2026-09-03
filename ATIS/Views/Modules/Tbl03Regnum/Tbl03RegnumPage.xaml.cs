using System.ComponentModel;
using ATIS.Views.Modules.Tbl06Phylum;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl03RegnumPage Skriptdatum:  24.08.2026  12:32      

namespace ATIS.Views.Modules.Tbl03Regnum;

public sealed partial class Tbl03RegnumPage : Page
{

    public Tbl03RegnumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl03RegnumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl03RegnumViewModel>();
        DataContext = ViewModel;

        Loaded += RegnumPage_Loaded;
    }

    private Tbl03RegnumViewModel Vm => (Tbl03RegnumViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void RegnumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RegnumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.PhylumViewModel.PropertyChanged += PhylumViewModel_PropertyChanged;

        UpdatePhylumTabs(Vm.PhylumViewModel.SelectedItem != null);
        PrepareCurrentTabState();
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   
    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ---------------------------------------------------------                   
    // TABSTATE, STYLE, CLICK-EVENTS
    // ---------------------------------------------------------                   
    private void PrepareCurrentTabState()
    {
        switch (Vm.SelectedDetailTabIndex)
        {
            case 1:
                Vm.RegnumCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.RegnumReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.PhylumViewModel.PhylumCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.PhylumViewModel.PhylumReferenceViewModel.PrepareViewState();
                break;
        }
    }
    private void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }
    private void DetailTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.Tag is string tagText &&
            int.TryParse(tagText, out int index))
        {
            Vm.SelectedDetailTabIndex = index;

            PrepareCurrentTabState();
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // Phylum  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdatePhylumTabs(bool hasSelection)
    {
        CommentsPhylumTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesPhylumTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsPhylumTab.IsEnabled = hasSelection;
        ReferencesPhylumTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditRegnumTab, EditRegnumTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsRegnumTab, CommentsRegnumTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesRegnumTab, ReferencesRegnumTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(PhylumTab, PhylumTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsPhylumTab, CommentsPhylumTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesPhylumTab, ReferencesPhylumTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditRegnumContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsRegnumContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesRegnumContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        PhylumContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsPhylumContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesPhylumContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void PhylumViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl06PhylumViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdatePhylumTabs(Vm.PhylumViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl03RegnumViewModel vm)
            vm.PhylumViewModel.PropertyChanged -= PhylumViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
