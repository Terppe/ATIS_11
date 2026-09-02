using System.ComponentModel;
using ATIS.Views.Modules.Tbl75Species;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl66GenusPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusPage : Page
{

    public Tbl66GenusViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl66GenusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl66GenusViewModel>();
        DataContext = ViewModel;

        Loaded += GenusPage_Loaded;
    }

    private Tbl66GenusViewModel Vm => (Tbl66GenusViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void GenusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= GenusPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.SpeciesViewModel.PropertyChanged += SpeciesViewModel_PropertyChanged;

        UpdateSpeciesTabs(Vm.SpeciesViewModel.SelectedItem != null);
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
                Vm.GenusCommentViewModel.PrepareViewState();
                break;

            case 2:
                Vm.GenusReferenceViewModel.PrepareViewState();
                break;

            case 4:
                Vm.SpeciesViewModel.SpeciesCommentViewModel.PrepareViewState();
                break;

            case 5:
                Vm.SpeciesViewModel.SpeciesReferenceViewModel.PrepareViewState();
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
    // Species  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------

    private void UpdateSpeciesTabs(bool hasSelection)
    {
        CommentsSpeciesTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSpeciesTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        CommentsSpeciesTab.IsEnabled = hasSelection;
        ReferencesSpeciesTab.IsEnabled = hasSelection;

        if (!hasSelection && Vm.SelectedDetailPivotIndex >= 4)
            Vm.SelectedDetailPivotIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditGenusTab, EditGenusTabUnderline, Vm.SelectedDetailPivotIndex == 0);
        SetTabButtonStyle(CommentsGenusTab, CommentsGenusTabUnderline, Vm.SelectedDetailPivotIndex == 1);
        SetTabButtonStyle(ReferencesGenusTab, ReferencesGenusTabUnderline, Vm.SelectedDetailPivotIndex == 2);
        SetTabButtonStyle(SpeciesTab, SpeciesTabUnderline, Vm.SelectedDetailPivotIndex == 3);
        SetTabButtonStyle(CommentsSpeciesTab, CommentsSpeciesTabUnderline, Vm.SelectedDetailPivotIndex == 4);
        SetTabButtonStyle(ReferencesSpeciesTab, ReferencesSpeciesTabUnderline, Vm.SelectedDetailPivotIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditGenusContent.Visibility = Vm.SelectedDetailPivotIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        CommentsGenusContent.Visibility = Vm.SelectedDetailPivotIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesGenusContent.Visibility = Vm.SelectedDetailPivotIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SpeciesContent.Visibility = Vm.SelectedDetailPivotIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        CommentsSpeciesContent.Visibility = Vm.SelectedDetailPivotIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReferencesSpeciesContent.Visibility = Vm.SelectedDetailPivotIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void SpeciesViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl75SpeciesViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateSpeciesTabs(Vm.SpeciesViewModel.SelectedItem != null));
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl66GenusViewModel vm)
            vm.SpeciesViewModel.PropertyChanged -= SpeciesViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }
}
