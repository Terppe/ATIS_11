using System.ComponentModel;
using ATIS.Views.Modules.Tbl30Legio;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl27InfraclassPage Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl27Infraclass;

public sealed partial class Tbl27InfraclassPage : Page
{

    public Tbl27InfraclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl27InfraclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl27InfraclassViewModel>();
        DataContext = ViewModel;

        Loaded += InfraclassPage_Loaded;
    }

    private Tbl27InfraclassViewModel Vm => (Tbl27InfraclassViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void InfraclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfraclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.LegioViewModel.PropertyChanged += LegioViewModel_PropertyChanged;

        UpdateLegioTabs(Vm.LegioViewModel.SelectedItem != null);
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
                Vm.InfraclassCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.InfraclassReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.LegioViewModel.LegioCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.LegioViewModel.LegioReferenceViewModel.PrepareViewState();
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
    // Legio  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateLegioTabs(bool hasSelection)
    {
        CommentsLegioTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesLegioTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsLegioTab.IsEnabled = hasSelection;
        ReferencesLegioTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditInfraclassTab, EditInfraclassTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsInfraclassTab, CommentsInfraclassTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesInfraclassTab, ReferencesInfraclassTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(LegioTab, LegioTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsLegioTab, CommentsLegioTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesLegioTab, ReferencesLegioTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditInfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        LegioContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsLegioContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesLegioContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void LegioViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl30LegioViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateLegioTabs(Vm.LegioViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl27InfraclassViewModel vm)
            vm.LegioViewModel.PropertyChanged -= LegioViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
