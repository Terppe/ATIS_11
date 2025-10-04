using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Main.MiscellaneousPdf;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MiscellaneousPage : Page
{
    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public MiscellaneousViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public MiscellaneousPage()
    {
        ViewModel = App.GetService<MiscellaneousViewModel>();
        InitializeComponent();
        SimpleLog.Info(" MiscellaneousPage opended ");
    }
    #endregion [Constructor]         

    #region [Disease Methods]

    private async void GetMiscellaneoussesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblMiscellaneoussesList?.Clear();

            ViewModel.MiscellaneousItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblMiscellaneoussesList?.Clear();

                MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
                //     ListViewMiscellaneous.ItemsSource = ViewModel.TblMiscellaneoussList;
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblMiscellaneoussesList ??= [];
                ViewModel.TblMiscellaneoussesList = int.TryParse(_search, out var id)
                ? [.. Context.TblMiscellaneousses
                .Where(e => e.MiscellaneousId == id)]
                : [.. Context.TblMiscellaneousses
                .Where(e => e.MiscellaneousName.StartsWith(_search))
                .OrderBy(a => a.MiscellaneousName)];

            });

            ViewModel.MiscellaneousDataSetCount = ViewModel.TblMiscellaneoussesList.Count;

            if (ViewModel.TblMiscellaneoussesList is { Count: 0 })
            {
                ViewModel.TblMiscellaneoussesList?.Clear();

                //    AppBarButtonSync.IsEnabled = false;


                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(MiscellaneousPage));
            }
            else
            {
                MiscellaneousDataGrid.ItemsSource = ViewModel.TblMiscellaneoussesList;
                //       ListViewMiscellaneous.ItemsSource = ViewModel.TblMiscellaneoussList;
                ViewModel.MiscellaneousSelected = ViewModel.TblMiscellaneoussesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private void MiscellaneousSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        MiscellaneousSearchBox.Visibility = Visibility.Visible;
        MiscellaneousSearchBox.Focus(FocusState.Programmatic);
        MiscellaneousSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void MiscellaneousDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblMiscellaneoussesList.Sort);
    }


    private void MiscellaneousSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.MiscellaneousSelected.MiscellaneousId == 0)
        {
            _ = AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        var window = new ReportMiscellaneoussesWindow(ViewModel.MiscellaneousSelected.MiscellaneousId)
        {
            IsAlwaysOnTop = true
        };
        window.Activate();
    }
    public ICommand PdfMiscellaneousCommand => new WinUICommunity.RelayCommand(execute: delegate
    {
        var reportMiscellaneoussesWindow = new ReportMiscellaneoussesWindow(ViewModel.MiscellaneousSelected.MiscellaneousId);
        reportMiscellaneoussesWindow.GeneratePdfMiscellaneous(ViewModel.MiscellaneousSelected.MiscellaneousId);
    });


    #endregion [Disease Methods]                    
}
