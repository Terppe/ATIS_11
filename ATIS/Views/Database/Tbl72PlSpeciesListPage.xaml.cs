using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Database;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl72PlSpeciesListPage : Page
{
    #region [Private Data Members]

    public Tbl72PlSpeciesViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();
    private string _search;
    private int _fishId;
    private readonly FileOpenPicker _filePicker = new();

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl72PlSpeciesListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl72PlSpeciesViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor]

    #region [PlSpecies Methods]
    private async void GetPlSpeciesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        _search = args.QueryText;
        ViewModel.IsLoading = true;

        ViewModel.Tbl72PlSpeciessesList?.Clear();
        ViewModel.PlSpeciesItems.Clear();

        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        ViewModel.Tbl72PlSpeciessesList ??= [];
        //ViewModel.Tbl72PlSpeciessesList = int.TryParse(_search, out var id)
        //    ? new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
        //        .Where(e => e.PlSpeciesId == id))
        //    : new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
        //        .Where(e => _search != null && e.PlSpeciesName.StartsWith(_search))
        //        .OrderBy(a => a.Tbl66Genusses.GenusName)
        //        .ThenBy(a => a.PlSpeciesName)
        //        .ThenBy(a => a.Subspecies)
        //        .ThenBy(a => a.Divers));

        //search about GenusName
        ViewModel.Tbl72PlSpeciessesList = int.TryParse(_search, out var id)
            ? [.. Context.Tbl72PlSpeciesses
                .Where(e => e.PlSpeciesId == id)]
            : [.. Context.Tbl72PlSpeciesses
                .Where(e => e.Tbl66Genusses.GenusName.StartsWith(_search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)];

        if (ViewModel.Tbl72PlSpeciessesList is { Count: 0 })
        {
            ViewModel.Tbl66GenussesList?.Clear();
            ViewModel.Tbl72PlSpeciessesList?.Clear();
            ViewModel.Tbl90ReferencesList?.Clear();
            ViewModel.Tbl93CommentsList?.Clear();
            ViewModel.Tbl78NamesList?.Clear();
            ViewModel.Tbl81ImagesList?.Clear();
            ViewModel.Tbl84SynonymsList?.Clear();
            ViewModel.Tbl87GeographicsList?.Clear();

            ReferencesExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            NamesExpander.Visibility = Visibility.Collapsed;
            ImagesExpander.Visibility = Visibility.Collapsed;
            SynonymsExpander.Visibility = Visibility.Collapsed;
            GeographicsExpander.Visibility = Visibility.Collapsed;

            ViewModel.IsLoading = false;
            await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            return;
        }

        PlSpeciesDataGrid.ItemsSource = ViewModel.Tbl72PlSpeciessesList;
        ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[0];
        ViewModel.IsLoading = false;
    }

    private async void PlSpeciesDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        ViewModel.IsLoading = true;

        if (ViewModel.PlSpeciesSelected == null)
        {
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        //----------------------------------Reference--------------------------------
        ViewModel.Tbl90ReferencesList ??= [];
        ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

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
        ViewModel.Tbl93CommentsList ??= [];
        ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

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
        //--------------------------Name-----------------------------          
        ViewModel.Tbl78NamesList ??= [];
        ViewModel.Tbl78NamesList = [.. Context.Tbl78Names
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshNameItems();

        if (ViewModel.Tbl78NamesList.Count == 0)
        {
            NamesExpander.Visibility = Visibility.Collapsed;
            NameDataGrid.ItemsSource = null;
        }
        else
        {
            NamesExpander.Visibility = Visibility.Visible;
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
        }
        //--------------------------Image-----------------------------          
        ViewModel.Tbl81ImagesList ??= [];
        ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDataGrid.ItemsSource = null;
        }
        else
        {
            ImagesExpander.Visibility = Visibility.Visible;
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
        }

        //--------------------------Synonym-----------------------------          
        ViewModel.Tbl84SynonymsList ??= [];
        ViewModel.Tbl84SynonymsList = [.. Context.Tbl84Synonyms
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshSynonymItems();

        if (ViewModel.Tbl84SynonymsList.Count == 0)
        {
            SynonymsExpander.Visibility = Visibility.Collapsed;
            SynonymDataGrid.ItemsSource = null;
        }
        else
        {
            SynonymsExpander.Visibility = Visibility.Visible;
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
        }
        //--------------------------Geographic-----------------------------          
        ViewModel.Tbl87GeographicsList ??= [];
        ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshGeographicItems();

        if (ViewModel.Tbl87GeographicsList.Count == 0)
        {
            GeographicsExpander.Visibility = Visibility.Collapsed;
            GeographicDataGrid.ItemsSource = null;
        }
        else
        {
            GeographicsExpander.Visibility = Visibility.Visible;
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
        }

        ViewModel.IsLoading = false;
    }

    private async void AddPlSpecies_Click(object sender, RoutedEventArgs e)
    {
        var genusId = 0;
        var speciesgroupId = 0;

        ViewModel.IsInEdit = true;
        ViewModel.IsNewPlSpecies = true;

        //Id search for first Dataset of Tbl66GenussList
        var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(a => a.GenusName));

        genusId = collection.First().GenusId;

        //Id search for first Dataset of Tbl68SpeciesgroupsList
        var collection1 = new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
            .Where(a => a.SpeciesgroupName == "NotClear"));

        if (collection1.Count == 0)
        {
            await AllDialogs.ErrorMessageDialogAsync(StringsRes.DatasetNotExist + "Speciesgroup - NotClear");
        }
        else
        {
            speciesgroupId = collection1.First().SpeciesgroupId;
        }

        ViewModel.Tbl72PlSpeciessesList.Insert(index: 0, item: new Tbl72PlSpecies { PlSpeciesName = StringsRes.DatasetNew, GenusId = genusId, SpeciesgroupId = speciesgroupId });

        ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[0];

        PlSpeciesDetailExpander.IsExpanded = true;
        PlSpeciesDetailExpander.Visibility = Visibility.Visible;
    }

    private void PlSpeciesSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        PlSpeciesSearchBox.Visibility = Visibility.Visible;
        PlSpeciesSearchBox.Focus(FocusState.Programmatic);
        PlSpeciesSearchBox.PlaceholderText = StringsRes.SearchGenusnameOrSpeciesId;
    }

    private void PlSpeciesDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl72PlSpeciessesList.Sort);
    }

    private void AddNewPlSpeciesCanceled(object sender, EventArgs e) => PlSpeciesDetailExpander.IsExpanded = false;
    private void ViewPlSpeciesHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        PlSpeciesDetailExpander.IsExpanded = true;
        PlSpeciesDetailExpander.Visibility = Visibility.Visible;
    }

    private async void DeletePlSpecies_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeletePlSpecies(ViewModel.PlSpeciesSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }

    private async Task DeletePlSpecies(Tbl72PlSpecies plspecies)
    {
        Tbl72PlSpecies result = null;
        try
        {
            var deletePlSpeciesId = ViewModel.PlSpeciesSelected.PlSpeciesId;
            await ViewModel.DeletePlSpeciesAsync(deletePlSpeciesId);

            result = plspecies;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingPlSpecies = true;
            ViewModel.IsDeleting = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async void SavePlSpecies_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SavePlSpecies(ViewModel.PlSpeciesSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }

    private async Task SavePlSpecies(Tbl72PlSpecies plspecies)
    {
        if (ViewModel.IsNewPlSpecies)
        {
            ViewModel.IsNewPlSpecies = false;
        }

        Tbl72PlSpecies result = null;
        try
        {
            var current = await Context.Tbl72PlSpeciesses.FirstOrDefaultAsync(a => a.PlSpeciesId == plspecies.PlSpeciesId);
            if (null == current)
            {
                plspecies.CountId = RandomHelper.Randomnumber();
                plspecies.Writer = Environment.UserName;
                plspecies.WriterDate = DateTime.Now;
                plspecies.Updater = Environment.UserName;
                plspecies.UpdaterDate = DateTime.Now;
                Context.Tbl72PlSpeciesses.Add(plspecies);
            }
            else
            {
                plspecies.Updater = Environment.UserName;
                plspecies.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(plspecies);
            }
            await Context.SaveChangesAsync();

            result = plspecies;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingPlSpecies = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    private void BackPlSpecies_Click(object sender, RoutedEventArgs e)
    {
        Refresh_Click(null, null);
        PlSpeciesDetailExpander.IsExpanded = false;
        PlSpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [PlSpecies Methods] 

    #region [Reference Methods]

    private void AddNewReferenceCanceled(object sender, EventArgs e) => ReferencesExpander.IsExpanded = false;

    private void DataGrid_Reference_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90ReferencesList.Sort);
    }

    private void ViewReferenceHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        var referenceId = ((sender as FrameworkElement).DataContext as Tbl90Reference).ReferenceId;

        ViewModel.ReferenceSelected = new Tbl90Reference();
        ViewModel.ReferenceSelected = Context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceId);

        if (ViewModel.ReferenceSelected != null)
        {
            if (ViewModel.ReferenceSelected.RefExpertId != null)
            {
                ReferenceExpertDetailExpander.IsExpanded = true;
                ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
                return;
            }
            if (ViewModel.ReferenceSelected.RefSourceId != null)
            {
                ReferenceSourceDetailExpander.IsExpanded = true;
                ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
                return;
            }
            if (ViewModel.ReferenceSelected.RefAuthorId != null)
            {
                ReferenceAuthorDetailExpander.IsExpanded = true;
                ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
            }
        }
    }

    private async void DeleteReference_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteReference(ViewModel.ReferenceSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }

    private async Task DeleteReference(Tbl90Reference referenceSelected)
    {
        if (referenceSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl90Reference result = null;
        try
        {
            var deletereferenceId = referenceSelected.ReferenceId;
            await ViewModel.DeleteReferenceAsync(deletereferenceId);



            result = referenceSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingReference = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async void SaveReference_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveReference(ViewModel.ReferenceSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }
    private async Task SaveReference(Tbl90Reference referenceSelected)
    {
        if (ViewModel.IsNewReference)
        {
            ViewModel.IsNewReference = false;
        }

        Tbl90Reference result = null;
        try
        {
            var current = await Context.Tbl90References.FirstOrDefaultAsync(_ref => _ref.ReferenceId == referenceSelected.ReferenceId);
            if (null == current)
            {
                referenceSelected.CountId = RandomHelper.Randomnumber();
                referenceSelected.Writer = Environment.UserName;
                referenceSelected.WriterDate = DateTime.Now;
                referenceSelected.Updater = Environment.UserName;
                referenceSelected.UpdaterDate = DateTime.Now;
                Context.Tbl90References.Add(referenceSelected);
            }
            else
            {
                referenceSelected.Updater = Environment.UserName;
                referenceSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(referenceSelected);
            }
            await Context.SaveChangesAsync();
            result = referenceSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingReference = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    private void BackReference_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl90ReferencesList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl90ReferencesList.IndexOf(ViewModel.Tbl90ReferencesList.FirstOrDefault(t =>
            t.ReferenceId == ViewModel.ReferenceSelected.ReferenceId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl90ReferencesList!.Count)
        {
            ViewModel.ReferenceItems.Clear();
            foreach (var item in ViewModel.Tbl90ReferencesList)
            {
                ViewModel.ReferenceItems.Add(item);
            }

            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];
        }
        ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
        ReferenceDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl90ReferencesList ??= [];
        ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .AsNoTracking()
            .Where(re => re.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

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
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];
            }
            else
            {
                ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];
        }

        ViewModel.IsLoading = false;
        ReferenceExpertDetailExpander.IsExpanded = false;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.IsExpanded = false;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.IsExpanded = false;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Reference Methods]

    #region [Reference Expert Methods]

    private void AddReferenceExpertButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsInEdit = true;
        ViewModel.IsNewReference = true;

        ViewModel.Tbl90ExpertsAllList ??= [];
        ViewModel.Tbl90ExpertsAllList = [.. Context.Tbl90RefExperts
            .OrderBy(k => k.RefExpertName)];

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl90ReferencesList ??= [];
        ViewModel.Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { Info = StringsRes.DatasetNew, FiSpeciesId = id });


        ViewModel.ReferenceItems.Clear();
        foreach (var item in ViewModel.Tbl90ReferencesList)
        {
            ViewModel.ReferenceItems.Add(item);
        }
        ViewModel.ReferenceSelected = ViewModel.ReferenceItems.First();

        ReferenceExpertCombo.PlaceholderText = StringsRes.RequiredComboSelect;

        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
    }

    #endregion [Reference Expert Methods]

    #region [Reference Source Methods]

    private void AddReferenceSourceButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsInEdit = true;
        ViewModel.IsNewReference = true;

        ViewModel.Tbl90SourcesAllList ??= [];
        ViewModel.Tbl90SourcesAllList = [.. Context.Tbl90RefSources
            .OrderBy(a => a.RefSourceName)];

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl90ReferencesList ??= [];
        ViewModel.Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { Info = StringsRes.DatasetNew, FiSpeciesId = id });


        ViewModel.ReferenceItems.Clear();
        foreach (var item in ViewModel.Tbl90ReferencesList)
        {
            ViewModel.ReferenceItems.Add(item);
        }
        ViewModel.ReferenceSelected = ViewModel.ReferenceItems.First();

        ReferenceSourceCombo.PlaceholderText = StringsRes.RequiredComboSelect;

        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
    }
    #endregion [Reference Source Methods]

    #region [Reference Author Methods]

    private void AddReferenceAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsInEdit = true;
        ViewModel.IsNewReference = true;

        ViewModel.Tbl90AuthorsAllList ??= [];
        ViewModel.Tbl90AuthorsAllList = [.. Context.Tbl90RefAuthors
            .OrderBy(k => k.RefAuthorName)
            .ThenBy(a => a.BookName)
            .ThenBy(a => a.Page1)];

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl90ReferencesList ??= [];
        ViewModel.Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { Info = StringsRes.DatasetNew, FiSpeciesId = id });


        ViewModel.ReferenceItems.Clear();
        foreach (var item in ViewModel.Tbl90ReferencesList)
        {
            ViewModel.ReferenceItems.Add(item);
        }
        ViewModel.ReferenceSelected = ViewModel.ReferenceItems.First();

        ReferenceAuthorCombo.PlaceholderText = StringsRes.RequiredComboSelect;

        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
    }
    #endregion [Reference Author Methods]

    #region [Comment Methods]
    private void AddComment_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsInEdit = true;
        ViewModel.IsNewComment = true;

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl93CommentsList ??= [];
        ViewModel.Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment { Info = StringsRes.DatasetNew, FiSpeciesId = id });
        ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
    }

    private void DataGrid_Comment_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl93CommentsList.Sort);
    }

    private void AddNewCommentCanceled(object sender, EventArgs e) => CommentsExpander.IsExpanded = false;
    private void ViewCommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
    }
    private async void DeleteComment_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteComment(ViewModel.CommentSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }
    private async Task DeleteComment(Tbl93Comment commentSelected)
    {
        if (commentSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl93Comment result = null;
        try
        {
            var deletecommentId = commentSelected.CommentId;
            await ViewModel.DeleteCommentAsync(deletecommentId);

            result = commentSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingComment = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async void SaveComment_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveComment(ViewModel.CommentSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }
    private async Task SaveComment(Tbl93Comment commentSelected)
    {
        if (ViewModel.IsNewComment)
        {
            ViewModel.IsNewComment = false;
        }

        Tbl93Comment result = null;
        try
        {
            var current = await Context.Tbl93Comments.FirstOrDefaultAsync(_com => _com.CommentId == commentSelected.CommentId);
            if (null == current)
            {
                commentSelected.CountId = RandomHelper.Randomnumber();
                commentSelected.Writer = Environment.UserName;
                commentSelected.WriterDate = DateTime.Now;
                commentSelected.Updater = Environment.UserName;
                commentSelected.UpdaterDate = DateTime.Now;
                Context.Tbl93Comments.Add(commentSelected);
            }
            else
            {
                commentSelected.Updater = Environment.UserName;
                commentSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(commentSelected);
            }
            await Context.SaveChangesAsync();
            result = commentSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingComment = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl93CommentsList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl93CommentsList.IndexOf(ViewModel.Tbl93CommentsList.FirstOrDefault(t =>
            t.CommentId == ViewModel.CommentSelected.CommentId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl93CommentsList!.Count)
        {
            ViewModel.CommentItems.Clear();
            foreach (var item in ViewModel.Tbl93CommentsList)
            {
                ViewModel.CommentItems.Add(item);
            }

            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];
        }
        CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
        CommentDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl93CommentsList ??= [];
        ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

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
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
            }
            else
            {
                ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];
        }

        ViewModel.IsLoading = false;
        PlSpeciesDetailExpander.IsExpanded = false;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Comment Methods] 

    #region [Name Methods]

    private void AddName_Click(object sender, RoutedEventArgs e)
    {
        var animaliaRegnum = Context.Tbl69FiSpeciesses.FirstOrDefault(x => x.FiSpeciesName == "Animalia#Regnum#");

        if (animaliaRegnum != null)
        {
            _fishId = animaliaRegnum.FiSpeciesId;
        }

        ViewModel.IsInEdit = true;
        ViewModel.IsNewName = true;

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl78NamesList ??= [];
        ViewModel.Tbl78NamesList.Insert(index: 0, item: new Tbl78Name { NameName = StringsRes.DatasetNew, PlSpeciesId = id, FiSpeciesId = _fishId });
        ViewModel.NameSelected = ViewModel.Tbl78NamesList[0];

        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;
    }
    private void DataGrid_Name_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl78NamesList.Sort);
    }
    private void AddNewNameCanceled(object sender, EventArgs e) => NamesExpander.IsExpanded = false;
    private void ViewNameHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;
    }
    private async void DeleteName_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteName(ViewModel.NameSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }
    private async Task DeleteName(Tbl78Name nameSelected)
    {
        if (nameSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl78Name result = null;
        try
        {
            var deletenameId = nameSelected.NameId;
            await ViewModel.DeleteNameAsync(deletenameId);

            result = nameSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingName = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async void SaveName_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveName(ViewModel.NameSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }

    private async Task SaveName(Tbl78Name nameSelected)
    {
        if (ViewModel.IsNewName)
        {
            ViewModel.IsNewName = false;
        }

        Tbl78Name result = null;
        try
        {
            var current = await Context.Tbl78Names.FirstOrDefaultAsync(p => p.NameId == nameSelected.NameId);
            if (null == current)
            {
                nameSelected.CountId = RandomHelper.Randomnumber();
                nameSelected.Writer = Environment.UserName;
                nameSelected.WriterDate = DateTime.Now;
                nameSelected.Updater = Environment.UserName;
                nameSelected.UpdaterDate = DateTime.Now;
                Context.Tbl78Names.Add(nameSelected);
            }
            else
            {
                nameSelected.Updater = Environment.UserName;
                nameSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(nameSelected);
            }
            await Context.SaveChangesAsync();
            result = nameSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingName = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    private void BackName_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl78NamesList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl78NamesList.IndexOf(ViewModel.Tbl78NamesList.FirstOrDefault(t =>
            t.NameId == ViewModel.NameSelected.NameId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl78NamesList!.Count)
        {
            ViewModel.NameItems.Clear();
            foreach (var item in ViewModel.Tbl78NamesList)
            {
                ViewModel.NameItems.Add(item);
            }

            ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];
        }
        NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
        NameDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl78NamesList ??= [];
        ViewModel.Tbl78NamesList = [.. Context.Tbl78Names
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshNameItems();

        if (ViewModel.Tbl78NamesList.Count == 0)
        {
            NamesExpander.Visibility = Visibility.Collapsed;
            NameDataGrid.ItemsSource = null;
        }
        else
        {
            NamesExpander.Visibility = Visibility.Visible;
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
        }
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.NameSelected = ViewModel.Tbl78NamesList[0];
            }
            else
            {
                ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];
        }

        ViewModel.IsLoading = false;
        PlSpeciesDetailExpander.IsExpanded = false;

        NameDetailExpander.IsExpanded = false;
        NameDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Name Methods]

    #region [Image Methods]

    private void AddImage_Click(object sender, RoutedEventArgs e)
    {
        var animaliaRegnum = Context.Tbl69FiSpeciesses.FirstOrDefault(x => x.FiSpeciesName == "Animalia#Regnum#");

        if (animaliaRegnum != null)
        {
            _fishId = animaliaRegnum.FiSpeciesId;
        }

        ViewModel.IsInEdit = true;
        ViewModel.IsNewImage = true;

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl81ImagesList ??= [];
        ViewModel.Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { Info = StringsRes.DatasetNew, PlSpeciesId = id, FiSpeciesId = _fishId });
        ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];

        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;

        //New Image search
        OpenFileDialog();
        ViewModel.RefreshImageItems();

    }
    private void DataGrid_Image_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl81ImagesList.Sort);
    }

    private void AddNewImageCanceled(object sender, EventArgs e) => ImagesExpander.IsExpanded = false;
    private void ViewImageHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;
    }
    private async void DeleteImage_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteImage(ViewModel.ImageSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }
    private async Task DeleteImage(Tbl81Image imageSelected)
    {
        if (imageSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl81Image result = null;
        try
        {
            var deleteimageId = imageSelected.ImageId;
            await ViewModel.DeleteImageAsync(deleteimageId);

            result = imageSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingImage = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async void SaveImage_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveImage(ViewModel.ImageSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }
    private async Task SaveImage(Tbl81Image imageSelected)
    {
        if (ViewModel.IsNewImage)
        {
            ViewModel.IsNewImage = false;
        }

        Tbl81Image result = null;
        try
        {
            var current = await Context.Tbl81Images.FirstOrDefaultAsync(p => p.ImageId == imageSelected.ImageId);
            if (null == current)
            {
                imageSelected.CountId = RandomHelper.Randomnumber();
                imageSelected.Writer = Environment.UserName;
                imageSelected.WriterDate = DateTime.Now;
                imageSelected.Updater = Environment.UserName;
                imageSelected.UpdaterDate = DateTime.Now;
                Context.Tbl81Images.Add(imageSelected);
            }
            else
            {
                imageSelected.Updater = Environment.UserName;
                imageSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(imageSelected);
            }
            await Context.SaveChangesAsync();
            result = imageSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingImage = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    private void BackImage_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl81ImagesList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl81ImagesList.IndexOf(ViewModel.Tbl81ImagesList.FirstOrDefault(t =>
            t.ImageId == ViewModel.ImageSelected.ImageId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl81ImagesList!.Count)
        {
            ViewModel.ImageItems.Clear();
            foreach (var item in ViewModel.Tbl81ImagesList)
            {
                ViewModel.ImageItems.Add(item);
            }

            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];
        }
        ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
        ImageDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl81ImagesList ??= [];
        ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDataGrid.ItemsSource = null;
        }
        else
        {
            ImagesExpander.Visibility = Visibility.Visible;
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
        }
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];
            }
            else
            {
                ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];
        }

        ViewModel.IsLoading = false;
        PlSpeciesDetailExpander.IsExpanded = false;

        ImageDetailExpander.IsExpanded = false;
        ImageDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Image Methods] 

    #region [Synonym Methods]

    private void AddSynonym_Click(object sender, RoutedEventArgs e)
    {
        var animaliaRegnum = Context.Tbl69FiSpeciesses.FirstOrDefault(x => x.FiSpeciesName == "Animalia#Regnum#");

        if (animaliaRegnum != null)
        {
            _fishId = animaliaRegnum.FiSpeciesId;
        }

        ViewModel.IsInEdit = true;
        ViewModel.IsNewSynonym = true;

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl84SynonymsList ??= [];
        ViewModel.Tbl84SynonymsList.Insert(index: 0, item: new Tbl84Synonym { Info = StringsRes.DatasetNew, PlSpeciesId = id, FiSpeciesId = _fishId });
        ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[0];

        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;
    }
    private void DataGrid_Synonym_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl84SynonymsList.Sort);
    }
    private void ViewSynonymHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;
    }

    private async void DeleteSynonym_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteSynonym(ViewModel.SynonymSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }

    private async Task DeleteSynonym(Tbl84Synonym synonymSelected)
    {
        if (synonymSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl84Synonym result = null;
        try
        {
            var deletesynonymId = synonymSelected.SynonymId;
            await ViewModel.DeleteSynonymAsync(deletesynonymId);

            result = synonymSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingSynonym = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async void SaveSynonym_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveSynonym(ViewModel.SynonymSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }

    private async Task SaveSynonym(Tbl84Synonym synonymSelected)
    {
        if (ViewModel.IsNewSynonym)
        {
            ViewModel.IsNewSynonym = false;
        }

        Tbl84Synonym result = null;
        try
        {
            var current = await Context.Tbl84Synonyms.FirstOrDefaultAsync(p => p.SynonymId == synonymSelected.SynonymId);
            if (null == current)
            {
                synonymSelected.CountId = RandomHelper.Randomnumber();
                synonymSelected.Writer = Environment.UserName;
                synonymSelected.WriterDate = DateTime.Now;
                synonymSelected.Updater = Environment.UserName;
                synonymSelected.UpdaterDate = DateTime.Now;
                Context.Tbl84Synonyms.Add(synonymSelected);
            }
            else
            {
                synonymSelected.Updater = Environment.UserName;
                synonymSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(synonymSelected);
            }
            await Context.SaveChangesAsync();
            result = synonymSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingSynonym = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    private void BackSynonym_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl84SynonymsList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl84SynonymsList.IndexOf(ViewModel.Tbl84SynonymsList.FirstOrDefault(t =>
            t.SynonymId == ViewModel.SynonymSelected.SynonymId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl84SynonymsList!.Count)
        {
            ViewModel.SynonymItems.Clear();
            foreach (var item in ViewModel.Tbl84SynonymsList)
            {
                ViewModel.SynonymItems.Add(item);
            }

            ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];
        }
        SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
        SynonymDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl84SynonymsList ??= [];
        ViewModel.Tbl84SynonymsList = [.. Context.Tbl84Synonyms
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshSynonymItems();

        if (ViewModel.Tbl84SynonymsList.Count == 0)
        {
            SynonymsExpander.Visibility = Visibility.Collapsed;
            SynonymDataGrid.ItemsSource = null;
        }
        else
        {
            SynonymsExpander.Visibility = Visibility.Visible;
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
        }
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[0];
            }
            else
            {
                ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];
        }

        ViewModel.IsLoading = false;
        PlSpeciesDetailExpander.IsExpanded = false;

        SynonymDetailExpander.IsExpanded = false;
        SynonymDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Synonym Methods] 

    #region [Geographic Methods]

    private void AddGeographic_Click(object sender, RoutedEventArgs e)
    {
        var animaliaRegnum = Context.Tbl69FiSpeciesses.FirstOrDefault(x => x.FiSpeciesName == "Animalia#Regnum#");

        if (animaliaRegnum != null)
        {
            _fishId = animaliaRegnum.FiSpeciesId;
        }

        ViewModel.IsInEdit = true;
        ViewModel.IsNewGeographic = true;

        var id = ViewModel.PlSpeciesSelected.PlSpeciesId;

        ViewModel.Tbl87GeographicsList ??= [];
        ViewModel.Tbl87GeographicsList.Insert(index: 0, item: new Tbl87Geographic { Info = StringsRes.DatasetNew, PlSpeciesId = id, FiSpeciesId = _fishId });
        ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[0];

        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;
    }
    private void DataGrid_Geographic_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl87GeographicsList.Sort);
    }
    private void AddNewGeographicCanceled(object sender, EventArgs e) => GeographicsExpander.IsExpanded = false;
    private void ViewGeographicHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;
    }
    private async void DeleteGeographic_Click(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog
        {
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await DeleteGeographic(ViewModel.GeographicSelected);
                break;
            case DeleteDialogResult.DontDelete:
                await ViewModel.RevertChangesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
        }
    }
    private async Task DeleteGeographic(Tbl87Geographic geographicSelected)
    {
        if (geographicSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl87Geographic result = null;
        try
        {
            var deletegeographicId = geographicSelected.GeographicId;
            await ViewModel.DeleteImageAsync(deletegeographicId);

            result = geographicSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingGeographic = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async void SaveGeographic_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveChangesDialog
        {
            Title = StringsRes.SaveChanges,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                await SaveGeographic(ViewModel.GeographicSelected);
                break;
            case SaveChangesDialogResult.DontSave:
                await ViewModel.RevertChangesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
        }
    }

    private async Task SaveGeographic(Tbl87Geographic geographicSelected)
    {
        if (ViewModel.IsNewGeographic)
        {
            ViewModel.IsNewGeographic = false;
        }

        Tbl87Geographic result = null;
        try
        {
            var current = await Context.Tbl87Geographics.FirstOrDefaultAsync(p => p.GeographicId == geographicSelected.GeographicId);
            if (null == current)
            {
                geographicSelected.CountId = RandomHelper.Randomnumber();
                geographicSelected.Writer = Environment.UserName;
                geographicSelected.WriterDate = DateTime.Now;
                geographicSelected.Updater = Environment.UserName;
                geographicSelected.UpdaterDate = DateTime.Now;
                Context.Tbl87Geographics.Add(geographicSelected);
            }
            else
            {
                geographicSelected.Updater = Environment.UserName;
                geographicSelected.UpdaterDate = DateTime.Now;
                Context.Entry(current).CurrentValues.SetValues(geographicSelected);
            }
            await Context.SaveChangesAsync();
            result = geographicSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            throw new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2, ex);
        }

        if (result != null)
        {
            ViewModel.IsInEdit = false;
            ViewModel.IsModified = false;
            ViewModel.IsExistingGeographic = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    private void BackGeographic_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Tbl87GeographicsList ??= [];
        //   Index Position ?
        var iNdx = ViewModel.Tbl87GeographicsList.IndexOf(ViewModel.Tbl87GeographicsList.FirstOrDefault(t =>
            t.GeographicId == ViewModel.GeographicSelected.GeographicId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl87GeographicsList!.Count)
        {
            ViewModel.GeographicItems.Clear();
            foreach (var item in ViewModel.Tbl87GeographicsList)
            {
                ViewModel.GeographicItems.Add(item);
            }

            ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];
        }
        GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
        GeographicDataGrid.SelectedIndex = iNdx;

        ViewModel.Tbl87GeographicsList ??= [];
        ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
            .AsNoTracking()
            .Where(com => com.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId)];

        ViewModel.RefreshGeographicItems();

        if (ViewModel.Tbl87GeographicsList.Count == 0)
        {
            GeographicsExpander.Visibility = Visibility.Collapsed;
            GeographicDataGrid.ItemsSource = null;
        }
        else
        {
            GeographicsExpander.Visibility = Visibility.Visible;
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
        }
        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[0];
            }
            else
            {
                ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];
        }

        ViewModel.IsLoading = false;
        PlSpeciesDetailExpander.IsExpanded = false;

        GeographicDetailExpander.IsExpanded = false;
        GeographicDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Geographic Methods]    

    #region [Refresh Methods]
    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsLoading = true;

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        //   Index Position ?
        var iNdx = ViewModel.Tbl72PlSpeciessesList.IndexOf(ViewModel.Tbl72PlSpeciessesList.First(t =>
            t.PlSpeciesId == ViewModel.PlSpeciesSelected.PlSpeciesId));
        //   Index Position ?
        if (iNdx < ViewModel.Tbl72PlSpeciessesList!.Count)
        {
            ViewModel.PlSpeciesItems.Clear();
            foreach (var item in ViewModel.Tbl72PlSpeciessesList)
            {
                ViewModel.PlSpeciesItems.Add(item);
            }

            ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[iNdx];
        }
        PlSpeciesDataGrid.ItemsSource = ViewModel.Tbl72PlSpeciessesList;
        PlSpeciesDataGrid.SelectedIndex = iNdx;


        ViewModel.Tbl72PlSpeciessesList ??= [];
        //ViewModel.Tbl72PlSpeciessesList = int.TryParse(_search, out var id)
        //    ? new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
        //        .Where(a => a.PlSpeciesId == id))
        //    : new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
        //        .Where(a => _search != null && a.PlSpeciesName.StartsWith(_search))
        //        .OrderBy(a => a.Tbl66Genusses.GenusName)
        //        .ThenBy(a => a.PlSpeciesName)
        //        .ThenBy(a => a.Subspecies)
        //        .ThenBy(a => a.Divers));
        //search about GenusName
        ViewModel.Tbl72PlSpeciessesList = int.TryParse(_search, out var id)
            ? [.. Context.Tbl72PlSpeciesses
                .Where(a => a.PlSpeciesId == id)]
            : [.. Context.Tbl72PlSpeciesses
                .Where(a => a.Tbl66Genusses.GenusName.StartsWith(_search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)];

        if (ViewModel.Tbl72PlSpeciessesList is { Count: 0 })
        {
            ViewModel.Tbl72PlSpeciessesList?.Clear();
            ViewModel.Tbl90ReferencesList?.Clear();
            ViewModel.Tbl93CommentsList?.Clear();
            ViewModel.Tbl78NamesList?.Clear();
            ViewModel.Tbl81ImagesList?.Clear();
            ViewModel.Tbl84SynonymsList?.Clear();
            ViewModel.Tbl87GeographicsList?.Clear();
            ViewModel.IsLoading = false;
            await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            return;
        }
        PlSpeciesDataGrid.ItemsSource = ViewModel.Tbl72PlSpeciessesList;

        if (ViewModel.IsDeleting)
        {
            if (iNdx == 0)
            {
                ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[0];
            }
            else
            {
                ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[iNdx - 1];
            }
        }
        else
        {
            ViewModel.PlSpeciesSelected = ViewModel.Tbl72PlSpeciessesList[iNdx];
        }

        PlSpeciesDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeleting = false;
    }
    #endregion [Refresh Methods]

    private async void OpenFileDialog()
    {
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(_filePicker, hwnd);
        _filePicker.FileTypeFilter.Add("*");
        _filePicker.ViewMode = PickerViewMode.Thumbnail;
        //_filePicker.FileTypeFilter.Add("*");
        _filePicker.FileTypeFilter.Add(".jpg");
        _filePicker.FileTypeFilter.Add(".jpeg");
        _filePicker.FileTypeFilter.Add(".png");
        _filePicker.FileTypeFilter.Add(".gif");
        _filePicker.FileTypeFilter.Add(".jpe");
        _filePicker.FileTypeFilter.Add(".bmp");
        _filePicker.FileTypeFilter.Add(".ico");
        _filePicker.FileTypeFilter.Add(".tif");
        _filePicker.FileTypeFilter.Add(".tiff");
        _filePicker.FileTypeFilter.Add(".hpd");
        _filePicker.FileTypeFilter.Add(".jxr");
        _filePicker.FileTypeFilter.Add(".wdp");
        _filePicker.FileTypeFilter.Add(".hpd");
        var file = await _filePicker.PickSingleFileAsync();
        if (file != null && file.Path != null)
        {
            SelectedPathImage.Text = file.Path;
            Image1.Source = new BitmapImage(new Uri(file.Path));
        }
    }


    private async void ChangeImage_Click(object sender, RoutedEventArgs e)
    {
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(_filePicker, hwnd);
        _filePicker.FileTypeFilter.Add("*");
        _filePicker.ViewMode = PickerViewMode.Thumbnail;
        //_filePicker.FileTypeFilter.Add("*");
        _filePicker.FileTypeFilter.Add(".jpg");
        _filePicker.FileTypeFilter.Add(".jpeg");
        _filePicker.FileTypeFilter.Add(".png");
        _filePicker.FileTypeFilter.Add(".gif");
        _filePicker.FileTypeFilter.Add(".jpe");
        _filePicker.FileTypeFilter.Add(".bmp");
        _filePicker.FileTypeFilter.Add(".ico");
        _filePicker.FileTypeFilter.Add(".tif");
        _filePicker.FileTypeFilter.Add(".tiff");
        _filePicker.FileTypeFilter.Add(".hpd");
        _filePicker.FileTypeFilter.Add(".jxr");
        _filePicker.FileTypeFilter.Add(".wdp");
        _filePicker.FileTypeFilter.Add(".hpd");
        var file = await _filePicker.PickSingleFileAsync();
        if (file != null && file.Path != null)
        {
            SelectedPathImage.Text = file.Path;
            Image1.Source = new BitmapImage(new Uri(file.Path));
        }
    }

    //private void ScrollViewerImage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    //{
    //    throw new NotImplementedException();
    //}

    //private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    //{
    //    throw new NotImplementedException();
    //}
}
