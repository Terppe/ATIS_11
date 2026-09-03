using System.ComponentModel;
using ATIS.Views.Modules.Tbl12Subphylum;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl06PhylumPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl06Phylum;

public sealed partial class Tbl06PhylumPage : Page
{

    public Tbl06PhylumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl06PhylumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl06PhylumViewModel>();
        DataContext = ViewModel;

        Loaded += PhylumPage_Loaded;
    }

    private Tbl06PhylumViewModel Vm => (Tbl06PhylumViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void PhylumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= PhylumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SubphylumViewModel.PropertyChanged += SubphylumViewModel_PropertyChanged;

        UpdateSubphylumTabs(Vm.SubphylumViewModel.SelectedItem != null);
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
                Vm.PhylumCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.PhylumReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SubphylumViewModel.SubphylumCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SubphylumViewModel.SubphylumReferenceViewModel.PrepareViewState();
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
    // Subphylum  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSubphylumTabs(bool hasSelection)
    {
        CommentsSubphylumTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubphylumTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSubphylumTab.IsEnabled = hasSelection;
        ReferencesSubphylumTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditPhylumTab, EditPhylumTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsPhylumTab, CommentsPhylumTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesPhylumTab, ReferencesPhylumTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SubphylumTab, SubphylumTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSubphylumTab, CommentsSubphylumTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSubphylumTab, ReferencesSubphylumTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditPhylumContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsPhylumContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesPhylumContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SubphylumViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl12SubphylumViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubphylumTabs(Vm.SubphylumViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl06PhylumViewModel vm)
            vm.SubphylumViewModel.PropertyChanged -= SubphylumViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
