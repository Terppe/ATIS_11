using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Base;

public abstract class BaseHierarchyBossViewModel<TModel>
    : BaseBossListViewModel<TModel>
    where TModel : class
{
    // ---------------------------------------------------------
    // FIELDS
    // ---------------------------------------------------------
    protected Action<TModel>? AssignParentFk;
    protected Func<int, Task<IEnumerable<TModel>>>? LoadByParent;
    protected int CurrentParentId;

    // ---------------------------------------------------------
    // CTOR
    // ---------------------------------------------------------
    protected BaseHierarchyBossViewModel(
        IDataService data,
        IDialogService dialog,
        HierarchyCoordinator coordinator)
        : base(data, dialog, coordinator)
    {
    }

    // ---------------------------------------------------------
    // PARENT RESOLUTION (Hook für Subklassen)
    // ---------------------------------------------------------
    protected virtual void ResolveParent<TParent>(int parentId)
    {
        // Basis macht nichts – Subklassen können Parent-FK setzen
    }

    // ---------------------------------------------------------
    // LOAD PARENT (generisch, mit TParent)
    // ---------------------------------------------------------
    public async Task LoadParentAsync<TParent>(int parentId)
    {
        CurrentParentId = parentId;

        ResolveParent<TParent>(parentId);

        var items = LoadByParent != null
            ? await LoadByParent(parentId)
            : Enumerable.Empty<TModel>();

        Items = new ObservableCollection<TModel>(items);
        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD PARENT (override BaseBoss)
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        if (LoadByParent == null)
            return;

        var items = await LoadByParent(parentId);
        Items = new ObservableCollection<TModel>(items);

        DataSetCount = Items.Count;

        // Wichtig: SelectedItem NICHT zurücksetzen
        // SelectedItem = null;

        await LoadChildrenAsync(parentId);
    }

    // ---------------------------------------------------------
    // LOAD ROOT (Boss-Start)
    // ---------------------------------------------------------
    public virtual async Task LoadAsync()
    {
        LoadByParent = null;
        CurrentParentId = 0;

        await LoadLookupsAsync();

        Items = new ObservableCollection<TModel>();
        SelectedItem = null;

        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOOKUPS (Hook für Subklassen)
    // ---------------------------------------------------------
    public virtual Task LoadLookupsAsync()
    {
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // CLEAR CHILDREN (Default)
    // ---------------------------------------------------------
    protected override Task ClearChildCollectionsAsync()
    {
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SELECTED ITEM CHANGED (zentrale Hierarchie-Logik)
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        // Detail sichtbar, wenn ein Item ausgewählt ist
        IsDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
        {
            // generischer Hook für alle Hierarchie-VMs
            await LoadChildrenAsync(GetId(SelectedItem));
        }
        else
        {
            await ClearChildCollectionsAsync();
        }
    }

    // ---------------------------------------------------------
    // DETAIL VISIBILITY
    // ---------------------------------------------------------
    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }
}
