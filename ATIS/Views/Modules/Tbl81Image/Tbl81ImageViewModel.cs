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
using Microsoft.UI.Xaml.Media.Imaging;
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
    public IRelayCommand BackCommand { get; }
    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl81Image>>>? _activeLoader;
    private readonly Action<string> _langHandler;

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
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

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
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_ImageName")) ? "Name" : loc.GetLocalizedString("HeaderSort_ImageName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------                   

    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
    {
        SelectedDetailTabIndex = index;
    });

    private int _selectedDetailTabIndex;
    public int SelectedDetailTabIndex
    {
        get => _selectedDetailTabIndex;
        set
        {
            if (SetProperty(ref _selectedDetailTabIndex, value))
            {
                _ = UpdateButtonStatesAsync();
            }
        }
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

    public bool IsDetailVisible => SelectedItem != null;
    public bool IsEditMode => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {

        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        // Für Simple-Page sinnvoll: Boss-Modus aktiv
        IsInParentMode = false;

        // Lookup nur versuchen, Fehler darf UI nicht stoppen
        try
        {
            await LoadSpeciesLookupAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> LoadSpeciesLookupAsync] {ex}");
            // absichtlich weiterlaufen
        }

        Items = new ObservableCollection<Models.Tbl81Image>();
        OnPropertyChanged(nameof(HasItems));

        // Hauptdaten trotzdem laden
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> ReloadAsync] {ex}");
            // optional: Dialog zeigen
            await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSpeciesAsync(int speciesId)
        => await LoadInternalAsync(speciesId, Data.GetImageForSpeciesAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl81Image>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl81Image>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;

        // Liste anzeigen, Editor erst nach Auswahl eines Eintrags.
        SelectedItem = null;
        Item = null;

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
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl81Image CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        var currentSpecies = Item?.Species;

        return new Models.Tbl81Image
        {
            SpeciesId = _currentParentId,
            Species = currentSpecies,
            FilestreamId = Guid.NewGuid(),
            ShotDate = DateTime.Today
        };
    }

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
//            Item.ImageMimeType = GetImageTypeFromExtension(file.FileType);

            var fileType = GetImageTypeFromExtension(file.FileType);
            if (!string.IsNullOrWhiteSpace(fileType))
                Item.ImageMimeType = fileType;

            if (Item.FilestreamId == Guid.Empty)
                Item.FilestreamId = Guid.NewGuid();

            Debug.WriteLine($"LOAD item hash: {Item.GetHashCode()}");
            Debug.WriteLine($"LOAD bytes: {Item.Filestream?.Length ?? 0}");
            Debug.WriteLine($"LOAD type: {Item.ImageMimeType}");
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.ToString());
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
        filePicker.FileTypeFilter.Add(".mp4");
        filePicker.FileTypeFilter.Add(".avi");
        filePicker.FileTypeFilter.Add(".mov");
        filePicker.FileTypeFilter.Add(".swf");
        filePicker.FileTypeFilter.Add(".flv");

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

    protected override async Task<Models.Tbl81Image?> SaveItemAsync(Models.Tbl81Image item)
    {
        Debug.WriteLine($"SAVE item hash: {item.GetHashCode()}");
        Debug.WriteLine($"SAVE bytes: {item.Filestream?.Length ?? 0}");

        if (item.Filestream == null || item.Filestream.Length == 0)
        {
            await Dialog.ShowErrorAsync("Error_NoFile_Title", "Error_NoFile_Message");
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
    private void ExecuteBack()
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

    //private static string? GetMimeTypeFromExtension(string? extension)
    //{
    //    return extension?.ToLowerInvariant() switch
    //    {
    //        ".jpg" or ".jpeg" => "image/jpeg",
    //        ".png" => "image/png",
    //        ".gif" => "image/gif",
    //        ".bmp" => "image/bmp",
    //        ".tif" or ".tiff" => "image/tiff",
    //        ".mp4" => "video/mp4",
    //        ".avi" => "video/x-msvideo",
    //        ".mov" => "video/quicktime",
    //        _ => null
    //    };
    //}

    //private string _selectedPath;
    //public string SelectedPath
    //{
    //    get => _selectedPath;
    //    set => SetProperty(ref _selectedPath, value);
    //}

    //public async Task LoadFileIntoSelectedItemAsync()
    //{
    //    try
    //    {
    //        if (Item == null)
    //            return;

    //        var file = await PickFileAsync();
    //        if (file == null || string.IsNullOrWhiteSpace(file.Path))
    //            return;

    //   //     SelectedPath = file.Path;

    //        Item.Filestream = await File.ReadAllBytesAsync(file.Path);
    //        Item.ImageMimeType = GetMimeTypeFromExtension(file.FileType);
    //   //     Item.Info = file.Name;
    ////        Item.GenusSpeciesFullName = Item.GenusSpeciesFullName;
    //        Debug.WriteLine($"LOAD item hash: {Item.GetHashCode()}");
    //        Debug.WriteLine($"LOAD bytes: {Item.Filestream?.Length ?? 0}");
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}
    //protected override async Task NewAsync()
    //{
    //    try
    //    {
    //        var file = await PickFileAsync();
    //        if (file == null || string.IsNullOrWhiteSpace(file.Path))
    //            return;

    //        if (Item == null)
    //        {
    //            Item = CreateNewItem();
    //        }

    //        SelectedPath = file.Path;
    //        Item.Filestream = await File.ReadAllBytesAsync(file.Path);
    //        Item.ImageMimeType = GetMimeTypeFromExtension(file.FileType);
    //    //    Item.Info = file.Name;
    //        Item.GenusName = Item.GenusSpeciesFullName;
    //        Item.FilestreamId = Guid.NewGuid();

    //        Debug.WriteLine($"NEW item hash: {Item.GetHashCode()}");
    //        Debug.WriteLine($"NEW bytes: {Item.Filestream?.Length ?? 0}");
    //    }
    //    catch (Exception xe)
    //    {
    //        await AllDialogs.ErrorMessageDialogAsync(xe.ToString());
    //        SimpleLog.Log(xe);
    //    }
    //}

}
