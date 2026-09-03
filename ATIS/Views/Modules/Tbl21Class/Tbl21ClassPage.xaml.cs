using System.ComponentModel;
using ATIS.Views.Modules.Tbl24Subclass;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl21ClassPage Skriptdatum:  26.08.2026   18:32    

namespace ATIS.Views.Modules.Tbl21Class;

public sealed partial class Tbl21ClassPage : Page
{

    public Tbl21ClassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl21ClassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl21ClassViewModel>();
        DataContext = ViewModel;

        Loaded += ClassPage_Loaded;
    }

    private Tbl21ClassViewModel Vm => (Tbl21ClassViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void ClassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ClassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SubclassViewModel.PropertyChanged += SubclassViewModel_PropertyChanged;

        UpdateSubclassTabs(Vm.SubclassViewModel.SelectedItem != null);
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
                Vm.ClassCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.ClassReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SubclassViewModel.SubclassCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SubclassViewModel.SubclassReferenceViewModel.PrepareViewState();
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
    // Subclass  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSubclassTabs(bool hasSelection)
    {
        CommentsSubclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSubclassTab.IsEnabled = hasSelection;
        ReferencesSubclassTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditClassTab, EditClassTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsClassTab, CommentsClassTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesClassTab, ReferencesClassTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SubclassTab, SubclassTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSubclassTab, CommentsSubclassTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSubclassTab, ReferencesSubclassTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditClassContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsClassContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesClassContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SubclassContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubclassContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubclassContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SubclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl24SubclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubclassTabs(Vm.SubclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl21ClassViewModel vm)
            vm.SubclassViewModel.PropertyChanged -= SubclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
