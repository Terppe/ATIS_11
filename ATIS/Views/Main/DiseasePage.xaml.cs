
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

//  DiseasePage.xaml.cs Skriptdatum:  09.09.2025  10:32      

namespace ATIS.Views.Main;

public sealed partial class DiseasePage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public DiseaseViewModel ViewModel { get; }
    private TblDisease SingleDisease { get; set; }
    private static readonly AtisDbContext Context = new();
    private Tbl81Image SingleImage { get; set; }
    private string _search;
    private readonly FileOpenPicker _filePicker = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public DiseasePage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<DiseaseViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]         

    #region [Disease Methods]

    private async void GetDiseasesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblDiseasesList?.Clear();

            ViewModel.DiseaseItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblDiseasesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();

                DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
                DiseaseDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingDisease = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblDiseasesList ??= [];
                ViewModel.TblDiseasesList = int.TryParse(_search, out var id)
                ? [.. Context.TblDiseases
                .Where(a => a.DiseaseId == id)]
                : [.. Context.TblDiseases
                .Where(a => _search != null && a.DiseaseName.StartsWith(_search))
                .OrderBy(a => a.DiseaseName)];
            });
            DiseaseDetailExpander.Visibility = Visibility.Collapsed;
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.DiseaseDataSetCount = ViewModel.TblDiseasesList.Count;

            if (ViewModel.TblDiseasesList is { Count: 0 })
            {
                ViewModel.TblDiseasesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();

                AppBarButtonDeleteDisease.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;
                AppBarButtonAddImage.IsEnabled = false;

                DetailPanelDisease.Visibility = Visibility.Visible;
                DiseaseDetailExpander.Visibility = Visibility.Collapsed;
                ImagesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(DiseasePage));
            }
            else
            {
                DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
                ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void DiseaseDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await DiseaseDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DiseaseDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.DiseaseSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        if (ViewModel.DiseaseSelected != null)
        {
            //--------------------------Image-----------------------------          
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl81ImagesList ??= [];
                ViewModel.Tbl81ImagesList =
                [
                    .. Context.Tbl81Images
                        .Where(com => com.DiseaseId == ViewModel.DiseaseSelected.DiseaseId)
                ];
            });

            ViewModel.ImageDataSetCount = ViewModel.Tbl81ImagesList.Count;
            ViewModel.RefreshImageItems();

            if (ViewModel.Tbl81ImagesList.Count == 0)
            {
                ImagesExpander.Visibility = Visibility.Collapsed;
                ImageDetailExpander.Visibility = Visibility.Collapsed;
                ImageDataGrid.ItemsSource = null;
            }
            else
            {
                ImagesExpander.Visibility = Visibility.Visible;
                ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddDisease_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddDisease();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddDisease()
    {
        DiseaseName.Loaded += (s, e) =>
        {
            DiseaseName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditDisease = true;
        ViewModel.IsNewDisease = true;

        await ViewModel.AddDiseaseAsync();

        if (ViewModel.TblDiseasesList != null)
        {
            ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[0];
        }

        DiseaseDetailExpander.IsExpanded = true;
        DiseaseDetailExpander.Visibility = Visibility.Visible;
        SpecificationExpander.IsExpanded = false;

        DiseaseName.PlaceholderText = StringsRes.EnterText;
    }

    private void DiseaseSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        DiseaseSearchBox.Visibility = Visibility.Visible;
        DiseaseSearchBox.Focus(FocusState.Programmatic);
        DiseaseSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void DiseaseDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblDiseasesList.Sort);
    }

    private void AddNewDiseaseCanceled(object sender, EventArgs e) => DiseaseDetailExpander.IsExpanded = false;

    private async void ViewDiseaseHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewDiseaseHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewDiseaseHyperlink()
    {
        ViewModel.IsInEditDisease = false;
        ViewModel.IsNewDisease = false;
        DiseaseDetailExpander.IsExpanded = true;
        DiseaseDetailExpander.Visibility = Visibility.Visible;

        if (DiseaseDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleDisease = App.SingleDisease(ViewModel.DiseaseSelected.DiseaseId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsDiseaseAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewDisease)
        {
            DiseaseDetailExpander.IsExpanded = false;
            DiseaseDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelDiseaseEdits();
        }
    }

    private void CancelDiseaseEdits()
    {
        var x = (DataGrid)FindName("DiseaseDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[iNdx];
            SingleDisease = App.SingleDisease(ViewModel.DiseaseSelected.DiseaseId);
            DiseaseSingles();
        }
        else
        {
            DiseaseDetailExpander.IsExpanded = false;
            DiseaseDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditDisease = false;
        ViewModel.IsNewDisease = false;
    }

    private void DiseaseSingles()
    {
        ViewModel.DiseaseSelected.DiseaseName = SingleDisease.DiseaseName;
        ViewModel.DiseaseSelected.Valid = SingleDisease.Valid;
        ViewModel.DiseaseSelected.ValidYear = SingleDisease.ValidYear;
        ViewModel.DiseaseSelected.ShortInfo = SingleDisease.ShortInfo;
        ViewModel.DiseaseSelected.Symptom = SingleDisease.Symptom;
        ViewModel.DiseaseSelected.Reason = SingleDisease.Reason;
        ViewModel.DiseaseSelected.Therapie = SingleDisease.Therapie;
        ViewModel.DiseaseSelected.Info = SingleDisease.Info;
        ViewModel.DiseaseSelected.Memo = SingleDisease.Memo;
    }
    private async Task RevertChangesDiseaseAsync()
    {
        ViewModel.IsInEditDisease = false;
        if (ViewModel.IsModifiedDisease)
        {
            await RefreshDiseaseAsync();
            ViewModel.IsModifiedDisease = false;
        }
    }
    private Task RefreshDiseaseAsync()
    {
        if (DiseaseDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleDisease = App.SingleDisease(ViewModel.DiseaseSelected.DiseaseId);
            DiseaseSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteDisease_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteDisease();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteDisease()
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
                await ViewModel.DeleteDiseaseAsync(ViewModel.DiseaseSelected);
                await Refresh();
                DiseaseDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesDiseaseAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveDisease_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveDisease();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveDisease()
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
                if (await BeforeSaveDisease())
                {
                    return;
                }
                await ViewModel.SaveDiseaseAsync(ViewModel.DiseaseSelected);
                await BehindSaveDisease(ViewModel.DiseaseSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedDisease = true;
                await RevertChangesDiseaseAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveDisease()
    {
        _search = DiseaseSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblDiseasesList ??= [];
            ViewModel.TblDiseasesList = [.. Context.TblDiseases
                .Where(a => a.DiseaseName == ViewModel.DiseaseSelected.DiseaseName)];
        });
        if (ViewModel.IsNewDisease)
        {
            if (ViewModel.TblDiseasesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedDisease = true;
        return false;
    }

    private async Task BehindSaveDisease(TblDisease viewModelDiseaseSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblDiseasesList ??= [];
            if (DiseaseSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.TblDiseasesList = int.TryParse(_search, out var id)
                    ? [.. Context.TblDiseases
                    .Where(a => a.DiseaseId == id)]
                    : [.. Context.TblDiseases
                    .Where(a => _search != null && a.DiseaseName.StartsWith(_search))
                    .OrderBy(a => a.DiseaseName)];
            }
        });

        ViewModel.RefreshDiseaseItems();

        if (ViewModel.TblDiseasesList.Count == 0)
        {
            ViewModel.TblDiseasesList = [.. Context.TblDiseases
                .Where(a => a.DiseaseId == viewModelDiseaseSelected.DiseaseId)];
            DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
            await Refresh();
            DiseaseDataGrid.SelectedIndex = 0;
        }
        else
        {
            DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
            var index = ViewModel.DiseaseItems.ToList().FindIndex(item => item.DiseaseId == viewModelDiseaseSelected.DiseaseId);
            DiseaseDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.DiseaseDataSetCount = ViewModel.TblDiseasesList.Count;
    }

    private void DiseaseName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackDisease_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Disease Methods]                    

    #region [Image Methods]

    private async void AddImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddImage()
    {
        InfoImage.Loaded += (s, e) =>
        {
            InfoImage.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditImage = true;
        ViewModel.IsNewImage = true;

        await ViewModel.AddImageAsync();

        if (ViewModel.Tbl81ImagesList != null)
        {
            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];
        }

        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;

        //FoodDetailExpander.IsExpanded = false;
        //ImagesExpander.IsExpanded = false;

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
            SingleImage = App.SingleImageDisease(ViewModel.ImageSelected.ImageId, ViewModel.DiseaseSelected.DiseaseId);
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

            SingleImage = App.SingleImageDisease(ViewModel.ImageSelected.ImageId, ViewModel.DiseaseSelected.DiseaseId);
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
        ViewModel.ImageSelected.FiSpeciesId = SingleImage.FiSpeciesId;
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
                SingleImage = App.SingleImageDisease(ViewModel.ImageSelected.ImageId, ViewModel.DiseaseSelected.DiseaseId);
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
                        a.PlSpeciesId == ViewModel.ImageSelected.PlSpeciesId &&
                        a.FiSpeciesId == ViewModel.ImageSelected.FiSpeciesId)];

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
                .Where(com => com.FiSpeciesId == viewModelImageSelected.FiSpeciesId)];
        });
        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .Where(com => com.FiSpeciesId == viewModelImageSelected.FiSpeciesId)];
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
        DiseaseDetailExpander.IsExpanded = false;
        DiseaseDetailExpander.Visibility = Visibility.Collapsed;
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
            //  DetailPanelDisease.Visibility = Visibility.Visible;
            //   DiseaseDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("DiseaseDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.TblDiseasesList!.Count)
            {
                ViewModel.DiseaseItems.Clear();
                foreach (var item in ViewModel.TblDiseasesList)
                {
                    ViewModel.DiseaseItems.Add(item);
                }

                ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[iNdx];
            }
            DiseaseDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.TblDiseasesList.Count == 0)
                {
                    ViewModel.TblDiseasesList ??= [];
                    ViewModel.TblDiseasesList = [.. Context.TblDiseases
                            .Where(a => a.DiseaseId == ViewModel.DiseaseSelected.DiseaseId)];
                }
                else
                {
                    ViewModel.TblDiseasesList ??= [];
                    ViewModel.TblDiseasesList = int.TryParse(_search, out var id)
                        ? [.. Context.TblDiseases
                           .Where(a => a.DiseaseId == id)]
                       : [.. Context.TblDiseases
                           .Where(a => _search != null && a.DiseaseName.StartsWith(_search))
                         .OrderBy(a => a.DiseaseName)];
                }
            });

            DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;

            if (ViewModel.IsDeletingDisease)
            {
                if (iNdx == 0 && ViewModel.TblDiseasesList.Count == 0)
                {
                    ViewModel.TblDiseasesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[iNdx];
            }
        }
        DiseaseDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingDisease = false;
    }

    #endregion [Refresh Methods] 


}

