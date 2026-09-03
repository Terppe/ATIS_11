using System.ComponentModel;
using ATIS.Views.Modules.Tbl21Class;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl18SuperclassPage Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl18Superclass;

public sealed partial class Tbl18SuperclassPage : Page
{

    public Tbl18SuperclassViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl18SuperclassPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl18SuperclassViewModel>();
        DataContext = ViewModel;

        Loaded += SuperclassPage_Loaded;
    }

    private Tbl18SuperclassViewModel Vm => (Tbl18SuperclassViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SuperclassPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SuperclassPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.ClassViewModel.PropertyChanged += ClassViewModel_PropertyChanged;

        UpdateClassTabs(Vm.ClassViewModel.SelectedItem != null);
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
                Vm.SuperclassCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SuperclassReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.ClassViewModel.ClassCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.ClassViewModel.ClassReferenceViewModel.PrepareViewState();
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
    // Class  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateClassTabs(bool hasSelection)
    {
        CommentsClassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesClassTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsClassTab.IsEnabled = hasSelection;
        ReferencesClassTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSuperclassTab, EditSuperclassTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(CommentsSuperclassTab, CommentsSuperclassTabUnderline, Vm.SelectedDetailTabIndex == 1);
        SetTabButtonStyle(ReferencesSuperclassTab, ReferencesSuperclassTabUnderline, Vm.SelectedDetailTabIndex == 2);
        SetTabButtonStyle(ClassTab, ClassTabUnderline, Vm.SelectedDetailTabIndex == 3);
        SetTabButtonStyle(CommentsClassTab, CommentsClassTabUnderline, Vm.SelectedDetailTabIndex == 4);
        SetTabButtonStyle(ReferencesClassTab, ReferencesClassTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSuperclassContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        ClassContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsClassContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesClassContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void ClassViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl21ClassViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateClassTabs(Vm.ClassViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl18SuperclassViewModel vm)
            vm.ClassViewModel.PropertyChanged -= ClassViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
