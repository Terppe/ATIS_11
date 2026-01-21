using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  Tbl90ReferenceListPage.xaml.cs Skriptdatum:  06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl90ReferenceListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl90ReferenceViewModel ViewModel { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl90ReferenceListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl90ReferenceViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor]         

    #region [Reference Methods]

    private async void GetReferencesByInfoOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl90ReferencesList?.Clear();

            ViewModel.ReferenceItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl90ReferencesList?.Clear();

                ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
                ReferenceDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingReference = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl90References
                .Where(e => e.ReferenceId == id)]
                : [.. AppData.Data.Tbl90References
                .Where(e => e.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];

            });
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;

            if (ViewModel.Tbl90ReferencesList is { Count: 0 })
            {
                ViewModel.Tbl90ReferencesList?.Clear();

                AppBarButtonDeleteReference.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelReference.Visibility = Visibility.Visible;
                ReferenceDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl90ReferenceListPage));
            }
            else
            {
                ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
                ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];
            }
            ButtonsVisible();
            ButtonsIconVisible();
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void ReferenceDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await ReferenceDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task ReferenceDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.ReferenceSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddReference_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddReference();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReference()
    {
        ViewModel.IsInEditReference = true;
        ViewModel.IsNewReference = true;

        await ViewModel.AddReferenceAsync();

        if (ViewModel.Tbl90ReferencesList != null)
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];
        }

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;

        InfoReference.PlaceholderText = StringsRes.EnterText;
    }

    private void ReferenceSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        ReferenceSearchBox.Visibility = Visibility.Visible;
        ReferenceSearchBox.Focus(FocusState.Programmatic);
        ReferenceSearchBox.PlaceholderText = StringsRes.SearchInfoOrId;
    }

    private void ReferenceDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90ReferencesList.Sort);
    }

    private async void ViewReferenceHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewReferenceHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewReferenceHyperlink()
    {
        ViewModel.IsInEditReference = false;
        ViewModel.IsNewReference = false;
        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;

        if (ReferenceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleReference = AppData.Data.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsReferenceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReference)
        {
            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelReferenceEdits();
        }
    }

    private void CancelReferenceEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];
            SingleReference = AppData.Data.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
            ReferenceSingles();
        }
        else
        {
            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditReference = false;
        ViewModel.IsNewReference = false;
    }

    private void ReferenceSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        ViewModel.ReferenceSelected.RegnumId = SingleReference.RegnumId;
        ViewModel.ReferenceSelected.PhylumId = SingleReference.PhylumId;
        ViewModel.ReferenceSelected.DivisionId = SingleReference.DivisionId;
        ViewModel.ReferenceSelected.SubphylumId = SingleReference.SubphylumId;
        ViewModel.ReferenceSelected.SubdivisionId = SingleReference.SubdivisionId;
        ViewModel.ReferenceSelected.SuperclassId = SingleReference.SuperclassId;
        ViewModel.ReferenceSelected.ClassId = SingleReference.ClassId;
        ViewModel.ReferenceSelected.SubclassId = SingleReference.SubclassId;
        ViewModel.ReferenceSelected.InfraclassId = SingleReference.InfraclassId;
        ViewModel.ReferenceSelected.LegioId = SingleReference.LegioId;
        ViewModel.ReferenceSelected.OrdoId = SingleReference.OrdoId;
        ViewModel.ReferenceSelected.SubordoId = SingleReference.SubordoId;
        ViewModel.ReferenceSelected.InfraordoId = SingleReference.InfraordoId;
        ViewModel.ReferenceSelected.SuperfamilyId = SingleReference.SuperfamilyId;
        ViewModel.ReferenceSelected.FamilyId = SingleReference.FamilyId;
        ViewModel.ReferenceSelected.SubfamilyId = SingleReference.SubfamilyId;
        ViewModel.ReferenceSelected.InfrafamilyId = SingleReference.InfrafamilyId;
        ViewModel.ReferenceSelected.SupertribusId = SingleReference.SupertribusId;
        ViewModel.ReferenceSelected.TribusId = SingleReference.TribusId;
        ViewModel.ReferenceSelected.SubtribusId = SingleReference.SubtribusId;
        ViewModel.ReferenceSelected.InfratribusId = SingleReference.InfratribusId;
        ViewModel.ReferenceSelected.GenusId = SingleReference.GenusId;
        ViewModel.ReferenceSelected.SpeciesId = SingleReference.SpeciesId;
    }

    private async Task RevertChangesReferenceAsync()
    {
        ViewModel.IsInEditReference = false;
        if (ViewModel.IsModifiedReference)
        {
            await RefreshReferenceAsync();
            ViewModel.IsModifiedReference = false;
        }
    }

    private Task RefreshReferenceAsync()
    {
        if (ReferenceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.ReferenceSelected.ReferenceId == 0)
            {
                return Task.CompletedTask;
            }
            SingleReference = AppData.Data.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
            ReferenceSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteReference_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteReference();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteReference()
    {
        var deleteDialog = new DeleteDialog
        {
            PrimaryButtonText = StringsRes.Delete,
            SecondaryButtonText = StringsRes.DoNotDelete,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await ViewModel.DeleteReferenceAsync(ViewModel.ReferenceSelected);
                await Refresh();
                ReferenceDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesReferenceAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveReference_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveReference();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveReference()
    {
        var saveDialog = new SaveChangesDialog
        {
            PrimaryButtonText = StringsRes.Save,
            SecondaryButtonText = StringsRes.DoNotSave,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Save,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                if (await BeforeSaveReference())
                {
                    return;
                }
                await ViewModel.SaveReferenceAsync(ViewModel.ReferenceSelected);
                await BehindSaveReference(ViewModel.ReferenceSelected);
                await Refresh();
                ViewModel.IsNewReference = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedReference = true;
                await RevertChangesReferenceAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveReference()
    {
        _search = ReferenceSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
            .Where(a => a.Info == ViewModel.ReferenceSelected.Info &&
                        a.RefExpertId == ViewModel.ReferenceSelected.RefExpertId &&
                        a.RefAuthorId == ViewModel.ReferenceSelected.RefAuthorId &&
                        a.RefSourceId == ViewModel.ReferenceSelected.RefSourceId &&
                        a.RegnumId == ViewModel.ReferenceSelected.RegnumId &&
                        a.PhylumId == ViewModel.ReferenceSelected.PhylumId &&
                        a.SubphylumId == ViewModel.ReferenceSelected.SubphylumId &&
                        a.SuperclassId == ViewModel.ReferenceSelected.SuperclassId &&
                        a.ClassId == ViewModel.ReferenceSelected.ClassId &&
                        a.SubclassId == ViewModel.ReferenceSelected.SubclassId &&
                        a.InfraclassId == ViewModel.ReferenceSelected.InfraclassId &&
                        a.LegioId == ViewModel.ReferenceSelected.LegioId &&
                        a.OrdoId == ViewModel.ReferenceSelected.OrdoId &&
                        a.SubordoId == ViewModel.ReferenceSelected.SubordoId &&
                        a.InfraordoId == ViewModel.ReferenceSelected.InfraordoId &&
                        a.SuperfamilyId == ViewModel.ReferenceSelected.SuperfamilyId &&
                        a.FamilyId == ViewModel.ReferenceSelected.FamilyId &&
                        a.SubfamilyId == ViewModel.ReferenceSelected.SubfamilyId &&
                        a.InfrafamilyId == ViewModel.ReferenceSelected.InfrafamilyId &&
                        a.SupertribusId == ViewModel.ReferenceSelected.SupertribusId &&
                        a.TribusId == ViewModel.ReferenceSelected.TribusId &&
                        a.SubtribusId == ViewModel.ReferenceSelected.SubtribusId &&
                        a.InfratribusId == ViewModel.ReferenceSelected.InfratribusId &&
                        a.GenusId == ViewModel.ReferenceSelected.GenusId &&
                        a.SpeciesId == ViewModel.ReferenceSelected.SpeciesId)];
        });
        if (ViewModel.IsNewReference)
        {
            if (ViewModel.Tbl90ReferencesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedReference = true;
        return false;
    }

    private async Task BehindSaveReference(Tbl90Reference viewModelReferenceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            if (ReferenceSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl90ReferencesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl90References
                    .Where(a => a.ReferenceId == id)]
                    : [.. AppData.Data.Tbl90References
                    .Where(a => _search != null && a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            }
        });

        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(a => a.ReferenceId == viewModelReferenceSelected.ReferenceId)];
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            await Refresh();
            ReferenceDataGrid.SelectedIndex = 0;
        }
        else
        {
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            var index = ViewModel.ReferenceItems.ToList().FindIndex(item => item.ReferenceId == viewModelReferenceSelected.ReferenceId);
            ReferenceDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;
    }


    private void BackReference_Click(object sender, RoutedEventArgs e)
    {
        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void BackDetailExpanders()
    {
        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Reference Methods]        

    #region [Refresh Methods]
    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Refresh();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task Refresh()
    {
        ViewModel.IsLoading = true;

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (string.IsNullOrEmpty(_search))
        {
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("ReferenceDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl90ReferencesList!.Count)
            {
                ViewModel.ReferenceItems.Clear();
                foreach (var item in ViewModel.Tbl90ReferencesList)
                {
                    ViewModel.ReferenceItems.Add(item);
                }

                ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];
            }
            ReferenceDataGrid.SelectedIndex = iNdx;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl90References
                .Where(a => a.ReferenceId == id)]
                : [.. AppData.Data.Tbl90References
                    .Where(a => a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            });
            if (ViewModel.Tbl90ReferencesList is { Count: 0 })
            {
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;

            if (ViewModel.IsDeletingReference)
            {
                if (iNdx == 0 && ViewModel.Tbl90ReferencesList.Count == 0)
                {
                    ViewModel.Tbl90ReferencesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];
            }
        }
        ReferenceDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingReference = false;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteReference.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteReference.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 

}

