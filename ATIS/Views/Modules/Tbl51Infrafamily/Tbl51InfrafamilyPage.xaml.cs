using System.ComponentModel;
using ATIS.Views.Modules.Tbl54Supertribus;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl51InfrafamilyPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public sealed partial class Tbl51InfrafamilyPage : Page
{

    public Tbl51InfrafamilyViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl51InfrafamilyPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl51InfrafamilyViewModel>();
        DataContext = ViewModel;

        Loaded += InfrafamilyPage_Loaded;
    }

    private Tbl51InfrafamilyViewModel Vm => (Tbl51InfrafamilyViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void InfrafamilyPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfrafamilyPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SupertribusViewModel.PropertyChanged += SupertribusViewModel_PropertyChanged;

        UpdateSupertribusTabs(Vm.SupertribusViewModel.SelectedItem != null);
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
                Vm.InfrafamilyCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.InfrafamilyReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SupertribusViewModel.SupertribusCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SupertribusViewModel.SupertribusReferenceViewModel.PrepareViewState();
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
    // Supertribus  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSupertribusTabs(bool hasSelection)
    {
        CommentsSupertribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSupertribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSupertribusTab.IsEnabled = hasSelection;
        ReferencesSupertribusTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditInfrafamilyTab, EditInfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsInfrafamilyTab, CommentsInfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesInfrafamilyTab, ReferencesInfrafamilyTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SupertribusTab, SupertribusTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSupertribusTab, CommentsSupertribusTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSupertribusTab, ReferencesSupertribusTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditInfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfrafamilyContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SupertribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl54SupertribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSupertribusTabs(Vm.SupertribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl51InfrafamilyViewModel vm)
            vm.SupertribusViewModel.PropertyChanged -= SupertribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
