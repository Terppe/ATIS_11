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

//  TblMiscellaneousPage.xaml.cs Skriptdatum:  09.09.2025  10:32      

namespace ATIS.Views.Database;

public sealed partial class TblMiscellaneousListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public TblMiscellaneousViewModel ViewModel { get; }
    private TblMiscellaneous SingleMiscellaneous { get; set; }
    private static readonly AtisDbContext Context = new();
    private Tbl81Image SingleImage { get; set; }
    private string _search;
    private readonly FileOpenPicker _filePicker = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public TblMiscellaneousListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<TblMiscellaneousViewModel>();
            InitializeComponent();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]         

    #region [Miscellaneous Methods]

    private async void GetMiscellaneoussByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblMiscellaneoussesList?.Clear();

            ViewModel.MiscellaneousItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblMiscellaneoussesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();

                MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
                MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingMiscellaneous = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblMiscellaneoussesList ??= [];
                ViewModel.TblMiscellaneoussesList = int.TryParse(_search, out var id)
                ? [.. Context.TblMiscellaneousses
                .Where(a => a.MiscellaneousId == id)]
                : [.. Context.TblMiscellaneousses
                .Where(a => _search != null && a.MiscellaneousName.StartsWith(_search))
                .OrderBy(a => a.MiscellaneousName)];
            });
            MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.MiscellaneousDataSetCount = ViewModel.TblMiscellaneoussesList.Count;

            if (ViewModel.TblMiscellaneoussesList is { Count: 0 })
            {
                ViewModel.TblMiscellaneoussesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();

                AppBarButtonDeleteMiscellaneous.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;
                AppBarButtonAddImage.IsEnabled = false;

                DetailPanelMiscellaneous.Visibility = Visibility.Visible;
                MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;
                ImagesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(TblMiscellaneousListPage));
            }
            else
            {
                MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
                ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[0];
            }
            ButtonsVisible();
            ButtonsIconVisible();
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void MiscellaneousDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await MiscellaneousDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task MiscellaneousDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.MiscellaneousSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        if (ViewModel.MiscellaneousSelected != null)
        {
            //--------------------------Image-----------------------------          
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl81ImagesList ??= [];
                ViewModel.Tbl81ImagesList =
                [
                    .. Context.Tbl81Images
                        .Where(com => com.MiscellaneousId == ViewModel.MiscellaneousSelected.MiscellaneousId)
                ];
            });

            ViewModel.ImageDataSetCount = ViewModel.Tbl81ImagesList.Count;

            if (ViewModel.Tbl81ImagesList.Count == 0)
            {
                ImagesExpander.Visibility = Visibility.Collapsed;
                ImageDetailExpander.Visibility = Visibility.Collapsed;
                ImageDataGrid.ItemsSource = null;
            }
            else
            {
                ImagesExpander.Visibility = Visibility.Visible;
                ImageDataGrid.ItemsSource = null;
                ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddMiscellaneous_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddMiscellaneous();
            MiscellaneousName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(MiscellaneousName);
            };
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddMiscellaneous()
    {
        ViewModel.IsInEditMiscellaneous = true;
        ViewModel.IsNewMiscellaneous = true;

        await ViewModel.AddMiscellaneousAsync();

        if (ViewModel.TblMiscellaneoussesList != null)
        {
            ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[0];
        }

        MiscellaneousDetailExpander.IsExpanded = true;
        MiscellaneousDetailExpander.Visibility = Visibility.Visible;

        MiscellaneousName.PlaceholderText = StringsRes.EnterText;
    }

    private void MiscellaneousSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        MiscellaneousSearchBox.Visibility = Visibility.Visible;
        MiscellaneousSearchBox.Focus(FocusState.Programmatic);
        MiscellaneousSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void MiscellaneousDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblMiscellaneoussesList.Sort);
    }

    private async void ViewMiscellaneousHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewMiscellaneousHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewMiscellaneousHyperlink()
    {
        ViewModel.IsInEditMiscellaneous = false;
        ViewModel.IsNewMiscellaneous = false;
        MiscellaneousDetailExpander.IsExpanded = true;
        MiscellaneousDetailExpander.Visibility = Visibility.Visible;

        if (MiscellaneousDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleMiscellaneous = App.SingleMiscellaneous(ViewModel.MiscellaneousSelected.MiscellaneousId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsMiscellaneousAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewMiscellaneous)
        {
            MiscellaneousDetailExpander.IsExpanded = false;
            MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelMiscellaneousEdits();
        }
    }

    private void CancelMiscellaneousEdits()
    {
        var x = (DataGrid)FindName("MiscellaneousDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[iNdx];
            SingleMiscellaneous = App.SingleMiscellaneous(ViewModel.MiscellaneousSelected.MiscellaneousId);
            MiscellaneousSingles();
        }
        else
        {
            MiscellaneousDetailExpander.IsExpanded = false;
            MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditMiscellaneous = false;
        ViewModel.IsNewMiscellaneous = false;
    }

    private void MiscellaneousSingles()
    {
        ViewModel.MiscellaneousSelected.MiscellaneousName = SingleMiscellaneous.MiscellaneousName;
        ViewModel.MiscellaneousSelected.Valid = SingleMiscellaneous.Valid;
        ViewModel.MiscellaneousSelected.ValidYear = SingleMiscellaneous.ValidYear;
        ViewModel.MiscellaneousSelected.Thema1 = SingleMiscellaneous.Thema1;
        ViewModel.MiscellaneousSelected.Thema2 = SingleMiscellaneous.Thema2;
        ViewModel.MiscellaneousSelected.Info = SingleMiscellaneous.Info;
        ViewModel.MiscellaneousSelected.Memo = SingleMiscellaneous.Memo;
    }

    private async Task RevertChangesMiscellaneousAsync()
    {
        ViewModel.IsInEditMiscellaneous = false;
        if (ViewModel.IsModifiedMiscellaneous)
        {
            await RefreshMiscellaneousAsync();
            ViewModel.IsModifiedMiscellaneous = false;
        }
    }

    private Task RefreshMiscellaneousAsync()
    {
        if (MiscellaneousDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.MiscellaneousSelected.MiscellaneousId == 0)
            {
                return Task.CompletedTask;
            }
            SingleMiscellaneous = App.SingleMiscellaneous(ViewModel.MiscellaneousSelected.MiscellaneousId);
            MiscellaneousSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteMiscellaneous_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteMiscellaneous();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteMiscellaneous()
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
                await ViewModel.DeleteMiscellaneousAsync(ViewModel.MiscellaneousSelected);
                await Refresh();
                MiscellaneousDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesMiscellaneousAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveMiscellaneous_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveMiscellaneous();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveMiscellaneous()
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
                if (await BeforeSaveMiscellaneous())
                {
                    return;
                }
                await ViewModel.SaveMiscellaneousAsync(ViewModel.MiscellaneousSelected);
                await BehindSaveMiscellaneous(ViewModel.MiscellaneousSelected);
                await Refresh();
                ViewModel.IsNewMiscellaneous = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedMiscellaneous = true;
                await RevertChangesMiscellaneousAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveMiscellaneous()
    {
        _search = MiscellaneousSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblMiscellaneoussesList ??= [];
            ViewModel.TblMiscellaneoussesList = [.. Context.TblMiscellaneousses
                .Where(a => a.MiscellaneousName == ViewModel.MiscellaneousSelected.MiscellaneousName)];
        });
        if (ViewModel.IsNewMiscellaneous)
        {
            if (ViewModel.TblMiscellaneoussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedMiscellaneous = true;
        return false;
    }

    private async Task BehindSaveMiscellaneous(TblMiscellaneous viewModelMiscellaneousSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblMiscellaneoussesList ??= [];
            if (MiscellaneousSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.TblMiscellaneoussesList = int.TryParse(_search, out var id)
                    ? [.. Context.TblMiscellaneousses
                    .Where(a => a.MiscellaneousId == id)]
                    : [.. Context.TblMiscellaneousses
                    .Where(a => _search != null && a.MiscellaneousName.StartsWith(_search))
                    .OrderBy(a => a.MiscellaneousName)];
            }
        });

        ViewModel.RefreshMiscellaneousItems();

        if (ViewModel.TblMiscellaneoussesList.Count == 0)
        {
            ViewModel.TblMiscellaneoussesList = [.. Context.TblMiscellaneousses
                .Where(a => a.MiscellaneousId == viewModelMiscellaneousSelected.MiscellaneousId)];
            MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
            await Refresh();
            MiscellaneousDataGrid.SelectedIndex = 0;
        }
        else
        {
            MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
            var index = ViewModel.MiscellaneousItems.ToList().FindIndex(item => item.MiscellaneousId == viewModelMiscellaneousSelected.MiscellaneousId);
            MiscellaneousDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.MiscellaneousDataSetCount = ViewModel.TblMiscellaneoussesList.Count;
    }

    private void MiscellaneousName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackMiscellaneous_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Miscellaneous Methods]                    

    #region [Image Methods]

    private async void AddImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddImage();
            InfoImage.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfoImage);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddImage()
    {
        ViewModel.IsInEditImage = true;
        ViewModel.IsNewImage = true;

        await ViewModel.AddImageAsync();

        if (ViewModel.Tbl81ImagesList != null)
        {
            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];
        }

        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;


        //New Image search
        OpenFileDialog();
        ViewModel.RefreshImageItems();

        InfoImage.PlaceholderText = StringsRes.EnterText;
    }

    private void DataGrid_Image_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl81ImagesList.Sort);
    }

    private void AddNewImageCanceled(object sender, EventArgs e) => ImagesExpander.IsExpanded = false;

    private async void ViewImageHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewImageHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }
    private Task ViewImageHyperlink()
    {
        ViewModel.IsInEditImage = false;
        ViewModel.IsNewImage = false;
        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;
        if (ImageDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleImage = App.SingleImageMiscellaneous(ViewModel.ImageSelected.ImageId, ViewModel.MiscellaneousSelected.MiscellaneousId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsImageAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewImage)
        {
            ImageDetailExpander.IsExpanded = false;
            ImageDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelImageEdits();
        }
    }
    private void CancelImageEdits()
    {
        var x = (DataGrid)FindName("ImageDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];

            SingleImage = App.SingleImageMiscellaneous(ViewModel.ImageSelected.ImageId, ViewModel.MiscellaneousSelected.MiscellaneousId);
            ImageSingles();
        }
        else
        {
            ImageDetailExpander.IsExpanded = false;
            ImageDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditImage = false;
        ViewModel.IsNewImage = false;
    }
    private void ImageSingles()
    {
        ViewModel.ImageSelected.MiscellaneousId = SingleImage.MiscellaneousId;
        ViewModel.ImageSelected.Valid = SingleImage.Valid;
        ViewModel.ImageSelected.ValidYear = SingleImage.ValidYear;
        ViewModel.ImageSelected.Filestream = SingleImage.Filestream;
        ViewModel.ImageSelected.FilestreamId = SingleImage.FilestreamId;
        ViewModel.ImageSelected.ImageData = SingleImage.ImageData;
        ViewModel.ImageSelected.ImageMimeType = SingleImage.ImageMimeType;
        ViewModel.ImageSelected.Info = SingleImage.Info;
        ViewModel.ImageSelected.Memo = SingleImage.Memo;
        ViewModel.ImageSelected.ShotDate = SingleImage.ShotDate;
    }
    private async Task RevertChangesImageAsync()
    {
        ViewModel.IsInEditImage = false;
        if (ViewModel.IsModifiedImage)
        {
            await RefreshImageAsync();
            ViewModel.IsModifiedImage = false;
        }
    }
    private async Task RefreshImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (ImageDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleImage = App.SingleImageMiscellaneous(ViewModel.ImageSelected.ImageId, ViewModel.MiscellaneousSelected.MiscellaneousId);
                ImageSingles();
            }
        });
    }

    private async void DeleteImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteImage()
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
                await ViewModel.DeleteImageAsync(ViewModel.ImageSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesImageAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveImage()
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
                if (await BeforeSaveImage())
                {
                    return;
                }
                await ViewModel.SaveImageAsync(ViewModel.ImageSelected, ViewModel.SelectedPath);
                await BehindSaveImage(ViewModel.ImageSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedImage = true;
                await RevertChangesImageAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private async Task<bool> BeforeSaveImage()
    {
        ViewModel.Tbl81ImagesList ??= [];
        ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
            .Where(a => a.Info == ViewModel.ImageSelected.Info &&
                        a.MiscellaneousId == ViewModel.ImageSelected.MiscellaneousId &&
                        a.ImageMimeType == ViewModel.ImageSelected.ImageMimeType &&
                        a.Valid == ViewModel.ImageSelected.Valid &&
                        a.ShotDate == ViewModel.ImageSelected.ShotDate)];

        if (ViewModel.IsNewImage)
        {
            if (ViewModel.Tbl81ImagesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedImage = true;
        return false;
    }
    private async Task BehindSaveImage(Tbl81Image viewModelImageSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl81ImagesList ??= [];
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .Where(com => com.MiscellaneousId == viewModelImageSelected.MiscellaneousId)];
        });
        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .Where(com => com.MiscellaneousId == viewModelImageSelected.MiscellaneousId)];
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            await Refresh();
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDataGrid.ItemsSource = null;
        }
        else
        {
            ImagesExpander.Visibility = Visibility.Visible;
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            ImageDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.ImageDataSetCount = ViewModel.Tbl81ImagesList.Count;
    }

    private static byte[] LoadImageData(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new BinaryReader(fs);
        var imageBytes = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return imageBytes;
    }

    private void BackImage_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
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

    #endregion [Image Methods]         

    #region [Refresh Methods]

    private void BackDetailExpanders()
    {
        MiscellaneousDetailExpander.IsExpanded = false;
        MiscellaneousDetailExpander.Visibility = Visibility.Collapsed;
        ImageDetailExpander.IsExpanded = false;
        ImageDetailExpander.Visibility = Visibility.Collapsed;
    }

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
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("MiscellaneousDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.TblMiscellaneoussesList!.Count)
            {
                ViewModel.MiscellaneousItems.Clear();
                foreach (var item in ViewModel.TblMiscellaneoussesList)
                {
                    ViewModel.MiscellaneousItems.Add(item);
                }

                ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[iNdx];
            }
            MiscellaneousDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.TblMiscellaneoussesList.Count == 0)
                {
                    ViewModel.TblMiscellaneoussesList ??= [];
                    ViewModel.TblMiscellaneoussesList = [.. Context.TblMiscellaneousses
                            .Where(a => a.MiscellaneousId == ViewModel.MiscellaneousSelected.MiscellaneousId)];
                }
                else
                {
                    ViewModel.TblMiscellaneoussesList ??= [];
                    ViewModel.TblMiscellaneoussesList = int.TryParse(_search, out var id)
                        ? [.. Context.TblMiscellaneousses
                           .Where(a => a.MiscellaneousId == id)]
                       : [.. Context.TblMiscellaneousses
                           .Where(a => _search != null && a.MiscellaneousName.StartsWith(_search))
                         .OrderBy(a => a.MiscellaneousName)];
                }
            });

            MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;

            if (ViewModel.IsDeletingMiscellaneous)
            {
                if (iNdx == 0 && ViewModel.TblMiscellaneoussesList.Count == 0)
                {
                    ViewModel.TblMiscellaneoussesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[iNdx];
            }
        }
        await MiscellaneousDataGridCurrentCellChanged();

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingMiscellaneous = false;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteMiscellaneous.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteMiscellaneous.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteMiscellaneous.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteMiscellaneous.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 

}

