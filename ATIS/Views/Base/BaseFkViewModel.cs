using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ATIS.Views.Base;

public abstract class BaseFkViewModel<TDomain> : BaseViewModel
{
    public ObservableCollection<TDomain> Items { get; } = new();

    private TDomain? _selectedItem;
    public TDomain? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public int DataSetCount => Items.Count;

    public IRelayCommand NewCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    protected BaseFkViewModel()
    {
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
        SaveCommand = new AsyncRelayCommand(SaveItemAsync, () => SelectedItem != null);
        DeleteCommand = new AsyncRelayCommand(DeleteItemAsync, () => SelectedItem != null);
    }

    protected abstract TDomain CreateNewDomain();
    protected abstract Task<TDomain> SaveDomainAsync(TDomain domain);
    protected abstract Task DeleteDomainAsync(TDomain domain);

    public void SetItems(IEnumerable<TDomain> items)
    {
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);
    }

    private void NewItem()
    {
        SelectedItem = CreateNewDomain();
        Items.Add(SelectedItem);
    }

    private async Task SaveItemAsync()
    {
        if (SelectedItem == null)
            return;

        SelectedItem = await SaveDomainAsync(SelectedItem);
    }

    private async Task DeleteItemAsync()
    {
        if (SelectedItem == null)
            return;

        await DeleteDomainAsync(SelectedItem);
        Items.Remove(SelectedItem);
        SelectedItem = Items.FirstOrDefault();
    }
}
