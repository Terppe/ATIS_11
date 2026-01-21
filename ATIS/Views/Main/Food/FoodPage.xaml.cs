using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Views.Report.Pdf.TblFoods;
using CommunityToolkit.WinUI.UI.Controls;

namespace ATIS.Views.Main.Food;

public sealed partial class FoodPage : Page
{
    public FoodViewModel ViewModel { get; set; }

    public FoodPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = App.GetService<FoodViewModel>();   // wichtig: vor InitializeComponent

        SimpleLog.Info("FoodPage opened");

    }

    // =========================================================
    // Search
    // =========================================================

    private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.SearchAsync(args.QueryText);
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        ViewModel.UpdateSuggestions(sender.Text);
    }

    private void SearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SearchBox.Focus(FocusState.Programmatic);
    }

    // =========================================================
    // Sorting
    // =========================================================

    private void FoodDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid)?.Sort(e.Column, ViewModel.Foods.Sort);
    }

    // =========================================================
    // PDF / Selection
    // =========================================================

    public ICommand PdfFoodCommand => new WinUICommunity.RelayCommand(execute: () =>
    {
        if (ViewModel.FoodSelected?.FoodId is not > 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var reportFoodsWindow = new ReportFoodsWindow(ViewModel.FoodSelected.FoodId);
        reportFoodsWindow.GeneratePdfFood(ViewModel.FoodSelected.FoodId);
    });

    private void FoodSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.FoodSelected?.FoodId is not > 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportFoodsWindow(ViewModel.FoodSelected.FoodId)
        {
            IsAlwaysOnTop = true
        };
        window.Activate();
    }
}


