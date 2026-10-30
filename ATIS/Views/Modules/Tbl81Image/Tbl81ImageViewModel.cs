using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WinUI3Localizer;

//    Tbl81ImageViewModel Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl81Image;

public class Tbl81ImageViewModel : BaseBossListViewModel<Models.Tbl81Image>
{
    private readonly FileOpenPicker _filePicker = new();
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl81Image>>>? _activeLoader;
    private readonly Action<string> _langHandler;


    private ImageParentKind _parentKind = ImageParentKind.None;
    private int _currentParentId;
    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl81ImageViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

        _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();

    }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl75Species> SpeciesList { get; private set; } = new();
    public Dictionary<int, string?> SpeciesLookup { get; private set; } = new();

    public async Task LoadSpeciesLookupAsync()
    {
        try
        {
            var list = await _lookupService.GetSpeciesListAsync();
            var lookup = await _lookupService.GetSpeciesLookupAsync();

            SpeciesList.Clear();
            foreach (var item in list)
                SpeciesList.Add(item);

            SpeciesLookup = lookup;
            OnPropertyChanged(nameof(SpeciesLookup));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadSpeciesLookupAsync] {ex}");

            SpeciesList.Clear();
            SpeciesLookup = new Dictionary<int, string?>();
            OnPropertyChanged(nameof(SpeciesLookup));
        }
    }

    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_ImageId")) ? "Id" : loc.GetLocalizedString("HeaderSort_ImageId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_ImageInfo")) ? "Name" : loc.GetLocalizedString("HeaderSort_ImageInfo"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------                   
    private enum ImageParentKind
    {
        None,
        Species,
        Food,
        Disease,
        Miscellaneous
    }
    public bool IsSpeciesParent => _parentKind == ImageParentKind.Species
                                   || _parentKind == ImageParentKind.None; // Boss-Modus: wie bisher anzeigen

    private void SetParentContext(ImageParentKind kind, int parentId)
    {
        _parentKind = kind;
        _currentParentId = parentId;
        OnPropertyChanged(nameof(IsSpeciesParent));
    }

    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
    {
        SelectedDetailTabIndex = index;
    });

    protected override void OnSelectedDetailTabIndexChanged()
    {
        _ = UpdateButtonStatesAsync();
    }

    private async Task UpdateButtonStatesAsync()
    {


        UpdateRefreshState();

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    public async Task ApplyFilterAsync(string? filter)
    {
        if (IsBossMode)
        {
            SearchText = filter;
            await ReloadAsync();
        }
    }

    private bool _showBusyOverlay;
    public bool ShowBusyOverlay
    {
        get => _showBusyOverlay;
        set => SetProperty(ref _showBusyOverlay, value);
    }

    protected override void OnLoadingStarted()
    {
        ShowBusyOverlay = true;
    }

    protected override void OnLoadingFinished()
    {
        ShowBusyOverlay = false;
    }
    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------
    private void ResetParentContext()
    {
        _parentKind = ImageParentKind.None;
        _currentParentId = 0;
        _activeLoader = null;
        OnPropertyChanged(nameof(IsSpeciesParent));
    }
    public async Task LoadAsync()
    {
        System.Diagnostics.Debug.WriteLine("[ImageVM] LoadAsync START");
        IsLoading = true;
        try
        {
            _activeLoader = null;
            _currentParentId = 0;
            _parentKind = ImageParentKind.None;

            ResetParentContext();
            IsInParentMode = false;

            try
            {
                await LoadSpeciesLookupAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadAsync -> LoadSpeciesLookupAsync] {ex}");
            }

            Items = new ObservableCollection<Models.Tbl81Image>();
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(IsSpeciesParent));
            try
            {
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
            }
        }
        finally
        {
            IsLoading = false;
            System.Diagnostics.Debug.WriteLine("[ImageVM] LoadAsync END");
        }
    }

    public async Task LoadForSpeciesAsync(int speciesId)
    {
        SetParentContext(ImageParentKind.Species, speciesId);
        await LoadInternalAsync(speciesId, Data.GetImageForSpeciesAsync);
    }

    public async Task LoadForFoodAsync(int foodId)
    {
        SetParentContext(ImageParentKind.Food, foodId);
        await LoadInternalAsync(foodId, Data.GetImageForFoodAsync);
    }

    public async Task LoadForDiseaseAsync(int diseaseId)
    {
        SetParentContext(ImageParentKind.Disease, diseaseId);
        await LoadInternalAsync(diseaseId, Data.GetImageForDiseaseAsync);
    }

    public async Task LoadForMiscellaneousAsync(int miscellaneousId)
    {
        SetParentContext(ImageParentKind.Miscellaneous, miscellaneousId);
        await LoadInternalAsync(miscellaneousId, Data.GetImageForMiscellaneousAsync);
    }

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl81Image>>> loader)
    {
        System.Diagnostics.Debug.WriteLine($"[ImageVM] LoadInternalAsync START parentId={parentId}");
        IsLoading = true;
        ShowBusyOverlay = true;

        try
        {
            _currentParentId = parentId;
            _activeLoader = loader;

            var items = await loader(parentId);
            Items = new ObservableCollection<Models.Tbl81Image>(items);
            OnPropertyChanged(nameof(HasItems));

            DataSetCount = Items.Count;

            SelectedItem = null;
            Item = null;
        }
        finally
        {
            ShowBusyOverlay = false;
            IsLoading = false;
            System.Diagnostics.Debug.WriteLine($"[ImageVM] LoadInternalAsync END parentId={parentId}");
        }
    }
    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl81Image>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchImageWithSpeciesAndGenusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl81Image>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override Task OnSelectedItemChangedAsync()
    {
        // Das EditControl arbeitet mit Item,
        // die Liste mit SelectedItem.
        Item = SelectedItem;
        // Tbl81Image hat keine eigenen Children.

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override Task LoadChildrenAsync(int entityId)
    {
        return Task.CompletedTask;
    }
    protected override Task ClearChildCollectionsAsync()
    {
        return Task.CompletedTask;
    }
    private Models.Tbl81Image? _item;

    public Models.Tbl81Image? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasActiveItem));
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }
    public bool HasActiveItem => Item != null;

    public void PrepareViewState()
    {
        // Keine Auswahl: EditControl schließen
        if (SelectedItem == null)
        {
            Item = null;
            SelectedDetailTabIndex = 0;
            OnPropertyChanged(nameof(IsDetailVisible));
        }
        else
        {
            // Falls bereits ein Datensatz ausgewählt ist,
            // EditControl mit diesem Datensatz synchronisieren.
            Item = SelectedItem;
        }

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl81Image item, string column)
        => column switch
        {
            "Id" => item.ImageId,
            "Name" => item.Info ?? "",
            _ => ""
        };

    // ---------------------------------------------------------
    // Image/Video
    // ---------------------------------------------------------
    //private bool _isLoading;
    //public bool IsLoading
    //{
    //    get => _isLoading;
    //    set
    //    {
    //        if (_isLoading != value)
    //        {
    //            _isLoading = value;
    //            OnPropertyChanged(nameof(IsLoading));
    //        }
    //    }
    //}


    public async Task LoadFromUrlAsync(string url)
    {
        try
        {
            if (Item == null)
                Item = CreateNewItem();

            if (string.IsNullOrWhiteSpace(url))
            {
                await Dialog.ShowErrorAsync("Error_InvalidUrl_Title", "Error_InvalidUrl_Message");
                return;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                await Dialog.ShowErrorAsync("Error_InvalidUrl_Title", "Error_InvalidUrl_Message");
                return;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                await Dialog.ShowErrorAsync("Error_InvalidUrl_Title", "Error_InvalidUrl_Message");
                return;
            }

            using var http = new HttpClient();
            using var response = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null || bytes.Length == 0)
            {
                await Dialog.ShowErrorAsync("Error_LoadMediaFromUrl_Title", "Error_LoadMediaFromUrl_Message");
                return;
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            var extension = Path.GetExtension(uri.AbsolutePath);
            var mediaType = NormalizeMediaType(contentType, extension);

            if (string.Equals(mediaType, "html", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(contentType, "text/html", StringComparison.OrdinalIgnoreCase))
            {
                await Dialog.ShowErrorAsync("Error_InvalidMediaUrl_Title", "Error_InvalidMediaUrl_Message");
                return;
            }

            if (!IsSupportedMediaType(mediaType))
            {
                await Dialog.ShowErrorAsync("Error_UnsupportedMediaType_Title", "Error_UnsupportedMediaType_Message");
                return;
            }

            Item.Filestream = bytes;
            Item.ImageMimeType = mediaType;

            if (Item.FilestreamId == Guid.Empty)
                Item.FilestreamId = Guid.NewGuid();

            Debug.WriteLine($"[URL LOAD] item hash: {Item.GetHashCode()}");
            Debug.WriteLine($"[URL LOAD] bytes: {Item.Filestream?.Length ?? 0}");
            Debug.WriteLine($"[URL LOAD] type: {Item.ImageMimeType}");

            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(CanSave));
        }
        catch (Exception xe)
        {
            SimpleLog.Log(xe);
            await Dialog.ShowErrorAsync("Error_LoadMediaFromUrl_Title", "Error_LoadMediaFromUrl_Message");
        }
    }

    private static bool IsSupportedMediaType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return false;

        switch (type.Trim().ToLowerInvariant())
        {
            // Bilder
            case "jpg":
            case "jpeg":
            case "png":
            case "gif":
            case "bmp":
            case "tif":
            case "tiff":
            case "ico":
            case "webp":
            case "wmf":

            // Video
            case "mp4":
            case "avi":
            case "mov":
            case "mpg":
            case "wmv":
            case "flv":
            case "swf":

            // Audio
            case "mp3":
            case "wav":
            case "m4a":
                return true;

            default:
                return false;
        }
    }
    private string? _statusMessage;
    public string? StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    private static string? NormalizeMediaType(string? contentType, string? extension)
    {
        if (!string.IsNullOrWhiteSpace(extension))
            return GetImageTypeFromExtension(extension);

        if (string.IsNullOrWhiteSpace(contentType))
            return null;

        var slashIndex = contentType.LastIndexOf('/');
        if (slashIndex >= 0 && slashIndex < contentType.Length - 1)
            return contentType[(slashIndex + 1)..].ToLowerInvariant();

        return contentType.ToLowerInvariant();
    }
    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl81Image CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        var item = new Models.Tbl81Image
        {
            SpeciesId = 0,
            FoodId = 0,
            DiseaseId = 0,
            MiscellaneousId = 0,
            FilestreamId = Guid.NewGuid(),
            ShotDate = DateTime.Today
        };

        switch (_parentKind)
        {
            case ImageParentKind.Species:
                item.SpeciesId = _currentParentId;
                break;

            case ImageParentKind.Food:
                item.FoodId = _currentParentId;
                break;

            case ImageParentKind.Disease:
                item.DiseaseId = _currentParentId;
                break;

            case ImageParentKind.Miscellaneous:
                item.MiscellaneousId = _currentParentId;
                break;
        }

        return item;
    }    // ---------------------------------------------------------
    // Image/Video
    // ---------------------------------------------------------

    public async Task SelectFileForCurrentItemAsync()
    {
        try
        {
            if (Item == null)
                Item = CreateNewItem();

            var file = await PickFileAsync();
            if (file == null || string.IsNullOrWhiteSpace(file.Path))
                return;

            Item.Filestream = await File.ReadAllBytesAsync(file.Path);

            var fileType = GetImageTypeFromExtension(file.FileType);


            if (!string.IsNullOrWhiteSpace(fileType))
                Item.ImageMimeType = fileType;

            if (Item.FilestreamId == Guid.Empty)
                Item.FilestreamId = Guid.NewGuid();

            Debug.WriteLine($"LOAD item hash: {Item.GetHashCode()}");
            Debug.WriteLine($"LOAD bytes: {Item.Filestream?.Length ?? 0}");
            Debug.WriteLine($"LOAD type: {Item.ImageMimeType}");

            OnPropertyChanged(nameof(CanSave));
        }
        catch (Exception xe)
        {
          //  await AllDialogs.ErrorMessageDialogAsync(xe.ToString());
            SimpleLog.Log(xe);
        }
    }
    private async Task<StorageFile?> PickFileAsync()
    {
        var filePicker = new FileOpenPicker();
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(filePicker, hwnd);

        filePicker.FileTypeFilter.Clear();
        filePicker.FileTypeFilter.Add(".jpg");
        filePicker.FileTypeFilter.Add(".jpeg");
        filePicker.FileTypeFilter.Add(".png");
        filePicker.FileTypeFilter.Add(".gif");
        filePicker.FileTypeFilter.Add(".bmp");
        filePicker.FileTypeFilter.Add(".tif");
        filePicker.FileTypeFilter.Add(".tiff");
        filePicker.FileTypeFilter.Add(".ico");
        filePicker.FileTypeFilter.Add(".webp");
        filePicker.FileTypeFilter.Add(".wmf");
        filePicker.FileTypeFilter.Add(".wmv");
        filePicker.FileTypeFilter.Add(".mpg");
        filePicker.FileTypeFilter.Add(".mpeg");
        filePicker.FileTypeFilter.Add(".mp3");
        filePicker.FileTypeFilter.Add(".wav");
        filePicker.FileTypeFilter.Add(".m4a");
        filePicker.FileTypeFilter.Add(".mp4");
        filePicker.FileTypeFilter.Add(".avi");
        filePicker.FileTypeFilter.Add(".mov");
        filePicker.FileTypeFilter.Add(".swf");
        filePicker.FileTypeFilter.Add(".flv");
        filePicker.FileTypeFilter.Add(".x-shockwave-flash");

        filePicker.ViewMode = PickerViewMode.Thumbnail;

        return await filePicker.PickSingleFileAsync();
    }
    private static string? GetImageTypeFromExtension(string? extension)
    {
        return extension?.TrimStart('.').ToLowerInvariant();
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    public bool CanSave => Item?.Filestream is { Length: > 0 };

    protected override async Task<Models.Tbl81Image?> SaveItemAsync(Models.Tbl81Image item)
    {
        if (item.Filestream == null || item.Filestream.Length == 0)
        {
            await Dialog.ShowErrorAsync("Error_NoFile_Title", "Error_NoFile_Message");
            return null;
        }

        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_MissingInfo_Title", "Error_MissingInfo_Message");
            return null;
        }

        if (await Data.ImageExistsAsync(item))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertImageAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        return saved;
    }
    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl81Image item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.Info;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl81Image item)
    {
        if (item.ImageId == 0)
            return false;

        await Data.DeleteImageAsync(item.ImageId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl81Image item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl81Image item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl81Image item)
        => item.ImageId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    protected override void ExecuteBack()
    {
        // SelectedItem zurücksetzen -> Detail schließen
        SelectedItem = null;
        OnPropertyChanged(nameof(IsDetailVisible));

        // Bei Simple-Page NICHT hart auf true setzen
        // IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
