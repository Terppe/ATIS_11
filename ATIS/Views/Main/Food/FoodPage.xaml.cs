using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Main.FoodPdf;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FoodPage : Page
{
    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public FoodViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public FoodPage()
    {
        ViewModel = App.GetService<FoodViewModel>();
        InitializeComponent();
        SimpleLog.Info(" FoodPage opended ");
    }
    #endregion [Constructor]         

    #region [Disease Methods]

    private async void GetFoodsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblFoodsList?.Clear();

            ViewModel.FoodItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblFoodsList?.Clear();

                FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
                //     ListViewFood.ItemsSource = ViewModel.TblFoodsList;
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblFoodsList ??= [];
                ViewModel.TblFoodsList = int.TryParse(_search, out var id)
                ? [.. Context.TblFoods
                .Where(e => e.FoodId == id)]
                : [.. Context.TblFoods
                .Where(e => e.FoodName.StartsWith(_search))
                .OrderBy(a => a.FoodName)];

            });

            ViewModel.FoodDataSetCount = ViewModel.TblFoodsList.Count;

            if (ViewModel.TblFoodsList is { Count: 0 })
            {
                ViewModel.TblFoodsList?.Clear();

                //    AppBarButtonSync.IsEnabled = false;


                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(FoodPage));
            }
            else
            {
                FoodDataGrid.ItemsSource = ViewModel.TblFoodsList;
                //       ListViewFood.ItemsSource = ViewModel.TblFoodsList;
                ViewModel.FoodSelected = ViewModel.TblFoodsList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private void FoodSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        FoodSearchBox.Visibility = Visibility.Visible;
        FoodSearchBox.Focus(FocusState.Programmatic);
        FoodSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void FoodDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblFoodsList.Sort);
    }


    private async void ViewFoodHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewFoodHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewFoodHyperlink()
    {
        if (FoodDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            //          SingleDisease = App.SingleDisease(ViewModel.DiseaseSelected.DiseaseId);
        }
        return Task.CompletedTask;
    }

    private void FoodSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.FoodSelected.FoodId == 0)
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
    public ICommand PdfFoodCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportFoodsWindow = new ReportFoodsWindow(ViewModel.FoodSelected.FoodId);
        reportFoodsWindow.GeneratePdfFood(ViewModel.FoodSelected.FoodId);
    });


    #endregion [Disease Methods]                    

}
