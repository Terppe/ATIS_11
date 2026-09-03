using System.ComponentModel;
using ATIS.Views.Modules.Tbl27Infraclass;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl24SubclassPage Skriptdatum:  15.08.2026  18:32    

namespace ATIS.Views.Modules.Tbl24Subclass;

public sealed partial class Tbl24SubclassPage : Page
{

    public Tbl24SubclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl24SubclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl24SubclassViewModel>();
        DataContext = ViewModel;

        Loaded += SubclassPage_Loaded;
    }

    private Tbl24SubclassViewModel Vm => (Tbl24SubclassViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SubclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.InfraclassViewModel.PropertyChanged += InfraclassViewModel_PropertyChanged;

        UpdateInfraclassTabs(Vm.InfraclassViewModel.SelectedItem != null);
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
                Vm.SubclassCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SubclassReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.InfraclassViewModel.InfraclassCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.InfraclassViewModel.InfraclassReferenceViewModel.PrepareViewState();
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
    // Infraclass  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateInfraclassTabs(bool hasSelection)
    {
        CommentsInfraclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsInfraclassTab.IsEnabled = hasSelection;
        ReferencesInfraclassTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSubclassTab, EditSubclassTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSubclassTab, CommentsSubclassTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSubclassTab, ReferencesSubclassTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(InfraclassTab, InfraclassTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsInfraclassTab, CommentsInfraclassTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesInfraclassTab, ReferencesInfraclassTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSubclassContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubclassContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubclassContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        InfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfraclassContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void InfraclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl27InfraclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfraclassTabs(Vm.InfraclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl24SubclassViewModel vm)
            vm.InfraclassViewModel.PropertyChanged -= InfraclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
