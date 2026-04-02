using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ATIS.Views.Base;

public abstract class BaseListViewModel<TModel> : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // ---------------------------------------------------------
    // LIST + SELECTION
    // ---------------------------------------------------------

    public ObservableCollection<TModel> Items { get; } = new();

    private TModel? _selectedItem;
    public TModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!Equals(_selectedItem, value))
            {
                _selectedItem = value;
                OnPropertyChanged();
                _ = OnSelectedItemChangedAsync();
            }
        }
    }

    // ---------------------------------------------------------
    // ABSTRACTS
    // ---------------------------------------------------------

    /// <summary>
    /// Lädt alle Items aus DB/Service.
    /// </summary>
    protected abstract Task<IEnumerable<TModel>> LoadItemsAsync();

    /// <summary>
    /// Wird aufgerufen, wenn SelectedItem geändert wurde.
    /// </summary>
    protected virtual Task OnSelectedItemChangedAsync() => Task.CompletedTask;

    // ---------------------------------------------------------
    // RELOAD
    // ---------------------------------------------------------

    public async Task ReloadAsync()
    {
        await ReloadAndSelectAsync(default);
    }

    public async Task ReloadAndSelectAsync(TModel? saved)
    {
        var old = SelectedItem;

        var list = await LoadItemsAsync();

        Items.Clear();
        foreach (var item in list)
            Items.Add(item);

        if (saved != null)
            SelectedItem = Items.FirstOrDefault(x => Equals(x, saved));
        else if (old != null)
            SelectedItem = Items.FirstOrDefault(x => Equals(x, old));
        else
            SelectedItem = Items.FirstOrDefault();
    }
}
