using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Main.Food;

public partial class FoodViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // =========================================================
    // Constructor
    // =========================================================
    public FoodViewModel(IDataService dataService)
    {
        _dataService = dataService;

        SearchCommand = new AsyncRelayCommand<string>(SearchAsync);
        PdfFoodCommand = new RelayCommand(ExecutePdfFood, CanExecutePdfFood);
        EditFoodCommand = new RelayCommand(ExecuteEditFood, CanExecuteEditFood);
        DeleteFoodCommand = new RelayCommand(ExecuteDeleteFood, CanExecuteDeleteFood);
    }

    // =========================================================
    // Collections
    // =========================================================
    public ObservableCollection<TblFood> Foods { get; } = [];

    // =========================================================
    // Loading Indicator
    // =========================================================
    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    // =========================================================
    // Dataset Count
    // =========================================================
    private int _foodDataSetCount;
    public int FoodDataSetCount
    {
        get => _foodDataSetCount;
        set => SetProperty(ref _foodDataSetCount, value);
    }
    public string FoodDataSetCountText =>
        $"{FoodDataSetCount} Datensätze gefunden";

    // =========================================================
    // Search Placeholder (für Localizer optional)
    // =========================================================
    public string SearchPlaceholder => "Suche nach Lebensmitteln…";

    // =========================================================
    // Search Logic
    // =========================================================
    public ICommand SearchCommand { get; }

    public async Task SearchAsync(string? query)
    {
        try
        {
            IsLoading = true;
            Foods.Clear();

            if (string.IsNullOrWhiteSpace(query))
            {
                FoodDataSetCount = 0;
                OnPropertyChanged(nameof(FoodDataSetCountText));
                AllDialogs.NoDatasetFoundWarnMessageDialog();
                return;
            }

            var result = await _dataService.GetFoodsByNameOrIdAsync(query);

            foreach (var item in result)
                Foods.Add(item);

            FoodDataSetCount = Foods.Count;
            OnPropertyChanged(nameof(FoodDataSetCountText));

            FoodSelected = Foods.FirstOrDefault();

            if (Foods.Count == 0)
                AllDialogs.NoDatasetFoundWarnMessageDialog();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void UpdateSuggestions(string text)
    {
        // später: Suggestion-Logik
    }

    // =========================================================
    // Selected Item
    // =========================================================
    private TblFood _foodSelected;
    public TblFood FoodSelected
    {
        get => _foodSelected;
        set
        {
            if (SetProperty(ref _foodSelected, value))
            {
                (PdfFoodCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditFoodCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (DeleteFoodCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    // =========================================================
    // Commands (PDF, Edit, Delete)
    // =========================================================
    public ICommand PdfFoodCommand { get; }
    public ICommand EditFoodCommand { get; }
    public ICommand DeleteFoodCommand { get; }

    private bool CanExecutePdfFood() => FoodSelected?.FoodId > 0;
    private void ExecutePdfFood()
    {
        // später: PDF-Service
    }

    private bool CanExecuteEditFood() => FoodSelected != null;
    private void ExecuteEditFood()
    {
        // später: Edit-Service
    }

    private bool CanExecuteDeleteFood() => FoodSelected != null;
    private void ExecuteDeleteFood()
    {
        // später: Delete-Service
    }
}


