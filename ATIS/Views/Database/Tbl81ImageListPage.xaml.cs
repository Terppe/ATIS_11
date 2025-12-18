
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

//  Tbl81ImageListPage.xaml.cs Skriptdatum:  25.11.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl81ImageListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl81ImageViewModel ViewModel { get; }
    private Tbl81Image SingleImage { get; set; }
    private Tbl75Species SingleSpecies { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;
    private readonly FileOpenPicker _filePicker = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl81ImageListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl81ImageViewModel>();
            InitializeComponent();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }

        //string command = "/C notepad.exe";
        //Process.Start("cmd.exe", command);
    }

    #endregion [Constructor] 

    #region [Image Methods]

    private async void GetImagesByInfoOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl81ImagesList?.Clear();

            ViewModel.ImageItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl81ImagesList?.Clear();

                ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
                ViewModel.Tbl75SpeciessesList?.Clear();

                SpeciessesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingImage = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                //ViewModel.Tbl81ImagesList ??= [];
                //ViewModel.Tbl81ImagesList = int.TryParse(_search, out var id)        //only Id
                //    ? [.. Context.Tbl81Images
                //        .Where(e => e.ImageId == id)]
                //    : [null];

                ViewModel.Tbl81ImagesList ??= [];
                ViewModel.Tbl81ImagesList = int.TryParse(_search, out var id)         //Info and Id
                ? [.. Context.Tbl81Images
                .Where(e => e.ImageId == id)]
                : [.. Context.Tbl81Images
                .Where(e => e.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];
            });

            ImageDetailExpander.Visibility = Visibility.Collapsed;
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.ImageDataSetCount = ViewModel.Tbl81ImagesList.Count;

            if (ViewModel.Tbl81ImagesList is { Count: 0 })
            {
                ViewModel.Tbl81ImagesList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();

                AppBarButtonDeleteImage.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelImage.Visibility = Visibility.Visible;
                ImageDetailExpander.Visibility = Visibility.Collapsed;
                SpeciessesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl81ImageListPage));
            }
            else
            {
                ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
                ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];
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

    private async void ImageDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await ImageDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task ImageDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.ImageSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.ImageSelected != null)
        {
            //--------------------------Species-----------------------------          
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
            .Where(com => com.SpeciesId == ViewModel.ImageSelected.SpeciesId)];
            });
            ViewModel.SpeciesDataSetCount = ViewModel.Tbl75SpeciessesList.Count;

            ViewModel.RefreshSpeciesItems();
            ViewModel.RefreshImageItems();

            if (ViewModel.Tbl75SpeciessesList.Count == 0)
            {
                SpeciessesExpander.Visibility = Visibility.Collapsed;
                SpeciesDataGrid.ItemsSource = null;
            }
            else
            {
                SpeciessesExpander.Visibility = Visibility.Visible;
                SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddImage();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
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
        OpenFileDialog(null);
        ViewModel.RefreshImageItems();

        InfoImage.PlaceholderText = StringsRes.EnterText;
    }

    private void ImageSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        ImageSearchBox.Visibility = Visibility.Visible;
        ImageSearchBox.Focus(FocusState.Programmatic);
        ImageSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void ImageDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl81ImagesList.Sort);
    }

    private void AddNewImageCanceled(object sender, EventArgs e) => ImageDetailExpander.IsExpanded = false;

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
            SingleImage = App.SingleImage(ViewModel.ImageSelected.ImageId);
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
            SingleImage = App.SingleImage(ViewModel.ImageSelected.ImageId);
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
        ViewModel.ImageSelected.SpeciesId = SingleImage.SpeciesId;
        ViewModel.ImageSelected.Valid = SingleImage.Valid;
        ViewModel.ImageSelected.ValidYear = SingleImage.ValidYear;
        ViewModel.ImageSelected.Info = SingleImage.Info;
        ViewModel.ImageSelected.Memo = SingleImage.Memo;
        ViewModel.ImageSelected.ImageMimeType = SingleImage.ImageMimeType;
        ViewModel.ImageSelected.Filestream = SingleImage.Filestream;
        ViewModel.ImageSelected.FilestreamId = SingleImage.FilestreamId;
        ViewModel.ImageSelected.ImageData = SingleImage.ImageData;
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
    private Task RefreshImageAsync()
    {
        if (ImageDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.ImageSelected.ImageId == 0)
            {
                return Task.CompletedTask;
            }
            SingleImage = App.SingleImage(ViewModel.ImageSelected.ImageId);
            ImageSingles();
        }

        return Task.CompletedTask;
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
                ImageDataGrid.SelectedIndex = 0;
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
                await BehindSaveImage(ViewModel.ImageSelected, ViewModel.SelectedPath);
                await Refresh();
                ViewModel.IsNewImage = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedImage = true;
                await RevertChangesImageAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveImage()
    {
        _search = ImageSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl81ImagesList ??= [];
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .Where(a => a.Info == ViewModel.ImageSelected.Info &&
                            a.SpeciesId == ViewModel.ImageSelected.SpeciesId)];
        });
            if (ViewModel.Tbl81ImagesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        ViewModel.IsModifiedImage = true;
        return false;
    }

    private async Task BehindSaveImage(Tbl81Image viewModelImageSelected, string viewModelSelectedPath)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl81ImagesList ??= [];
            if (ImageSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl81ImagesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl81Images
                    .Where(a => a.ImageId == id)]
                    : [.. Context.Tbl81Images
                    .Where(a => _search != null && a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            }
        });

        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .Where(a => a.ImageId == viewModelImageSelected.ImageId)];
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            await Refresh();
            ImageDataGrid.SelectedIndex = 0;
        }
        else
        {
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
            var index = ViewModel.ImageItems.ToList().FindIndex(item => item.ImageId == viewModelImageSelected.ImageId);
            ImageDataGrid.SelectedIndex = index;
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
                Image1.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private void BackImage_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    private async void OpenFileDialog(string s)
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
                Image1.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    #endregion [Image Methods]        

    #region [Species Methods]

    private async void ViewSpeciesHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSpeciesHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }

    }

    private Task ViewSpeciesHyperlink()
    {
        ViewModel.IsInEditSpecies = false;
        ViewModel.IsNewSpecies = false;
        SpeciesDetailExpander.IsExpanded = true;
        SpeciesDetailExpander.Visibility = Visibility.Visible;
        if (SpeciesDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSpeciesAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSpecies)
        {
            SpeciesDetailExpander.IsExpanded = false;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSpeciesEdits();
        }
    }

    private void CancelSpeciesEdits()
    {
        var x = (DataGrid)FindName("SpeciesDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[iNdx];

            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
            SpeciesSingles();
        }
        else
        {
            SpeciesDetailExpander.IsExpanded = false;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSpecies = false;
        ViewModel.IsNewSpecies = false;
    }

    private void SpeciesSingles()
    {
        ViewModel.SpeciesSelected.SpeciesName = SingleSpecies.SpeciesName;
        ViewModel.SpeciesSelected.Subspecies = SingleSpecies.Subspecies;
        ViewModel.SpeciesSelected.Divers = SingleSpecies.Divers;
        ViewModel.SpeciesSelected.GenusId = SingleSpecies.GenusId;
        ViewModel.SpeciesSelected.SpeciesgroupId = SingleSpecies.SpeciesgroupId;
        ViewModel.SpeciesSelected.Valid = SingleSpecies.Valid;
        ViewModel.SpeciesSelected.ValidYear = SingleSpecies.ValidYear;
        ViewModel.SpeciesSelected.MemoSpecies = SingleSpecies.MemoSpecies;
        ViewModel.SpeciesSelected.TradeName = SingleSpecies.TradeName;
        ViewModel.SpeciesSelected.Author = SingleSpecies.Author;
        ViewModel.SpeciesSelected.AuthorYear = SingleSpecies.AuthorYear;
        ViewModel.SpeciesSelected.Importer = SingleSpecies.Importer;
        ViewModel.SpeciesSelected.ImportingYear = SingleSpecies.ImportingYear;
        ViewModel.SpeciesSelected.TypeSpecies = SingleSpecies.TypeSpecies;
        ViewModel.SpeciesSelected.LNumber = SingleSpecies.LNumber;
        ViewModel.SpeciesSelected.LOrigin = SingleSpecies.LOrigin;
        ViewModel.SpeciesSelected.LdaOrigin = SingleSpecies.LdaOrigin;
        ViewModel.SpeciesSelected.LdaNumber = SingleSpecies.LdaNumber;
        ViewModel.SpeciesSelected.BasinLength = SingleSpecies.BasinLength;
        ViewModel.SpeciesSelected.FishLength = SingleSpecies.FishLength;
        ViewModel.SpeciesSelected.Karnivore = SingleSpecies.Karnivore;
        ViewModel.SpeciesSelected.Herbivore = SingleSpecies.Herbivore;
        ViewModel.SpeciesSelected.Limnivore = SingleSpecies.Limnivore;
        ViewModel.SpeciesSelected.Omnivore = SingleSpecies.Omnivore;
        ViewModel.SpeciesSelected.MemoFoods = SingleSpecies.MemoFoods;
        ViewModel.SpeciesSelected.Difficult1 = SingleSpecies.Difficult1;
        ViewModel.SpeciesSelected.Difficult2 = SingleSpecies.Difficult2;
        ViewModel.SpeciesSelected.Difficult3 = SingleSpecies.Difficult3;
        ViewModel.SpeciesSelected.Difficult4 = SingleSpecies.Difficult4;
        ViewModel.SpeciesSelected.RegionTop = SingleSpecies.RegionTop;
        ViewModel.SpeciesSelected.RegionMiddle = SingleSpecies.RegionMiddle;
        ViewModel.SpeciesSelected.RegionBottom = SingleSpecies.RegionBottom;
        ViewModel.SpeciesSelected.MemoRegion = SingleSpecies.MemoRegion;
        ViewModel.SpeciesSelected.MemoTech = SingleSpecies.MemoTech;
        ViewModel.SpeciesSelected.Ph1 = SingleSpecies.Ph1;
        ViewModel.SpeciesSelected.Ph2 = SingleSpecies.Ph2;
        ViewModel.SpeciesSelected.Temp1 = SingleSpecies.Temp1;
        ViewModel.SpeciesSelected.Temp2 = SingleSpecies.Temp2;
        ViewModel.SpeciesSelected.Hardness1 = SingleSpecies.Hardness1;
        ViewModel.SpeciesSelected.Hardness2 = SingleSpecies.Hardness2;
        ViewModel.SpeciesSelected.CarboHardness1 = SingleSpecies.CarboHardness1;
        ViewModel.SpeciesSelected.CarboHardness2 = SingleSpecies.CarboHardness2;
        ViewModel.SpeciesSelected.MemoHusbandry = SingleSpecies.MemoHusbandry;
        ViewModel.SpeciesSelected.MemoBuilt = SingleSpecies.MemoBuilt;
        ViewModel.SpeciesSelected.MemoColor = SingleSpecies.MemoColor;
        ViewModel.SpeciesSelected.MemoSozial = SingleSpecies.MemoSozial;
        ViewModel.SpeciesSelected.MemoDomorphism = SingleSpecies.MemoDomorphism;
        ViewModel.SpeciesSelected.MemoBreeding = SingleSpecies.MemoBreeding;
        ViewModel.SpeciesSelected.MemoSpecial = SingleSpecies.MemoSpecial;
    }

    private async Task RevertChangesSpeciesAsync()
    {
        ViewModel.IsInEditSpecies = false;
        if (ViewModel.IsModifiedSpecies)
        {
            await RefreshSpeciesAsync();
            ViewModel.IsModifiedSpecies = false;
        }
    }

    private async Task RefreshSpeciesAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SpeciesDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
                SpeciesSingles();
            }
        });
    }
    private async void SaveSpecies_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSpecies();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSpecies()
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
                if (await BeforeSaveSpecies())
                {
                    return;
                }
                await ViewModel.SaveSpeciesAsync(ViewModel.SpeciesSelected);
                await BehindSaveSpecies(ViewModel.SpeciesSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSpecies = true;
                await RevertChangesSpeciesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSpecies()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(a => a.SpeciesName == ViewModel.SpeciesSelected.SpeciesName &&
                        a.Subspecies == ViewModel.SpeciesSelected.Subspecies &&
                        a.Divers == ViewModel.SpeciesSelected.Divers &&
                        a.SpeciesgroupId == ViewModel.SpeciesSelected.SpeciesgroupId &&
                        a.Creature == ViewModel.SpeciesSelected.Creature &&
                        a.GenusId == ViewModel.SpeciesSelected.GenusId)];
        });
        if (ViewModel.IsNewSpecies)
        {
            if (ViewModel.Tbl75SpeciessesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSpecies = true;
        return false;
    }

    private async Task BehindSaveSpecies(Tbl75Species viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(com => com.SpeciesId == viewModelSpeciesSelected.SpeciesId)];
        });
        ViewModel.RefreshSpeciesItems();

        if (ViewModel.Tbl75SpeciessesList.Count == 0)
        {
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(com => com.SpeciesId == viewModelSpeciesSelected.SpeciesId)];
            SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            await Refresh();
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDataGrid.ItemsSource = null;
        }
        else
        {
            SpeciessesExpander.Visibility = Visibility.Visible;
            SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            SpeciesDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.SpeciesDataSetCount = ViewModel.Tbl75SpeciessesList.Count;
    }

    private void BackSpecies_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    private void ComboCreature_OnSelectionChangedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = sender as ComboBox;
        if (cmb == null)
        {
            return;
        }

        var text = ((ComboBox)sender).SelectedItem as string;
        switch (text)
        {
            case "":    //neu
                FoodExpanderFiSpecies.Visibility = Visibility.Visible;
                BreedingExpanderFiSpecies.Visibility = Visibility.Visible;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Visible;
                SpecialExpanderFiSpecies.Visibility = Visibility.Visible;
                ReproductionExpanderPlSpecies.Visibility = Visibility.Visible;
                CultureExpanderPlSpecies.Visibility = Visibility.Visible;
                GlobalExpanderPlSpecies.Visibility = Visibility.Visible;
                break;
            case "Animalia":
                FoodExpanderFiSpecies.Visibility = Visibility.Visible;
                BreedingExpanderFiSpecies.Visibility = Visibility.Visible;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Visible;
                SpecialExpanderFiSpecies.Visibility = Visibility.Visible;
                ReproductionExpanderPlSpecies.Visibility = Visibility.Collapsed;
                CultureExpanderPlSpecies.Visibility = Visibility.Collapsed;
                GlobalExpanderPlSpecies.Visibility = Visibility.Collapsed;
                break;
            case "Plantae":
                ReproductionExpanderPlSpecies.Visibility = Visibility.Visible;
                CultureExpanderPlSpecies.Visibility = Visibility.Visible;
                GlobalExpanderPlSpecies.Visibility = Visibility.Visible;
                FoodExpanderFiSpecies.Visibility = Visibility.Collapsed;
                BreedingExpanderFiSpecies.Visibility = Visibility.Collapsed;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Collapsed;
                SpecialExpanderFiSpecies.Visibility = Visibility.Collapsed;
                break;
            case "Fungi":
                break;
            case "Archaea":
                break;
            case "Bacteria":
                break;
            case "Chromista":
                break;
            case "Protozoa":
                break;
        }

        //var cmb = sender as ComboBox;
        //if (cmb != null)
        //{
        //    handle = !cmb.IsDropDownOpen;
        //}

        //Handle();
    }
    #endregion [Species Methods]  

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
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("ImageDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl81ImagesList!.Count)
            {
                ViewModel.ImageItems.Clear();
                foreach (var item in ViewModel.Tbl81ImagesList)
                {
                    ViewModel.ImageItems.Add(item);
                }

                ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];
            }
            ImageDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl81ImagesList ??= [];
                ViewModel.Tbl81ImagesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl81Images
                .Where(a => a.ImageId == id)]
                : [.. Context.Tbl81Images
                .Where(a => a.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];
            });
            if (ViewModel.Tbl81ImagesList is { Count: 0 })
            {
                ViewModel.Tbl81ImagesList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;

            if (ViewModel.IsDeletingImage)
            {
                if (iNdx == 0 && ViewModel.Tbl81ImagesList.Count == 0)
                {
                    ViewModel.Tbl81ImagesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];
            }
        }
        ImageDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingImage = false;
    }

    private void BackDetailExpanders()
    {
        ImageDetailExpander.IsExpanded = false;
        ImageDetailExpander.Visibility = Visibility.Collapsed;

        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteImage.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteImage.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteImage.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteImage.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

