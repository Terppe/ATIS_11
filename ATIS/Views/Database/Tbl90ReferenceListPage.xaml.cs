
using ATIS.Helpers;
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
    public Tbl90ReferenceViewModel ViewModel { get; }
    private Tbl90Reference SingleReference { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl90ReferenceListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl90ReferenceViewModel>();
            InitializeComponent();
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
                ? [.. Context.Tbl90References
                .Where(e => e.ReferenceId == id)]
                : [.. Context.Tbl90References
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

        ViewModel.IsNewReference = false;
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

    private void AddNewReferenceCanceled(object sender, EventArgs e) => ReferenceDetailExpander.IsExpanded = false;

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
            SingleReference = App.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsReferenceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReference)
        {
            ReferenceDetailExpander.IsExpanded = false;
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
        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
        ReferenceSingles();

        ViewModel.IsInEditReference = false;
        ViewModel.IsNewReference = false;
        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
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
        ViewModel.ReferenceSelected.FiSpeciesId = SingleReference.FiSpeciesId;
        ViewModel.ReferenceSelected.PlSpeciesId = SingleReference.PlSpeciesId;
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
            SingleReference = App.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
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
                await BehindSaveReference();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedReference = true;
                await RevertChangesReferenceAsync();
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
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(a => a.Info == ViewModel.ReferenceSelected.Info &&
                        a.RefExpertId == ViewModel.ReferenceSelected.RefExpertId &&
                        a.RefAuthorId == ViewModel.ReferenceSelected.RefAuthorId &&
                        a.RefSourceId == ViewModel.ReferenceSelected.RefSourceId &&
                        a.RegnumId == ViewModel.ReferenceSelected.RegnumId &&
                        a.PhylumId == ViewModel.ReferenceSelected.PhylumId &&
                        a.DivisionId == ViewModel.ReferenceSelected.DivisionId &&
                        a.SubphylumId == ViewModel.ReferenceSelected.SubphylumId &&
                        a.SubdivisionId == ViewModel.ReferenceSelected.SubdivisionId &&
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
                        a.PlSpeciesId == ViewModel.ReferenceSelected.PlSpeciesId &&
                        a.FiSpeciesId == ViewModel.ReferenceSelected.FiSpeciesId)];
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

    private async Task BehindSaveReference()
    {
        if (string.IsNullOrEmpty(ReferenceSearchBox.Text))
        {
            var x = (DataGrid)FindName("ReferenceDataGrid");
            var iNdx = x.SelectedIndex;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                    .Where(a => a.ReferenceId == ViewModel.ReferenceSelected.ReferenceId)];
            });
            ViewModel.RefreshReferenceItems();

            if (ViewModel.Tbl90ReferencesList.Count == 0)
            {
                ReferenceDetailExpander.Visibility = Visibility.Collapsed;
                ReferenceDataGrid.ItemsSource = null;
            }
            else
            {
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            }

            ReferenceDataGrid.SelectedIndex = iNdx;

            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
        }
    }

    private void BackReference_Click(object sender, RoutedEventArgs e)
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
            // DetailPanelReference.Visibility = Visibility.Visible;
            //  ReferenceDetailExpander.Visibility = Visibility.Visible;
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
                ? [.. Context.Tbl90References
                .Where(a => a.ReferenceId == id)]
                : [.. Context.Tbl90References
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

    #endregion [Refresh Methods] 


}

