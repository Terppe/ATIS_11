using System.ComponentModel;
using ATIS.Views.Modules.Tbl51Infrafamily;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl48SubfamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl48Subfamily;

public sealed partial class Tbl48SubfamilyPage : Page
{

    public Tbl48SubfamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl48SubfamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl48SubfamilyViewModel>();
        DataContext = ViewModel;

        Loaded += SubfamilyPage_Loaded;
    }

    private Tbl48SubfamilyViewModel Vm => (Tbl48SubfamilyViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SubfamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubfamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.InfrafamilyViewModel.PropertyChanged += InfrafamilyViewModel_PropertyChanged;

        UpdateInfrafamilyTabs(Vm.InfrafamilyViewModel.SelectedItem != null);
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
                Vm.SubfamilyCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SubfamilyReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.InfrafamilyViewModel.InfrafamilyCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.InfrafamilyViewModel.InfrafamilyReferenceViewModel.PrepareViewState();
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
    // Infrafamily  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateInfrafamilyTabs(bool hasSelection)
    {
        CommentsInfrafamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfrafamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsInfrafamilyTab.IsEnabled = hasSelection;
        ReferencesInfrafamilyTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSubfamilyTab, EditSubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSubfamilyTab, CommentsSubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSubfamilyTab, ReferencesSubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(InfrafamilyTab, InfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsInfrafamilyTab, CommentsInfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesInfrafamilyTab, ReferencesInfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        InfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void InfrafamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl51InfrafamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfrafamilyTabs(Vm.InfrafamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl48SubfamilyViewModel vm)
            vm.InfrafamilyViewModel.PropertyChanged -= InfrafamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
