
using System.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    TblCountryPage Skriptdatum:   01.10.2026 12:32      

namespace ATIS.Views.Modules.TblCountry;

public sealed partial class TblCountryPage : Page
{

    public TblCountryViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public TblCountryPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<TblCountryViewModel>();
        DataContext = ViewModel;

        Loaded += CountryPage_Loaded;
    }

    private TblCountryViewModel Vm => (TblCountryViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void CountryPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= CountryPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        //Vm.NULLViewModel.PropertyChanged += NULLViewModel_PropertyChanged;

        //UpdateNULLTabs(Vm.NULLViewModel.SelectedItem != null);
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
            //    Vm.CountryCommentViewModel.PrepareViewState();
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
    // NULL  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    //private void UpdateNULLTabs(bool hasSelection)
    //{
    //    CommentsNULLTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
    //    ReferencesNULLTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

    //    CommentsNULLTab.IsEnabled = hasSelection;
    //    ReferencesNULLTab.IsEnabled = hasSelection;

    //    if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
    //        Vm.SelectedDetailTabIndex = 3;

    //    UpdateTabButtonStates();
    //    UpdateDetailContentVisibility();
    //}

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditCountryTab, EditCountryTabUnderline, Vm.SelectedDetailTabIndex == 0);
        //SetTabButtonStyle(CommentsCountryTab, CommentsCountryTabUnderline, Vm.SelectedDetailTabIndex == 1);
        //SetTabButtonStyle(ReferencesCountryTab, ReferencesCountryTabUnderline, Vm.SelectedDetailTabIndex == 2);
        //SetTabButtonStyle(NULLTab, NULLTabUnderline, Vm.SelectedDetailTabIndex == 3);
        //SetTabButtonStyle(CommentsNULLTab, CommentsNULLTabUnderline, Vm.SelectedDetailTabIndex == 4);
        //SetTabButtonStyle(ReferencesNULLTab, ReferencesNULLTabUnderline, Vm.SelectedDetailTabIndex == 5);
    }

    private void UpdateDetailContentVisibility()
    {
        EditCountryContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        //CommentsCountryContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesCountryContent.Visibility = Vm.SelectedDetailTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        //NULLContent.Visibility = Vm.SelectedDetailTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        //CommentsNULLContent.Visibility = Vm.SelectedDetailTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesNULLContent.Visibility = Vm.SelectedDetailTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    //private void NULLViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(NULLViewModel.SelectedItem))
    //    {
    //        DispatcherQueue.TryEnqueue(() =>
    //            UpdateNULLTabs(Vm.NULLViewModel.SelectedItem != null));
    //    }
    //}

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    //protected override void OnNavigatedFrom(NavigationEventArgs e)
    //{
    //    if (DataContext is TblCountryViewModel vm)
    //        vm.NULLViewModel.PropertyChanged -= NULLViewModel_PropertyChanged;

    //    base.OnNavigatedFrom(e);
    //}



}

