
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  FoodPage.xaml.cs Skriptdatum:  09.09.2025  10:32         

namespace ATIS.Views.Main;

public sealed partial class FoodPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public FoodViewModel ViewModel { get; }
    private TblFood SingleFood { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;
    private readonly FileOpenPicker _filePicker = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public FoodPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<FoodViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Food Methods]

    private async void GetFoodsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblFoodsList?.Clear();

            ViewModel.FoodItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblFoodsList?.Clear();

                FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
                FoodDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingFood = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblFoodsList ??= [];
                ViewModel.TblFoodsList = int.TryParse(_search, out var id)
                ? [.. Context.TblFoods
                .Where(a => a.FoodId == id)]
                : [.. Context.TblFoods
                .Where(a => _search != null && a.FoodName.StartsWith(_search))
                .OrderBy(a => a.FoodName)];
            });
            FoodDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.FoodDataSetCount = ViewModel.TblFoodsList.Count;

            if (ViewModel.TblFoodsList is { Count: 0 })
            {
                ViewModel.TblFoodsList?.Clear();

                AppBarButtonDeleteFood.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelFood.Visibility = Visibility.Visible;
                FoodDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(FoodPage));
            }
            else
            {
                FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
                ViewModel.FoodSelected = ViewModel.TblFoodsList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void FoodDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await FoodDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task FoodDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.FoodSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        ViewModel.IsLoading = false;
    }

    private async void AddFood_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddFood();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddFood()
    {
        FoodName.Loaded += (s, e) =>
        {
            FoodName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditFood = true;
        ViewModel.IsNewFood = true;

        await ViewModel.AddFoodAsync();

        if (ViewModel.TblFoodsList != null)
        {
            ViewModel.FoodSelected = ViewModel.TblFoodsList[0];
        }

        FoodDetailExpander.IsExpanded = true;
        FoodDetailExpander.Visibility = Visibility.Visible;

        FoodName.PlaceholderText = StringsRes.EnterText;
    }

    private void FoodSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        FoodSearchBox.Visibility = Visibility.Visible;
        FoodSearchBox.Focus(FocusState.Programmatic);
        FoodSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void FoodDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblFoodsList.Sort);
    }

    private void AddNewFoodCanceled(object sender, EventArgs e) => FoodDetailExpander.IsExpanded = false;

    private async void ViewFoodHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewFoodHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewFoodHyperlink()
    {
        ViewModel.IsInEditFood = false;
        ViewModel.IsNewFood = false;
        FoodDetailExpander.IsExpanded = true;
        FoodDetailExpander.Visibility = Visibility.Visible;

        if (FoodDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleFood = App.SingleFood(ViewModel.FoodSelected.FoodId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsFoodAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewFood)
        {
            FoodDetailExpander.IsExpanded = false;
            FoodDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelFoodEdits();
        }
    }

    private void CancelFoodEdits()
    {
        var x = (DataGrid)FindName("FoodDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.FoodSelected = ViewModel.TblFoodsList[iNdx];
            SingleFood = App.SingleFood(ViewModel.FoodSelected.FoodId);
            FoodSingles();
        }
        else
        {
            FoodDetailExpander.IsExpanded = false;
            FoodDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditFood = false;
        ViewModel.IsNewFood = false;
    }

    private void FoodSingles()
    {
        ViewModel.FoodSelected.FoodName = SingleFood.FoodName;
        ViewModel.FoodSelected.Valid = SingleFood.Valid;
        ViewModel.FoodSelected.ValidYear = SingleFood.ValidYear;
        ViewModel.FoodSelected.FoodVolume = SingleFood.FoodVolume;
        ViewModel.FoodSelected.Karnivore = SingleFood.Karnivore;
        ViewModel.FoodSelected.Herbivore = SingleFood.Herbivore;
        ViewModel.FoodSelected.Limnivore = SingleFood.Limnivore;
        ViewModel.FoodSelected.Omnivore = SingleFood.Omnivore;
        ViewModel.FoodSelected.RegionTop = SingleFood.RegionTop;
        ViewModel.FoodSelected.RegionMiddle = SingleFood.RegionMiddle;
        ViewModel.FoodSelected.RegionBottom = SingleFood.RegionBottom;
        ViewModel.FoodSelected.Kind = SingleFood.Kind;
        ViewModel.FoodSelected.Help = SingleFood.Help;
        ViewModel.FoodSelected.Info = SingleFood.Info;
        ViewModel.FoodSelected.Memo = SingleFood.Memo;
        ViewModel.FoodSelected.ImageMimeType = SingleFood.ImageMimeType;
        ViewModel.FoodSelected.Filestream = SingleFood.Filestream;
        ViewModel.FoodSelected.FilestreamId = SingleFood.FilestreamId;
        ViewModel.FoodSelected.ImageData = SingleFood.ImageData;
    }
    private async Task RevertChangesFoodAsync()
    {
        ViewModel.IsInEditFood = false;
        if (ViewModel.IsModifiedFood)
        {
            await RefreshFoodAsync();
            ViewModel.IsModifiedFood = false;
        }
    }
    private Task RefreshFoodAsync()
    {
        if (FoodDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleFood = App.SingleFood(ViewModel.FoodSelected.FoodId);
            FoodSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteFood_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteFood();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteFood()
    {
        var deleteDialog = new DeleteDialog
        {
            PrimaryButtonText = StringsRes.Delete,
            SecondaryButtonText = StringsRes.DoNotDelete,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await ViewModel.DeleteFoodAsync(ViewModel.FoodSelected);
                await Refresh();
                FoodDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesFoodAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveFood_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveFood();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveFood()
    {
        var saveDialog = new SaveChangesDialog
        {
            PrimaryButtonText = StringsRes.Save,
            SecondaryButtonText = StringsRes.DoNotSave,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Save,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                if (await BeforeSaveFood())
                {
                    return;
                }
                await ViewModel.SaveFoodAsync(ViewModel.FoodSelected, ViewModel.SelectedPath);
                await BehindSaveFood(ViewModel.FoodSelected, ViewModel.SelectedPath);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedFood = true;
                await RevertChangesFoodAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveFood()
    {
        _search = FoodSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblFoodsList ??= [];
            ViewModel.TblFoodsList = [.. Context.TblFoods
                .Where(a => a.FoodName == ViewModel.FoodSelected.FoodName)];
        });
        if (ViewModel.IsNewFood)
        {
            if (ViewModel.TblFoodsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedFood = true;
        return false;
    }

    private async Task BehindSaveFood(TblFood viewModelFoodSelected, string viewModelSelectedPath)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblFoodsList ??= [];
            if (FoodSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.TblFoodsList = int.TryParse(_search, out var id)
                    ? [.. Context.TblFoods
                    .Where(a => a.FoodId == id)]
                    : [.. Context.TblFoods
                    .Where(a => _search != null && a.FoodName.StartsWith(_search))
                    .OrderBy(a => a.FoodName)];
            }
        });

        ViewModel.RefreshFoodItems();

        if (ViewModel.TblFoodsList.Count == 0)
        {
            ViewModel.TblFoodsList = [.. Context.TblFoods
                .Where(a => a.FoodId == viewModelFoodSelected.FoodId)];
            FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
            await Refresh();
            FoodDataGrid.SelectedIndex = 0;
        }
        else
        {
            FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
            var index = ViewModel.FoodItems.ToList().FindIndex(item => item.FoodId == viewModelFoodSelected.FoodId);
            FoodDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.FoodDataSetCount = ViewModel.TblFoodsList.Count;
    }

    private void FoodName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackFood_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Food Methods]                    

    private void BackDetailExpanders()
    {
        FoodDetailExpander.IsExpanded = false;
        FoodDetailExpander.Visibility = Visibility.Collapsed;
    }

    #region [Refresh Methods]
    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Refresh();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task Refresh()
    {
        ViewModel.IsLoading = true;

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (string.IsNullOrEmpty(_search))
        {
            //  DetailPanelFood.Visibility = Visibility.Visible;
            //  FoodDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("FoodDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.TblFoodsList!.Count)
            {
                ViewModel.FoodItems.Clear();
                foreach (var item in ViewModel.TblFoodsList)
                {
                    ViewModel.FoodItems.Add(item);
                }

                ViewModel.FoodSelected = ViewModel.TblFoodsList[iNdx];
            }
            FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
            FoodDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.TblFoodsList.Count == 0)
                {
                    ViewModel.TblFoodsList ??= [];
                    ViewModel.TblFoodsList = [.. Context.TblFoods
                            .Where(a => a.FoodId == ViewModel.FoodSelected.FoodId)];
                }
                else
                {
                    ViewModel.TblFoodsList ??= [];
                    ViewModel.TblFoodsList = int.TryParse(_search, out var id)
                        ? [.. Context.TblFoods
                           .Where(a => a.FoodId == id)]
                       : [.. Context.TblFoods
                           .Where(a => _search != null && a.FoodName.StartsWith(_search))
                         .OrderBy(a => a.FoodName)];
                }
            });

            if (ViewModel.TblFoodsList is { Count: 0 })
            {
                ViewModel.TblFoodsList?.Clear();
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;

            if (ViewModel.IsDeletingFood)
            {
                if (iNdx == 0 && ViewModel.TblFoodsList.Count == 0)
                {
                    ViewModel.TblFoodsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.FoodSelected = ViewModel.TblFoodsList[iNdx];
            }
        }
        FoodDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingFood = false;
    }


    private async void ChangeImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(_filePicker, hwnd);
            _filePicker.FileTypeFilter.Add("*");
            _filePicker.ViewMode = PickerViewMode.Thumbnail;
            //_filePicker.FileTypeFilter.Add("*");
            _filePicker.FileTypeFilter.Add(".jpg");
            _filePicker.FileTypeFilter.Add(".jpeg");
            _filePicker.FileTypeFilter.Add(".png");
            _filePicker.FileTypeFilter.Add(".gif");
            _filePicker.FileTypeFilter.Add(".jpe");
            _filePicker.FileTypeFilter.Add(".bmp");
            _filePicker.FileTypeFilter.Add(".ico");
            _filePicker.FileTypeFilter.Add(".tif");
            _filePicker.FileTypeFilter.Add(".tiff");
            _filePicker.FileTypeFilter.Add(".hpd");
            _filePicker.FileTypeFilter.Add(".jxr");
            _filePicker.FileTypeFilter.Add(".wdp");
            _filePicker.FileTypeFilter.Add(".hpd");
            var file = await _filePicker.PickSingleFileAsync();
            if (file != null && file.Path != null)
            {
                SelectedPathImage.Text = file.Path;
                Image.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void AddImage_Click(object sender, RoutedEventArgs e)
    {
        FoodName.Loaded += (s, e) =>
        {
            FoodName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditFood = true;
        ViewModel.IsNewFood = true;

        await ViewModel.AddFoodAsync();

        if (ViewModel.TblFoodsList != null)
        {
            ViewModel.FoodSelected = ViewModel.TblFoodsList[0];
        }

        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;


        //New Image search
        OpenFileDialog();
        ViewModel.RefreshFoodItems();
        FoodName.PlaceholderText = StringsRes.EnterText;
    }

    private async void OpenFileDialog()
    {
        try
        {
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(_filePicker, hwnd);
            _filePicker.FileTypeFilter.Add("*");
            _filePicker.ViewMode = PickerViewMode.Thumbnail;
            //_filePicker.FileTypeFilter.Add("*");
            _filePicker.FileTypeFilter.Add(".jpg");
            _filePicker.FileTypeFilter.Add(".jpeg");
            _filePicker.FileTypeFilter.Add(".png");
            _filePicker.FileTypeFilter.Add(".gif");
            _filePicker.FileTypeFilter.Add(".jpe");
            _filePicker.FileTypeFilter.Add(".bmp");
            _filePicker.FileTypeFilter.Add(".ico");
            _filePicker.FileTypeFilter.Add(".tif");
            _filePicker.FileTypeFilter.Add(".tiff");
            _filePicker.FileTypeFilter.Add(".hpd");
            _filePicker.FileTypeFilter.Add(".jxr");
            _filePicker.FileTypeFilter.Add(".wdp");
            _filePicker.FileTypeFilter.Add(".hpd");
            var file = await _filePicker.PickSingleFileAsync();
            if (file != null && file.Path != null)
            {
                SelectedPathImage.Text = file.Path;
                Image.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }
    #endregion [Refresh Methods]

}

