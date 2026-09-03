using System.ComponentModel;
using ATIS.Views.Modules.Tbl48Subfamily;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl45FamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl45Family;

public sealed partial class Tbl45FamilyPage : Page
{

    public Tbl45FamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl45FamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl45FamilyViewModel>();
        DataContext = ViewModel;

        Loaded += FamilyPage_Loaded;
    }

    private Tbl45FamilyViewModel Vm => (Tbl45FamilyViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void FamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= FamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SubfamilyViewModel.PropertyChanged += SubfamilyViewModel_PropertyChanged;

        UpdateSubfamilyTabs(Vm.SubfamilyViewModel.SelectedItem != null);
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
                Vm.FamilyCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.FamilyReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SubfamilyViewModel.SubfamilyCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SubfamilyViewModel.SubfamilyReferenceViewModel.PrepareViewState();
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
    // Subfamily  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSubfamilyTabs(bool hasSelection)
    {
        CommentsSubfamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubfamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSubfamilyTab.IsEnabled = hasSelection;
        ReferencesSubfamilyTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditFamilyTab, EditFamilyTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsFamilyTab, CommentsFamilyTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesFamilyTab, ReferencesFamilyTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SubfamilyTab, SubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSubfamilyTab, CommentsSubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSubfamilyTab, ReferencesSubfamilyTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditFamilyContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsFamilyContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesFamilyContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SubfamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl48SubfamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubfamilyTabs(Vm.SubfamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl45FamilyViewModel vm)
            vm.SubfamilyViewModel.PropertyChanged -= SubfamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
