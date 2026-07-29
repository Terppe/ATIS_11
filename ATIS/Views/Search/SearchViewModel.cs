using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ATIS.Interfaces;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Search;

public partial class SearchViewModel : INotifyPropertyChanged
{
    private readonly IDataService _dataService;
    private CancellationTokenSource? _cts;

    public ObservableCollection<SearchResultItem> Results { get; } = new();

    public SearchViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasNoResults));
        }
    }

    private string _selectedEntityType = "All";
    public string SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (_selectedEntityType == value) return;
            _selectedEntityType = value;
            OnPropertyChanged();
        }
    }

    public bool HasNoResults => !IsBusy && Results.Count == 0;

    public async Task SearchAsync(string text)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            IsBusy = true;
            Results.Clear();
            OnPropertyChanged(nameof(HasNoResults));

            var rows = await _dataService.SearchAcrossTablesAsync(text, SelectedEntityType);

            foreach (var r in rows)
                Results.Add(r);

            OnPropertyChanged(nameof(HasNoResults));
        }
        catch (OperationCanceledException)
        {
            // ignorieren
        }
        finally
        {
            IsBusy = false;
        }
    }
    public ICommand BackCommand => new RelayCommand(() =>
    {
        var frame = App.MainPageInstance.NavFrame;

        if (frame.CanGoBack)
            frame.GoBack();
    });

    public void ClearResults()
    {
        Results.Clear();
        OnPropertyChanged(nameof(HasNoResults));
    }

    public void NotifyHasNoResultsChanged()
        => OnPropertyChanged(nameof(HasNoResults));

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
