
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  Tbl21ClassListPage.xaml.cs Skriptdatum:  15.06.2024  18:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl21ClassListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl21ClassViewModel ViewModel { get; }
    private Tbl21Class SingleClass { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl24Subclass SingleSubclass { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl21ClassListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl21ClassViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Class Methods]

    private async void GetClassesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl21ClassesList?.Clear();

            ViewModel.ClassItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl21ClassesList?.Clear();

                ClassDataGrid.ItemsSource = ViewModel.Tbl21ClassesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl24SubclassesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubclassesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingClass = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl21ClassesList ??= [];
                ViewModel.Tbl21ClassesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl21Classes
                .Where(a => a.ClassId == id)]
                : [.. Context.Tbl21Classes
                .Where(a => _search != null && a.ClassName.StartsWith(_search))
                .OrderBy(a => a.ClassName)];
            });

            ClassDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SubclassesExpander.Visibility = Visibility.Collapsed;
            SubclassDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.ClassDataSetCount = ViewModel.Tbl21ClassesList.Count;

            if (ViewModel.Tbl21ClassesList is { Count: 0 })
            {
                ViewModel.Tbl21ClassesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl24SubclassesList?.Clear();

                AppBarButtonDeleteClass.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddSubclass.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelClass.Visibility = Visibility.Visible;
                ClassDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubclassesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl21ClassListPage));
            }
            else
            {
                ClassDataGrid.ItemsSource = ViewModel.Tbl21ClassesList;
                ViewModel.ClassSelected = ViewModel.Tbl21ClassesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void ClassDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await ClassDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task ClassDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.ClassSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        //----------------------------------Reference--------------------------------       
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.ClassId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceDataGrid.ItemsSource = null;
        }
        else
        {
            ReferencesExpander.Visibility = Visibility.Visible;
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
        }

        //----------------------------------Comment--------------------------------
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .AsNoTracking()
            .Where(com => com.ClassId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDataGrid.ItemsSource = null;
        }
        else
        {
            CommentsExpander.Visibility = Visibility.Visible;
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
        }
        //--------------------------Subclass-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl24SubclassesList ??= [];
            ViewModel.Tbl24SubclassesList = [.. Context.Tbl24Subclasses
            .AsNoTracking()
            .Where(com => com.ClassId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.SubclassDataSetCount = ViewModel.Tbl24SubclassesList.Count;
        ViewModel.RefreshSubclassItems();

        if (ViewModel.Tbl24SubclassesList.Count == 0)
        {
            SubclassesExpander.Visibility = Visibility.Collapsed;
            SubclassDataGrid.ItemsSource = null;
        }
        else
        {
            SubclassesExpander.Visibility = Visibility.Visible;
            SubclassDataGrid.ItemsSource = ViewModel.Tbl24SubclassesList;
        }
        ViewModel.IsLoading = false;
    }

    private async void AddClass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddClass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddClass()
    {
        ClassName.Loaded += (s, e) =>
        {
            ClassName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditClass = true;
        ViewModel.IsNewClass = true;

        await ViewModel.AddClassAsync();

        if (ViewModel.Tbl21ClassesList != null)
        {
            ViewModel.ClassSelected = ViewModel.Tbl21ClassesList[0];
        }

        ClassDetailExpander.IsExpanded = true;
        ClassDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewClass = false;
        ClassName.PlaceholderText = StringsRes.EnterText;
    }

    private void ClassSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        ClassSearchBox.Visibility = Visibility.Visible;
        ClassSearchBox.Focus(FocusState.Programmatic);
        ClassSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void ClassDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl21ClassesList.Sort);
    }

    private void AddNewClassCanceled(object sender, EventArgs e) => ClassDetailExpander.IsExpanded = false;

    private async void ViewClassHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewClassHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewClassHyperlink()
    {
        ViewModel.IsInEditClass = false;
        ViewModel.IsNewClass = false;
        ClassDetailExpander.IsExpanded = true;
        ClassDetailExpander.Visibility = Visibility.Visible;

        if (ClassDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleClass = App.SingleClass(ViewModel.ClassSelected.ClassId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsClassAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewClass)
        {
            ClassDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelClassEdits();
        }
    }

    private void CancelClassEdits()
    {
        var x = (DataGrid)FindName("ClassDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditClass = false;
            ViewModel.IsNewClass = false;
            ClassDetailExpander.IsExpanded = false;
            ClassDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ClassSelected = ViewModel.Tbl21ClassesList[iNdx];

        SingleClass = App.SingleClass(ViewModel.ClassSelected.ClassId);
        ClassSingles();

        ViewModel.IsInEditClass = false;
        ViewModel.IsNewClass = false;
        ClassDetailExpander.IsExpanded = true;
        ClassDetailExpander.Visibility = Visibility.Visible;
    }

    private void ClassSingles()
    {
        ViewModel.ClassSelected.ClassName = SingleClass.ClassName;
        ViewModel.ClassSelected.SuperclassId = SingleClass.SuperclassId;
        ViewModel.ClassSelected.Valid = SingleClass.Valid;
        ViewModel.ClassSelected.ValidYear = SingleClass.ValidYear;
        ViewModel.ClassSelected.Author = SingleClass.Author;
        ViewModel.ClassSelected.AuthorYear = SingleClass.AuthorYear;
        ViewModel.ClassSelected.Info = SingleClass.Info;
        ViewModel.ClassSelected.Synonym = SingleClass.Synonym;
        ViewModel.ClassSelected.EngName = SingleClass.EngName;
        ViewModel.ClassSelected.GerName = SingleClass.GerName;
        ViewModel.ClassSelected.FraName = SingleClass.FraName;
        ViewModel.ClassSelected.PorName = SingleClass.PorName;
        ViewModel.ClassSelected.Memo = SingleClass.Memo;
    }
    private async Task RevertChangesClassAsync()
    {
        ViewModel.IsInEditClass = false;
        if (ViewModel.IsModifiedClass)
        {
            await RefreshClassAsync();
            ViewModel.IsModifiedClass = false;
        }
    }
    private Task RefreshClassAsync()
    {
        if (ClassDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleClass = App.SingleClass(ViewModel.ClassSelected.ClassId);
            ClassSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteClass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteClass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteClass()
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
                await ViewModel.DeleteClassAsync(ViewModel.ClassSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesClassAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveClass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveClass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveClass()
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
                if (await BeforeSaveClass())
                {
                    return;
                }
                await ViewModel.SaveClassAsync(ViewModel.ClassSelected);
                await BehindSaveClass();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedClass = true;
                await RevertChangesClassAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveClass()
    {
        _search = ClassSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl21ClassesList ??= [];
            ViewModel.Tbl21ClassesList = [.. Context.Tbl21Classes
                .Where(a => a.ClassName == ViewModel.ClassSelected.ClassName &&
                        a.SuperclassId == ViewModel.ClassSelected.SuperclassId)];
        });
        if (ViewModel.IsNewClass)
        {
            if (ViewModel.Tbl21ClassesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedClass = true;
        return false;
    }

    private async Task BehindSaveClass()
    {
        var x = (DataGrid)FindName("ClassDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl21ClassesList ??= [];
            ViewModel.Tbl21ClassesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl21Classes
                    .Where(a => a.ClassId == id)]
                : [.. Context.Tbl21Classes
                    .Where(a => _search != null && a.ClassName.StartsWith(_search))
                    .OrderBy(a => a.ClassName)];
        });

        ViewModel.RefreshClassItems();

        if (ViewModel.Tbl21ClassesList.Count == 0)
        {
            ClassDataGrid.ItemsSource = null;
        }
        else
        {
            ClassDataGrid.ItemsSource = ViewModel.Tbl21ClassesList;
        }
        ClassDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void ClassName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackClass_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Class Methods]                    

    #region [Reference Methods]

    private void AddNewReferenceCanceled(object sender, EventArgs e) => ReferencesExpander.IsExpanded = false;

    private void DataGrid_Reference_Sorting(object sender, DataGridColumnEventArgs e)
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
        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReference = false;
        ViewModel.IsNewReferenceExpert = false;
        ViewModel.IsNewReferenceSource = false;
        ViewModel.IsNewReferenceAuthor = false;

        if (ViewModel.ReferenceSelected != null)
        {
            if (ViewModel.ReferenceSelected.RefExpertId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceExpertDetailExpander.IsExpanded = true;
                ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 0;
                TabViewItemReferenceExpert.Visibility = Visibility.Visible;
                TabViewItemReferenceSource.IsEnabled = false;
                TabViewItemReferenceAuthor.IsEnabled = false;
                return Task.CompletedTask;
            }
            if (ViewModel.ReferenceSelected.RefSourceId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceSourceDetailExpander.IsExpanded = true;
                ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 1;
                TabViewItemReferenceSource.Visibility = Visibility.Visible;
                TabViewItemReferenceExpert.IsEnabled = false;
                TabViewItemReferenceAuthor.IsEnabled = false;
                return Task.CompletedTask;
            }
            if (ViewModel.ReferenceSelected.RefAuthorId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceAuthorDetailExpander.IsExpanded = true;
                ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 2;
                TabViewItemReferenceAuthor.Visibility = Visibility.Visible;
                TabViewItemReferenceExpert.IsEnabled = false;
                TabViewItemReferenceSource.IsEnabled = false;
            }
        }

        if (ReferenceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
        }
        else
        {
            if (ViewModel.ReferenceSelected != null)
            {
                SingleReference = App.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
            }
        }

        return Task.CompletedTask;
    }

    private void ReferenceSingles()
    {
        ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
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
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
        }
        else
        {
            SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);
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
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(a => a.Info == ViewModel.ReferenceSelected.Info &&
                        a.ClassId == ViewModel.ReferenceSelected.ClassId &&
                        a.RefExpertId == ViewModel.ReferenceSelected.RefExpertId &&
                        a.RefSourceId == ViewModel.ReferenceSelected.RefSourceId &&
                        a.RefAuthorId == ViewModel.ReferenceSelected.RefAuthorId)];
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
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(refe => refe.FamilyId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceDataGrid.ItemsSource = null;
        }
        else
        {
            ReferencesExpander.Visibility = Visibility.Visible;
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
        }
        ReferenceDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackReference_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Reference Methods]   

    #region [Reference Expert Methods]

    private async void AddReferenceExpertButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceExpert();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceExpert()
    {
        InfoReferenceExpert.Loaded += (s, e) =>
        {
            InfoReferenceExpert.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditReferenceExpert = true;
        ViewModel.IsNewReferenceExpert = true;

        ReferenceExpertCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceExpertAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 0;
        TabViewItemReferenceExpert.Visibility = Visibility.Visible;
        TabViewItemReferenceSource.IsEnabled = false;
        TabViewItemReferenceAuthor.IsEnabled = false;
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceExpertAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceExpert)
        {
            ReferenceExpertDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelReferenceExpertEdits();
        }
    }

    private void CancelReferenceExpertEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceExpert = false;
            ViewModel.IsNewReferenceExpert = false;
            ReferenceExpertDetailExpander.IsExpanded = false;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

        ReferenceExpertSingles();

        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsNewReferenceExpert = false;
        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
    }

    private void ReferenceExpertSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        //ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        //ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceExpertAsync()
    {
        ViewModel.IsInEditReferenceExpert = false;
        if (ViewModel.IsModifiedReferenceExpert)
        {
            await RefreshReferenceExpertAsync();
            ViewModel.IsModifiedReferenceExpert = false;
        }
    }
    private async Task RefreshReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

            ReferenceExpertSingles();
        });
    }
    #endregion [Reference Expert Methods]

    #region [Reference Source Methods]

    private async void AddReferenceSourceButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceSource();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceSource()
    {
        InfoReferenceSource.Loaded += (s, e) =>
        {
            InfoReferenceSource.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditReferenceSource = true;
        ViewModel.IsNewReferenceSource = true;

        ReferenceSourceCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceSourceAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 1;
        TabViewItemReferenceSource.Visibility = Visibility.Visible;
        TabViewItemReferenceExpert.IsEnabled = false;
        TabViewItemReferenceAuthor.IsEnabled = false;
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceSourceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceSource)
        {
            ReferenceSourceDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelReferenceSourceEdits();
        }
    }
    private void CancelReferenceSourceEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceSource = false;
            ViewModel.IsNewReferenceSource = false;
            ReferenceSourceDetailExpander.IsExpanded = false;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

        ReferenceSourceSingles();

        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsNewReferenceSource = false;
        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
    }
    private void ReferenceSourceSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        // ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        //ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceSourceAsync()
    {
        ViewModel.IsInEditReferenceSource = false;
        if (ViewModel.IsModifiedReferenceSource)
        {
            await RefreshReferenceSourceAsync();
            ViewModel.IsModifiedReferenceSource = false;
        }
    }
    private async Task RefreshReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

            ReferenceSourceSingles();
        });
    }

    #endregion [Reference Source Methods]

    #region [Reference Author Methods]

    private async void AddReferenceAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceAuthor();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceAuthor()
    {
        InfoReferenceAuthor.Loaded += (s, e) =>
        {
            InfoReferenceAuthor.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditReferenceAuthor = true;
        ViewModel.IsNewReferenceAuthor = true;

        ReferenceAuthorCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceAuthorAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 2;
        TabViewItemReferenceAuthor.Visibility = Visibility.Visible;
        TabViewItemReferenceExpert.IsEnabled = false;
        TabViewItemReferenceSource.IsEnabled = false;
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceAuthorAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceAuthor)
        {
            ReferenceAuthorDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelReferenceAuthorEdits();
        }
    }

    private void CancelReferenceAuthorEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceAuthor = false;
            ViewModel.IsNewReferenceAuthor = false;
            ReferenceAuthorDetailExpander.IsExpanded = false;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

        ReferenceAuthorSingles();

        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReferenceAuthor = false;
        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
    }
    private void ReferenceAuthorSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        // ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        // ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceAuthorAsync()
    {
        ViewModel.IsInEditReferenceAuthor = false;
        if (ViewModel.IsModifiedReferenceAuthor)
        {
            await RefreshReferenceAuthorAsync();
            ViewModel.IsModifiedReferenceAuthor = false;
        }
    }
    private async Task RefreshReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceClass(ViewModel.ReferenceSelected.ReferenceId, ViewModel.ClassSelected.ClassId);

            ReferenceAuthorSingles();
        });
    }

    #endregion [Reference Author Methods]   

    #region [Comment Methods]
    private async void AddComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddComment()
    {
        InfoComment.Loaded += (s, e) =>
        {
            InfoComment.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditComment = true;
        ViewModel.IsNewComment = true;

        await ViewModel.AddComment();

        if (ViewModel.Tbl93CommentsList != null)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
        }

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewComment = false;
    }

    private void DataGrid_Comment_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl93CommentsList.Sort);
    }

    private void AddNewCommentCanceled(object sender, EventArgs e) => CommentsExpander.IsExpanded = false;

    private async void ViewCommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewCommentHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewCommentHyperlink()
    {
        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
        if (CommentDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleComment = App.SingleCommentClass(ViewModel.CommentSelected.CommentId, ViewModel.ClassSelected.ClassId);
        }
        return Task.CompletedTask;
    }

    private void CancelEditsCommentAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewComment)
        {
            CommentDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelCommentEdits();
        }
    }

    private void CancelCommentEdits()
    {
        var x = (DataGrid)FindName("CommentDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditComment = false;
            ViewModel.IsNewComment = false;
            CommentDetailExpander.IsExpanded = false;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];

        SingleComment = App.SingleCommentClass(ViewModel.CommentSelected.CommentId, ViewModel.ClassSelected.ClassId);
        CommentSingles();

        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
    }

    private void CommentSingles()
    {
        ViewModel.CommentSelected.Valid = SingleComment.Valid;
        ViewModel.CommentSelected.ValidYear = SingleComment.ValidYear;
        ViewModel.CommentSelected.Info = SingleComment.Info;
        ViewModel.CommentSelected.Memo = SingleComment.Memo;
    }

    private async Task RevertChangesCommentAsync()
    {
        ViewModel.IsInEditComment = false;
        if (ViewModel.IsModifiedComment)
        {
            await RefreshCommentAsync();
            ViewModel.IsModifiedComment = false;
        }
    }

    private async Task RefreshCommentAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {

            if (CommentDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
            }
            else
            {
                SingleComment = App.SingleCommentClass(ViewModel.CommentSelected.CommentId, ViewModel.ClassSelected.ClassId);
                CommentSingles();
            }
        });
    }

    private async void DeleteComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteComment()
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
                await ViewModel.DeleteCommentAsync(ViewModel.CommentSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesCommentAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveComment()
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
                if (await BeforeSaveComment())
                {
                    return;
                }
                await ViewModel.SaveCommentAsync(ViewModel.CommentSelected);
                await BehindSaveComment();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedComment = true;
                await RevertChangesCommentAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveComment()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .Where(a => a.Info == ViewModel.CommentSelected.Info &&
                        a.Memo == ViewModel.CommentSelected.Memo &&
                        a.ClassId == ViewModel.CommentSelected.ClassId)];
        });
        if (ViewModel.IsNewComment)
        {
            if (ViewModel.Tbl93CommentsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedComment = true;
        return false;
    }

    private async Task BehindSaveComment()
    {
        var x = (DataGrid)FindName("CommentDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .AsNoTracking()
                .Where(com => com.ClassId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDataGrid.ItemsSource = null;
        }
        else
        {
            CommentsExpander.Visibility = Visibility.Visible;
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
        }
        CommentDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Comment Methods]  

    #region [Subclass Methods]

    private async void AddSubclass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubclass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSubclass()
    {
        SubclassName.Loaded += (s, e) =>
        {
            SubclassName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSubclass = true;
        ViewModel.IsNewSubclass = true;

        await ViewModel.AddSubclassAsync();

        if (ViewModel.Tbl24SubclassesList != null)
        {
            ViewModel.SubclassSelected = ViewModel.Tbl24SubclassesList[0];
        }

        SubclassDetailExpander.IsExpanded = true;
        SubclassDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewSubclass = false;
    }

    private void DataGrid_Subclass_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl24SubclassesList.Sort);
    }

    private void AddNewSubclassCanceled(object sender, EventArgs e) => SubclassesExpander.IsExpanded = false;

    private async void ViewSubclassHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubclassHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubclassHyperlink()
    {
        ViewModel.IsInEditSubclass = false;
        ViewModel.IsNewSubclass = false;
        SubclassDetailExpander.IsExpanded = true;
        SubclassDetailExpander.Visibility = Visibility.Visible;

        if (SubclassDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubclass = App.SingleSubclassClass(ViewModel.SubclassSelected.SubclassId,
                    ViewModel.ClassSelected.ClassId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSubclassAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubclass)
        {
            SubclassDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelSubclassEdits();
        }
    }

    private void CancelSubclassEdits()
    {
        var x = (DataGrid)FindName("SubclassDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditSubclass = false;
            ViewModel.IsNewSubclass = false;
            SubclassDetailExpander.IsExpanded = false;
            SubclassDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }
        ViewModel.SubclassSelected = ViewModel.Tbl24SubclassesList[iNdx];

        SingleSubclass = App.SingleSubclassClass(ViewModel.SubclassSelected.SubclassId, ViewModel.ClassSelected.ClassId);
        SubclassSingles();

        ViewModel.IsInEditSubclass = false;
        ViewModel.IsNewSubclass = false;
        SubclassDetailExpander.IsExpanded = true;
        SubclassDetailExpander.Visibility = Visibility.Visible;
    }

    private void SubclassSingles()
    {
        ViewModel.SubclassSelected.SubclassName = SingleSubclass.SubclassName;
        ViewModel.SubclassSelected.ClassId = SingleSubclass.ClassId;
        ViewModel.SubclassSelected.Valid = SingleSubclass.Valid;
        ViewModel.SubclassSelected.ValidYear = SingleSubclass.ValidYear;
        ViewModel.SubclassSelected.Author = SingleSubclass.Author;
        ViewModel.SubclassSelected.AuthorYear = SingleSubclass.AuthorYear;
        ViewModel.SubclassSelected.Info = SingleSubclass.Info;
        ViewModel.SubclassSelected.Synonym = SingleSubclass.Synonym;
        ViewModel.SubclassSelected.EngName = SingleSubclass.EngName;
        ViewModel.SubclassSelected.GerName = SingleSubclass.GerName;
        ViewModel.SubclassSelected.FraName = SingleSubclass.FraName;
        ViewModel.SubclassSelected.PorName = SingleSubclass.PorName;
        ViewModel.SubclassSelected.Memo = SingleSubclass.Memo;
    }

    private async Task RevertChangesSubclassAsync()
    {
        ViewModel.IsInEditSubclass = false;
        if (ViewModel.IsModifiedSubclass)
        {
            await RefreshSubclassAsync();
            ViewModel.IsModifiedSubclass = false;
        }
    }
    private async Task RefreshSubclassAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SubclassDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSubclass = App.SingleSubclassClass(ViewModel.SubclassSelected.SubclassId, ViewModel.ClassSelected.ClassId);
                SubclassSingles();
            }
        });
    }

    private async void SaveSubclass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubclass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubclass()
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
                if (await BeforeSaveSubclass())
                {
                    return;
                }
                await ViewModel.SaveSubclassAsync(ViewModel.SubclassSelected);
                await BehindSaveSubclass();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubclass = true;
                await RevertChangesSubclassAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubclass()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl24SubclassesList ??= [];
            ViewModel.Tbl24SubclassesList = [.. Context.Tbl24Subclasses
            .Where(a => a.SubclassName == ViewModel.SubclassSelected.SubclassName &&
                        a.ClassId == ViewModel.SubclassSelected.ClassId)];
        });
        if (ViewModel.IsNewSubclass)
        {
            if (ViewModel.Tbl24SubclassesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubclass = true;
        return false;
    }

    private async Task BehindSaveSubclass()
    {
        var x = (DataGrid)FindName("SubclassDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl24SubclassesList ??= [];
            ViewModel.Tbl24SubclassesList = [.. Context.Tbl24Subclasses
                .AsNoTracking()
                .Where(com => com.ClassId == ViewModel.ClassSelected.ClassId)];
        });
        ViewModel.RefreshSubclassItems();

        if (ViewModel.Tbl24SubclassesList.Count == 0)
        {
            SubclassesExpander.Visibility = Visibility.Collapsed;
            SubclassDataGrid.ItemsSource = null;
        }
        else
        {
            SubclassesExpander.Visibility = Visibility.Visible;
            SubclassDataGrid.ItemsSource = ViewModel.Tbl24SubclassesList;
        }
        SubclassDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void SubclassName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubclass_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subclass Methods] 

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
            //  DetailPanelClass.Visibility = Visibility.Visible;
            //  ClassDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("ClassDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl21ClassesList!.Count)
            {
                ViewModel.ClassItems.Clear();
                foreach (var item in ViewModel.Tbl21ClassesList)
                {
                    ViewModel.ClassItems.Add(item);
                }

                ViewModel.ClassSelected = ViewModel.Tbl21ClassesList[iNdx];
            }
            ClassDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl21ClassesList.Count == 0)
                {
                    ViewModel.Tbl21ClassesList ??= [];
                    ViewModel.Tbl21ClassesList = [.. Context.Tbl21Classes
                            .Where(a => a.ClassId == ViewModel.ClassSelected.ClassId)];
                }
                else
                {
                    ViewModel.Tbl21ClassesList ??= [];
                    ViewModel.Tbl21ClassesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl21Classes
                           .Where(a => a.ClassId == id)]
                       : [.. Context.Tbl21Classes
                           .Where(a => _search != null && a.ClassName.StartsWith(_search))
                         .OrderBy(a => a.ClassName)];
                }
            });

            ClassDataGrid.ItemsSource = ViewModel.Tbl21ClassesList;

            if (ViewModel.IsDeletingClass)
            {
                if (iNdx == 0 && ViewModel.Tbl21ClassesList.Count == 0)
                {
                    ViewModel.Tbl21ClassesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.ClassSelected = ViewModel.Tbl21ClassesList[iNdx];
            }
            if (ViewModel.Tbl21ClassesList is { Count: 0 })
            {
                ViewModel.Tbl21ClassesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl24SubclassesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        ClassDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingClass = false;
    }

    private void BackDetailExpanders()
    {
        ClassDetailExpander.IsExpanded = false;
        ClassDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SubclassDetailExpander.IsExpanded = false;
        SubclassDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

