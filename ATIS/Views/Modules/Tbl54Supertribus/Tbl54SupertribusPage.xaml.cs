using System.ComponentModel;
using ATIS.Views.Modules.Tbl57Tribus;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl54SupertribusPage Skriptdatum:  18.18.2026  10:32    

namespace ATIS.Views.Modules.Tbl54Supertribus;

public sealed partial class Tbl54SupertribusPage : Page
{

    public Tbl54SupertribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl54SupertribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl54SupertribusViewModel>();
        DataContext = ViewModel;

        Loaded += SupertribusPage_Loaded;
    }

    private Tbl54SupertribusViewModel Vm => (Tbl54SupertribusViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SupertribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SupertribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.TribusViewModel.PropertyChanged += TribusViewModel_PropertyChanged;

        UpdateTribusTabs(Vm.TribusViewModel.SelectedItem != null);
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
                Vm.SupertribusCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SupertribusReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.TribusViewModel.TribusCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.TribusViewModel.TribusReferenceViewModel.PrepareViewState();
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
    // Tribus  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateTribusTabs(bool hasSelection)
    {
        CommentsTribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesTribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsTribusTab.IsEnabled = hasSelection;
        ReferencesTribusTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSupertribusTab, EditSupertribusTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSupertribusTab, CommentsSupertribusTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSupertribusTab, ReferencesSupertribusTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(TribusTab, TribusTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsTribusTab, CommentsTribusTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesTribusTab, ReferencesTribusTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSupertribusContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        TribusContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsTribusContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesTribusContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void TribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl57TribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateTribusTabs(Vm.TribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl54SupertribusViewModel vm)
            vm.TribusViewModel.PropertyChanged -= TribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
