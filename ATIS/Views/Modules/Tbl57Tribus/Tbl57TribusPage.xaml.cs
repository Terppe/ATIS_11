using System.ComponentModel;
using ATIS.Views.Modules.Tbl60Subtribus;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl57TribusPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl57Tribus;

public sealed partial class Tbl57TribusPage : Page
{

    public Tbl57TribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl57TribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl57TribusViewModel>();
        DataContext = ViewModel;

        Loaded += TribusPage_Loaded;
    }

    private Tbl57TribusViewModel Vm => (Tbl57TribusViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void TribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= TribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SubtribusViewModel.PropertyChanged += SubtribusViewModel_PropertyChanged;

        UpdateSubtribusTabs(Vm.SubtribusViewModel.SelectedItem != null);
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
                Vm.TribusCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.TribusReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SubtribusViewModel.SubtribusCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SubtribusViewModel.SubtribusReferenceViewModel.PrepareViewState();
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
    // Subtribus  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSubtribusTabs(bool hasSelection)
    {
        CommentsSubtribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubtribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSubtribusTab.IsEnabled = hasSelection;
        ReferencesSubtribusTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditTribusTab, EditTribusTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsTribusTab, CommentsTribusTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesTribusTab, ReferencesTribusTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SubtribusTab, SubtribusTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSubtribusTab, CommentsSubtribusTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSubtribusTab, ReferencesSubtribusTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditTribusContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsTribusContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesTribusContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SubtribusContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubtribusContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubtribusContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SubtribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl60SubtribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSubtribusTabs(Vm.SubtribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl57TribusViewModel vm)
            vm.SubtribusViewModel.PropertyChanged -= SubtribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
