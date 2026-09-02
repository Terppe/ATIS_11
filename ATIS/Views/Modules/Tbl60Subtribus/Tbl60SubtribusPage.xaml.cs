using System.ComponentModel;
using ATIS.Views.Modules.Tbl63Infratribus;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl60SubtribusPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusPage : Page
{

    public Tbl60SubtribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl60SubtribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl60SubtribusViewModel>();
        DataContext = ViewModel;

        Loaded += SubtribusPage_Loaded;
    }

    private Tbl60SubtribusViewModel Vm => (Tbl60SubtribusViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubtribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.InfratribusViewModel.PropertyChanged += InfratribusViewModel_PropertyChanged;

        UpdateInfratribusTabs(Vm.InfratribusViewModel.SelectedItem != null);
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
        switch (Vm.SelectedDetailPivotIndex)
        {
            case 1:
                Vm.SubtribusCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.SubtribusReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.InfratribusViewModel.InfratribusCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.InfratribusViewModel.InfratribusReferenceViewModel.PrepareViewState();
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
            Vm.SelectedDetailPivotIndex = index;

            PrepareCurrentTabState();
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // Infratribus  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateInfratribusTabs(bool hasSelection)
    {
        CommentsInfratribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfratribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsInfratribusTab.IsEnabled = hasSelection;
        ReferencesInfratribusTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailPivotIndex >= 4)
            Vm.SelectedDetailPivotIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSubtribusTab, EditSubtribusTabUnderline, Vm.SelectedDetailPivotIndex == 0);
        SetTabButtonStyle(CommentsSubtribusTab, CommentsSubtribusTabUnderline, Vm.SelectedDetailPivotIndex == 1);
        SetTabButtonStyle(ReferencesSubtribusTab, ReferencesSubtribusTabUnderline, Vm.SelectedDetailPivotIndex == 2);
        SetTabButtonStyle(InfratribusTab, InfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 3);
        SetTabButtonStyle(CommentsInfratribusTab, CommentsInfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 4);
        SetTabButtonStyle(ReferencesInfratribusTab, ReferencesInfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSubtribusContent.Visibility = Vm.SelectedDetailPivotIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSubtribusContent.Visibility = Vm.SelectedDetailPivotIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSubtribusContent.Visibility = Vm.SelectedDetailPivotIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        InfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void InfratribusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl63InfratribusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateInfratribusTabs(Vm.InfratribusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl60SubtribusViewModel vm)
            vm.InfratribusViewModel.PropertyChanged -= InfratribusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
