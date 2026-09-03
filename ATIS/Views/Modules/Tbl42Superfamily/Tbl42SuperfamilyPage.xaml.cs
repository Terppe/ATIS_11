using System.ComponentModel;
using ATIS.Views.Modules.Tbl45Family;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl42SuperfamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl42Superfamily;

public sealed partial class Tbl42SuperfamilyPage : Page
{

    public Tbl42SuperfamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl42SuperfamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl42SuperfamilyViewModel>();
        DataContext = ViewModel;

        Loaded += SuperfamilyPage_Loaded;
    }

    private Tbl42SuperfamilyViewModel Vm => (Tbl42SuperfamilyViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SuperfamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SuperfamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.FamilyViewModel.PropertyChanged += FamilyViewModel_PropertyChanged;

        UpdateFamilyTabs(Vm.FamilyViewModel.SelectedItem != null);
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
                Vm.SuperfamilyCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SuperfamilyReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.FamilyViewModel.FamilyCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.FamilyViewModel.FamilyReferenceViewModel.PrepareViewState();
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
    // Family  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateFamilyTabs(bool hasSelection)
    {
        CommentsFamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesFamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsFamilyTab.IsEnabled = hasSelection;
        ReferencesFamilyTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSuperfamilyTab, EditSuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSuperfamilyTab, CommentsSuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSuperfamilyTab, ReferencesSuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(FamilyTab, FamilyTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsFamilyTab, CommentsFamilyTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesFamilyTab, ReferencesFamilyTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        FamilyContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsFamilyContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesFamilyContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void FamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl45FamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateFamilyTabs(Vm.FamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl42SuperfamilyViewModel vm)
            vm.FamilyViewModel.PropertyChanged -= FamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
