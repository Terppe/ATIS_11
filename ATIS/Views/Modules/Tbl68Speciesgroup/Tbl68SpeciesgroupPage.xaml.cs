using System.ComponentModel;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl75Species;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl68Speciesgroup;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl68SpeciesgroupPage : Page
{

    public Tbl68SpeciesgroupViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl68SpeciesgroupPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl68SpeciesgroupViewModel>();
        DataContext = ViewModel;

        Loaded += SpeciesgroupPage_Loaded;
    }

    private Tbl68SpeciesgroupViewModel Vm => (Tbl68SpeciesgroupViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void SpeciesgroupPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SpeciesgroupPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        //Vm.SpeciesViewModel.PropertyChanged += SpeciesViewModel_PropertyChanged;

        //UpdateSpeciesTabs(Vm.SpeciesViewModel.SelectedItem != null);
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
            //case 1:
            //    Vm.GenusCommentViewModel.PrepareViewState();
            //    break;

            //case 2:
            //    Vm.GenusReferenceViewModel.PrepareViewState();
            //    break;

            //case 4:
            //    Vm.SpeciesViewModel.SpeciesCommentViewModel.PrepareViewState();
            //    break;

            //case 5:
            //    Vm.SpeciesViewModel.SpeciesReferenceViewModel.PrepareViewState();
            //    break;
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
    // Species  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateSpeciesTabs(bool hasSelection)
    {
        //CommentsSpeciesTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesSpeciesTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        //CommentsSpeciesTab.IsEnabled = hasSelection;
        //ReferencesSpeciesTab.IsEnabled = hasSelection;

        //if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
        //    Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditSpeciesgroupTab, EditSpeciesgroupTabUnderline, Vm.SelectedDetailTabIndex == 0);
        //SetTabButtonStyle(CommentsGenusTab, CommentsGenusTabUnderline, Vm.SelectedDetailTabIndex == 1);
        //SetTabButtonStyle(ReferencesGenusTab, ReferencesGenusTabUnderline, Vm.SelectedDetailTabIndex == 2);
        //SetTabButtonStyle(SpeciesTab, SpeciesTabUnderline, Vm.SelectedDetailTabIndex == 3);
        //SetTabButtonStyle(CommentsSpeciesTab, CommentsSpeciesTabUnderline, Vm.SelectedDetailTabIndex == 4);
        //SetTabButtonStyle(ReferencesSpeciesTab, ReferencesSpeciesTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditSpeciesgroupContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        //CommentsGenusContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesGenusContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        //SpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        //CommentsSpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesSpeciesContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
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
