// Views/Base/BaseBossListViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Base;

/// <summary>
/// TEMPLATE für alle "Boss" ViewModels (Parent mit hierarchischen Kindern)
/// Erbt von BaseListViewModel und erweitert um Hierarchie-Features
/// </summary>
public abstract class BaseBossListViewModel<T> : BaseListViewModel<T>
    where T : class, new()
{
    protected readonly IDialogService _dialog;
    protected readonly HierarchyCoordinator _coordinator;

    // ============================================
    // HIERARCHIE-CONFIGURATION (ABSTRACT)
    // ============================================
    protected abstract HierarchyLevel CurrentLevel { get; }
    protected abstract HierarchyLevel ParentLevel { get; }

    // ============================================
    // CHILD VIEWMODELS (Dictionary statt einzelne Properties!)
    // ============================================
    protected readonly Dictionary<string, object> ChildViewModels = new();

    // ============================================
    // EDIT STATE FÜR BOSS
    // ============================================
    private bool _isEditVisible;
    public bool IsEditVisible
    {
        get => _isEditVisible;
        set => SetProperty(ref _isEditVisible, value);
    }

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public BaseBossListViewModel(
        IDataService data,
        IDialogService dialog,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _dialog = dialog;
        _coordinator = coordinator;

        // Sorting Command zusätzlich zu Base
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        // Subscribe auf Parent-Level-Änderungen
        _coordinator.LevelChanged += OnHierarchyLevelChanged;
    }

    // ============================================
    // SORT COMMAND
    // ============================================
    public ICommand SortCommand { get; }

    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    public void SortBy(string column)
    {
        if (Items?.Count == 0) return;

        if (_currentSortColumn == column)
            _sortAscending = !_sortAscending;
        else
        {
            _currentSortColumn = column;
            _sortAscending = true;
        }

        var sorted = SortItems(Items, column, _sortAscending).ToList();
        Items = new ObservableCollection<T>(sorted);
        SelectedItem = null;
        UpdateHeaderTexts();
    }

    // ============================================
    // HIERARCHY NAVIGATION
    // ============================================
    private void OnHierarchyLevelChanged(object? sender, HierarchyLevelChangedArgs args)
    {
        // Reagiert NUR auf Parent-Level-Änderungen
        if (args.Level != ParentLevel)
            return;

        Items?.Clear();
        SelectItemSilently(default);
        IsEditVisible = false;

        if (args.SelectedId.HasValue && args.SelectedId > 0)
            _ = LoadForParentAsync(args.SelectedId.Value);
    }

    // ============================================
    // ITEM SELECTION
    // ============================================
    public override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
        {
            _coordinator.SetSelectedId(CurrentLevel, null);
            await ClearChildCollectionsAsync();
            return;
        }

        // Für neue Items (ID == 0)
        if (SelectedItem is IHierarchicalEntity entity && entity.GetEntityId() == 0)
        {
            _coordinator.SetSelectedId(CurrentLevel, 0);
            await ClearChildCollectionsAsync();
            return;
        }

        // Für existierende Items
        if (SelectedItem is IHierarchicalEntity existingEntity)
        {
            _coordinator.SetSelectedId(CurrentLevel, existingEntity.GetEntityId());
            await LoadChildrenAsync(existingEntity.GetEntityId());
        }
    }

    // ============================================
    // NEW ITEM OVERRIDE
    // ============================================
    public override void New()
    {
        //if (!ValidateBeforeNew())
        //{
        //    _ = _dialog.ShowErrorAsync("Fehler", GetNewValidationErrorMessage());
        //    return;
        //}

        var item = CreateNewItem();
        Items ??= new ObservableCollection<T>();
        Items.Add(item);

        SelectedItem = item;
        OnAfterNew(item);
        IsEditVisible = true;
    }

    protected virtual bool ValidateBeforeNew()
    {
        var parentId = _coordinator.GetSelectedId(ParentLevel);
        return parentId.HasValue && parentId > 0;
    }

    protected virtual string GetNewValidationErrorMessage()
        => $"Bitte zuerst ein Element auf der Parent-Ebene auswählen.";

    // ============================================
    // SAVE OVERRIDE
    // ============================================

    public override async Task SaveAsync()
    {
        if (SelectedItem == null)
            return;

        if (!await ConfirmSaveAsync(SelectedItem))
            return;

        if (!await ValidateBeforeSaveAsync(SelectedItem))
            return;

        var saved = await SaveItemAsync(SelectedItem);
        if (saved == null)
            return;

        await OnBeforeReloadAfterSaveAsync(saved);

        var parentId = _coordinator.GetSelectedId(ParentLevel);
        if (parentId.HasValue)
            // ✅ autoSelect=true sorgt für Fokus + Scroll zum gespeicherten Item
            await ReloadAndSelectAsync(saved, autoSelect: true);

        IsEditVisible = false;
    }

    // ============================================
    // DELETE OVERRIDE
    // ============================================
    public override async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        var toDelete = SelectedItem;

        if (!await ConfirmDeleteAsync(toDelete))
            return;

        var deleted = await DeleteItemAsync(toDelete);
        if (!deleted)
            return;

        Items?.Remove(toDelete);
        SelectedItem = null;
        IsEditVisible = false;
    }

    // ============================================
    // ABSTRACT - MÜSSEN IMPLEMENTIERT WERDEN
    // ============================================
    protected abstract Task LoadForParentAsync(int parentId);
    protected abstract Task ClearChildCollectionsAsync();
    protected abstract Task LoadChildrenAsync(int entityId);
    protected abstract IEnumerable<T> SortItems(IEnumerable<T> items, string column, bool ascending);
    protected abstract void UpdateHeaderTexts();

    // ============================================
    // ABSTRACT VON BASE (müssen auch implementiert werden)
    // ============================================
    // protected abstract Task<IEnumerable<TModel>> LoadItemsAsync();
    // protected abstract TModel CreateNewItem();
    // protected abstract Task<TModel?> SaveItemAsync(TModel item);
    // protected abstract Task<bool> DeleteItemAsync(TModel item);
}
