using System.ComponentModel;
using ATIS.Views.Modules.Tbl18Superclass;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl12SubphylumPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl12Subphylum;

public sealed partial class Tbl12SubphylumPage : Page
{

    public Tbl12SubphylumViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl12SubphylumPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl12SubphylumViewModel>();
        DataContext = ViewModel;

        Loaded += SubphylumPage_Loaded;
    }

    private Tbl12SubphylumViewModel Vm => (Tbl12SubphylumViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SubphylumPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubphylumPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SuperclassViewModel.PropertyChanged += SuperclassViewModel_PropertyChanged;

        UpdateSuperclassTabs(Vm.SuperclassViewModel.SelectedItem != null);
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
                Vm.SubphylumCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SubphylumReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SuperclassViewModel.SuperclassCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SuperclassViewModel.SuperclassReferenceViewModel.PrepareViewState();
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
    // Superclass  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSuperclassTabs(bool hasSelection)
    {
        CommentsSuperclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperclassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSuperclassTab.IsEnabled = hasSelection;
        ReferencesSuperclassTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSubphylumTab, EditSubphylumTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSubphylumTab, CommentsSubphylumTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSubphylumTab, ReferencesSubphylumTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(SuperclassTab, SuperclassTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsSuperclassTab, CommentsSuperclassTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesSuperclassTab, ReferencesSuperclassTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubphylumContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SuperclassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl18SuperclassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSuperclassTabs(Vm.SuperclassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl12SubphylumViewModel vm)
            vm.SuperclassViewModel.PropertyChanged -= SuperclassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
