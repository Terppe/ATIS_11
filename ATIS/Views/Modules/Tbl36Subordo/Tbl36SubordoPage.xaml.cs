using System.ComponentModel;
using ATIS.Views.Modules.Tbl39Infraordo;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl36SubordoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl36Subordo;

public sealed partial class Tbl36SubordoPage : Page
{

    public Tbl36SubordoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl36SubordoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl36SubordoViewModel>();
        DataContext = ViewModel;

        Loaded += SubordoPage_Loaded;
    }

    private Tbl36SubordoViewModel Vm => (Tbl36SubordoViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SubordoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubordoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.InfraordoViewModel.PropertyChanged += InfraordoViewModel_PropertyChanged;

        UpdateInfraordoTabs(Vm.InfraordoViewModel.SelectedItem != null);
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
                Vm.SubordoCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SubordoReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.InfraordoViewModel.InfraordoCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.InfraordoViewModel.InfraordoReferenceViewModel.PrepareViewState();
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
    // Infraordo  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateInfraordoTabs(bool hasSelection)
    {
        CommentsInfraordoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraordoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsInfraordoTab.IsEnabled = hasSelection;
        ReferencesInfraordoTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSubordoTab, EditSubordoTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSubordoTab, CommentsSubordoTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSubordoTab, ReferencesSubordoTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(InfraordoTab, InfraordoTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsInfraordoTab, CommentsInfraordoTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesInfraordoTab, ReferencesInfraordoTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSubordoContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubordoContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubordoContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        InfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraordoContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void InfraordoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl39InfraordoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfraordoTabs(Vm.InfraordoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl36SubordoViewModel vm)
            vm.InfraordoViewModel.PropertyChanged -= InfraordoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
