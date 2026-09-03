using System.ComponentModel;
using ATIS.Views.Modules.Tbl33Ordo;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl30LegioPage Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl30Legio;

public sealed partial class Tbl30LegioPage : Page
{

    public Tbl30LegioViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl30LegioPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl30LegioViewModel>();
        DataContext = ViewModel;

        Loaded += LegioPage_Loaded;
    }

    private Tbl30LegioViewModel Vm => (Tbl30LegioViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void LegioPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LegioPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.OrdoViewModel.PropertyChanged += OrdoViewModel_PropertyChanged;

        UpdateOrdoTabs(Vm.OrdoViewModel.SelectedItem != null);
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
                Vm.LegioCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.LegioReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.OrdoViewModel.OrdoCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.OrdoViewModel.OrdoReferenceViewModel.PrepareViewState();
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
    // Ordo  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateOrdoTabs(bool hasSelection)
    {
        CommentsOrdoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesOrdoTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsOrdoTab.IsEnabled = hasSelection;
        ReferencesOrdoTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditLegioTab, EditLegioTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsLegioTab, CommentsLegioTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesLegioTab, ReferencesLegioTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(OrdoTab, OrdoTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsOrdoTab, CommentsOrdoTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesOrdoTab, ReferencesOrdoTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditLegioContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsLegioContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesLegioContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        OrdoContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsOrdoContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesOrdoContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void OrdoViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl33OrdoViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateOrdoTabs(Vm.OrdoViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl30LegioViewModel vm)
            vm.OrdoViewModel.PropertyChanged -= OrdoViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
