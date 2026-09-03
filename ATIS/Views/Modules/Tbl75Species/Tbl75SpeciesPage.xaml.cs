using System.ComponentModel;
using ATIS.Views.Modules.Tbl78Name;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using MimeKit;


//    Tbl75SpeciesPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesPage : Page
{

    public Tbl75SpeciesViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl75SpeciesPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl75SpeciesViewModel>();
        DataContext = ViewModel;

        Loaded += SpeciesPage_Loaded;
    }

    private Tbl75SpeciesViewModel Vm => (Tbl75SpeciesViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SpeciesPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SpeciesPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.NameViewModel.PropertyChanged += NameViewModel_PropertyChanged;

        UpdateNameTabs(Vm.NameViewModel.SelectedItem != null);
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
                Vm.SpeciesCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SpeciesReferenceViewModel.PrepareViewState();
                break;

            case 4:
    //            Vm.NameViewModel.NameCommentViewModel.PrepareViewState();
                break;

            case 5:
   //             Vm.NameViewModel.NameReferenceViewModel.PrepareViewState();
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
    // Name  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateNameTabs(bool hasSelection)
    {
        CommentsNameTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesNameTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsNameTab.IsEnabled = hasSelection;
        ReferencesNameTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSpeciesTab, EditSpeciesTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSpeciesTab, CommentsSpeciesTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSpeciesTab, ReferencesSpeciesTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(NameTab, NameTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsNameTab, CommentsNameTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesNameTab, ReferencesNameTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
  //      NameContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsNameContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesNameContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void NameViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl78NameViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateNameTabs(Vm.NameViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl75SpeciesViewModel vm)
            vm.NameViewModel.PropertyChanged -= NameViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
