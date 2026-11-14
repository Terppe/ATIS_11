using ATIS.Views.Modules.TblUserProfile;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Admin;

public sealed partial class AdminAreaPage : Page
{

    public TblUserProfileViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public AdminAreaPage()
    {
        ViewModel = App.GetService<TblUserProfileViewModel>();
        InitializeComponent();
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

            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }


    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditUserProfileTab, EditUserProfileTabUnderline, Vm.SelectedDetailTabIndex == 0);
    }

    private void UpdateDetailContentVisibility()
    {
        EditUserProfileContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

}
