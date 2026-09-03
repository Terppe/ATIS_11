using System.ComponentModel;
using ATIS.Views.Modules.Tbl42Superfamily;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl39InfraordoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl39Infraordo;

public sealed partial class Tbl39InfraordoPage : Page
{

    public Tbl39InfraordoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl39InfraordoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl39InfraordoViewModel>();
        DataContext = ViewModel;

        Loaded += InfraordoPage_Loaded;
    }

    private Tbl39InfraordoViewModel Vm => (Tbl39InfraordoViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void InfraordoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfraordoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SuperfamilyViewModel.PropertyChanged += SuperfamilyViewModel_PropertyChanged;

        UpdateSuperfamilyTabs(Vm.SuperfamilyViewModel.SelectedItem != null);
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
                Vm.InfraordoCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.InfraordoReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SuperfamilyViewModel.SuperfamilyCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SuperfamilyViewModel.SuperfamilyReferenceViewModel.PrepareViewState();
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
    // Superfamily  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSuperfamilyTabs(bool hasSelection)
    {
        CommentsSuperfamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperfamilyTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSuperfamilyTab.IsEnabled = hasSelection;
        ReferencesSuperfamilyTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditInfraordoTab, EditInfraordoTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsInfraordoTab, CommentsInfraordoTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesInfraordoTab, ReferencesInfraordoTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SuperfamilyTab, SuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSuperfamilyTab, CommentsSuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSuperfamilyTab, ReferencesSuperfamilyTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditInfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperfamilyContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SuperfamilyViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl42SuperfamilyViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSuperfamilyTabs(Vm.SuperfamilyViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl39InfraordoViewModel vm)
            vm.SuperfamilyViewModel.PropertyChanged -= SuperfamilyViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
