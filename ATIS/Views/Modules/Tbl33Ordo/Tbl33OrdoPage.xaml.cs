using System.ComponentModel;
using ATIS.Views.Modules.Tbl36Subordo;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl33OrdoPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl33Ordo;

public sealed partial class Tbl33OrdoPage : Page
{

    public Tbl33OrdoViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl33OrdoPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl33OrdoViewModel>();
        DataContext = ViewModel;

        Loaded += OrdoPage_Loaded;
    }

    private Tbl33OrdoViewModel Vm => (Tbl33OrdoViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void OrdoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OrdoPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SubordoViewModel.PropertyChanged += SubordoViewModel_PropertyChanged;

        UpdateSubordoTabs(Vm.SubordoViewModel.SelectedItem != null);
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
                Vm.OrdoCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.OrdoReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SubordoViewModel.SubordoCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SubordoViewModel.SubordoReferenceViewModel.PrepareViewState();
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
    // Subordo  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSubordoTabs(bool hasSelection)
    {
        CommentsSubordoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubordoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSubordoTab.IsEnabled = hasSelection;
        ReferencesSubordoTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditOrdoTab, EditOrdoTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsOrdoTab, CommentsOrdoTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesOrdoTab, ReferencesOrdoTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SubordoTab, SubordoTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSubordoTab, CommentsSubordoTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSubordoTab, ReferencesSubordoTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditOrdoContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsOrdoContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesOrdoContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SubordoContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubordoContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubordoContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SubordoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl36SubordoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubordoTabs(Vm.SubordoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl33OrdoViewModel vm)
            vm.SubordoViewModel.PropertyChanged -= SubordoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
