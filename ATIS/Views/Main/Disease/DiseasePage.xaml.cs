using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Report.Pdf.TblDisease;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main.Disease;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DiseasePage : Page
{
    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public DiseaseViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public DiseasePage()
    {
        ViewModel = App.GetService<DiseaseViewModel>();
        InitializeComponent();
        SimpleLog.Info(" DiseasePage opended ");
    }
    #endregion [Constructor]         

    #region [Disease Methods]

    private async void GetDiseasesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblDiseasesList?.Clear();

            ViewModel.DiseaseItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblDiseasesList?.Clear();

                DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
           //     ListViewDisease.ItemsSource = ViewModel.TblDiseasesList;
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblDiseasesList ??= [];
                ViewModel.TblDiseasesList = int.TryParse(_search, out var id)
                ? [.. Context.TblDiseases
                .Where(e => e.DiseaseId == id)]
                : [.. Context.TblDiseases
                .Where(e => e.DiseaseName.StartsWith(_search))
                .OrderBy(a => a.DiseaseName)];

            });

            ViewModel.DiseaseDataSetCount = ViewModel.TblDiseasesList.Count;

            if (ViewModel.TblDiseasesList is { Count: 0 })
            {
                ViewModel.TblDiseasesList?.Clear();

            //    AppBarButtonSync.IsEnabled = false;


                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(DiseasePage));
            }
            else
            {
                DiseaseDataGrid.ItemsSource = ViewModel.TblDiseasesList;
         //       ListViewDisease.ItemsSource = ViewModel.TblDiseasesList;
                ViewModel.DiseaseSelected = ViewModel.TblDiseasesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private void DiseaseSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        DiseaseSearchBox.Visibility = Visibility.Visible;
        DiseaseSearchBox.Focus(FocusState.Programmatic);
        DiseaseSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void DiseaseDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblDiseasesList.Sort);
    }


    private async void ViewDiseaseHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewDiseaseHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewDiseaseHyperlink()
    {
        if (DiseaseDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
  //          SingleDisease = App.SingleDisease(ViewModel.DiseaseSelected.DiseaseId);
        }
        return Task.CompletedTask;
    }

    private void DiseaseSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.DiseaseSelected.DiseaseId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportDiseasesWindow(ViewModel.DiseaseSelected.DiseaseId)
        {
            IsAlwaysOnTop = true
        };
        window.Activate();
    }
    public ICommand PdfDiseaseCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportDiseasesWindow = new ReportDiseasesWindow(ViewModel.DiseaseSelected.DiseaseId);
        reportDiseasesWindow.GeneratePdfDisease(ViewModel.DiseaseSelected.DiseaseId);
    });


    #endregion [Disease Methods]                    

}
