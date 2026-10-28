using System.Configuration;
using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Auth;
using ATIS.Views.Modules.TblUserProfile;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Admin;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AdminAreaPage : Page
{

    public TblUserProfileViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public AdminAreaPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<TblUserProfileViewModel>();
        DataContext = ViewModel;

        Loaded += UserProfilePage_Loaded;
    }

    private TblUserProfileViewModel Vm => (TblUserProfileViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------
    private async void UserProfilePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= UserProfilePage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

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
            //    Vm.UserProfileCommentViewModel.PrepareViewState();
            //    break;

            //case 2:
            //    Vm.UserProfileReferenceViewModel.PrepareViewState();
            //    break;

            //case 4:
            //    Vm.NULLViewModel.NULLCommentViewModel.PrepareViewState();
            //    break;

            //case 5:
            //    Vm.NULLViewModel.NULLReferenceViewModel.PrepareViewState();
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
    private void UpdateNULLTabs(bool hasSelection)
    {

        if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
            Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditUserProfileTab, EditUserProfileTabUnderline, Vm.SelectedDetailTabIndex == 0);
    }

    private void UpdateDetailContentVisibility()
    {
        EditUserProfileContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
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
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        //if (DataContext is TblUserProfileViewModel vm)
        //    vm.NULLViewModel.PropertyChanged -= NULLViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }



}
