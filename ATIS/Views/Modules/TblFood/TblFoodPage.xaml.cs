using System.ComponentModel;
using ATIS.Views.Modules.Tbl81Image;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Modules.TblFood;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TblFoodPage : Page
{
    public TblFoodViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblFoodPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<TblFoodViewModel>();
        DataContext = ViewModel;

        Loaded += TblFoodPage_Loaded;
    }

    private TblFoodViewModel Vm => (TblFoodViewModel)DataContext;

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void TblFoodPage_Loaded(object sender, RoutedEventArgs e)
    {

        Loaded -= TblFoodPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.ImageViewModel.PropertyChanged += ImageViewModel_PropertyChanged;

        UpdateImageTabs(Vm.ImageViewModel.SelectedItem != null);
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
                Vm.ImageViewModel.PrepareViewState();
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
    // Image  -UNTERTABS SICHTBARKEIT UND STATUS VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------
    private void UpdateImageTabs(bool hasSelection)
    {
        //CommentsSubtribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        //ReferencesSubtribusTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        //CommentsSubtribusTab.IsEnabled = hasSelection;
        //ReferencesSubtribusTab.IsEnabled = hasSelection;

        //if (!hasSelection && Vm.SelectedDetailTabIndex >= 4)
        //    Vm.SelectedDetailTabIndex = 3;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(EditFoodTab, EditFoodTabUnderline, Vm.SelectedDetailTabIndex == 0);
        SetTabButtonStyle(ImageTab, ImageTabUnderline, Vm.SelectedDetailTabIndex == 1);
    }

    private void UpdateDetailContentVisibility()
    {
        EditFoodContent.Visibility = Vm.SelectedDetailTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        ImageContent.Visibility = Vm.SelectedDetailTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------                  
    // PROPERTY CHANGED
    // ---------------------------------------------------------                  
    private void ImageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl81ImageViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateImageTabs(Vm.ImageViewModel.SelectedItem != null);
            });
        }
    }

    // ---------------------------------------------------------         
    // NAVIGATED FROM
    // ---------------------------------------------------------
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is TblFoodViewModel vm)
            vm.ImageViewModel.PropertyChanged -= ImageViewModel_PropertyChanged;

        base.OnNavigatedFrom(e);
    }

}
