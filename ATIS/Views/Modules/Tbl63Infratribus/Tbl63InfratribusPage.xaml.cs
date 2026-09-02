using System.ComponentModel;
using ATIS.Views.Modules.Tbl66Genus;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    Tbl63InfratribusPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPage : Page
{
    public Tbl63InfratribusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl63InfratribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl63InfratribusViewModel>();
        DataContext = ViewModel;

        Loaded += InfratribusPage_Loaded;
    }

    private Tbl63InfratribusViewModel Vm => (Tbl63InfratribusViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void InfratribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfratribusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.GenusViewModel.PropertyChanged += GenusViewModel_PropertyChanged;

        UpdateGenusTabs(Vm.GenusViewModel.SelectedItem != null);
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
                Vm.InfratribusCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.InfratribusReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.GenusViewModel.GenusCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.GenusViewModel.GenusReferenceViewModel.PrepareViewState();
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
    // Genus-UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateGenusTabs(bool hasSelection)
    {
        CommentsGenusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesGenusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsGenusTab.IsEnabled = hasSelection;
        ReferencesGenusTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailPivotIndex >= 4)
            Vm.SelectedDetailPivotIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditInfratribusTab, EditInfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 0);
        SetTabButtonStyle(CommentsInfratribusTab, CommentsInfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 1);
        SetTabButtonStyle(ReferencesInfratribusTab, ReferencesInfratribusTabUnderline, Vm.SelectedDetailPivotIndex == 2);
        SetTabButtonStyle(GenusTab, GenusTabUnderline, Vm.SelectedDetailPivotIndex == 3);
        SetTabButtonStyle(CommentsGenusTab, CommentsGenusTabUnderline, Vm.SelectedDetailPivotIndex == 4);
        SetTabButtonStyle(ReferencesGenusTab, ReferencesGenusTabUnderline, Vm.SelectedDetailPivotIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditInfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsInfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesInfratribusContent.Visibility = Vm.SelectedDetailPivotIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        GenusContent.Visibility = Vm.SelectedDetailPivotIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsGenusContent.Visibility = Vm.SelectedDetailPivotIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesGenusContent.Visibility = Vm.SelectedDetailPivotIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }
    
    // ---------------------------------------------------------
    // PROPERTY CHANGED
    // ---------------------------------------------------------
    private void GenusViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl66GenusViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateGenusTabs(Vm.GenusViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl63InfratribusViewModel vm)
            vm.GenusViewModel.PropertyChanged -= GenusViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
