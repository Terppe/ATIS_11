using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl75SpeciesViewModel Skriptdatum:  20.10.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl75SpeciesViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];
    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl78Name> NameItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
    public ObservableCollection<Tbl84Synonym> SynonymItems { get; } = [];
    public ObservableCollection<Tbl87Geographic> GeographicItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    private readonly FileOpenPicker _filePicker = new();
    public Tbl75Species SingleSpecies { get; set; }

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };

    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl75SpeciesViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];

        Tbl68SpeciesgroupsAllList ??= [];
        Tbl68SpeciesgroupsAllList = [.. _dataService.GetSpeciesgroupAsync()];

        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsyncAlt()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsyncAlt()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsyncAlt()];

        TblCountriesAllList ??= [];
        TblCountriesAllList = new ObservableCollection<TblCountry>(_dataService.GetCountryAsync().GetAwaiter().GetResult());

        //  Languages = new List<Language>();
        //GetValueLanguage();
        //GetValueContinent();
        //GetValueMimeType();
    }

    #endregion [Constructor]  


    //    Part 1    

    #region [Methods Species]

    public async Task AddSpeciesAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditSpecies = true;
            IsNewSpecies = true;

            Tbl66GenussesAllList ??= [];
            Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];

            var genusId = Tbl66GenussesAllList.First().GenusId;

            Tbl68SpeciesgroupsAllList ??= [];
            Tbl68SpeciesgroupsAllList = [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupName == "NotClear")];

            var speciesgroupId = Tbl68SpeciesgroupsAllList.First().SpeciesgroupId;

            Tbl75SpeciessesList ??= [];
            Tbl75SpeciessesList.Insert(index: 0, item: new Tbl75Species { GenusId = genusId, SpeciesgroupId = speciesgroupId, Creature = "" });
        });
    }

    public async Task SaveSpeciesAsync(Tbl75Species viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpecies)
            {
                IsNewSpecies = false;
            }
        });

        Tbl75Species result = null;
        try
        {
            if (IsModifiedSpecies)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {

                    result = await _dataService.UpsertSpeciesAsync(viewModelSpeciesSelected);
                    IsModifiedSpecies = false;
                });
            }
            else
            {
                IsModifiedSpecies = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }

        if (result != null)
        {
            IsInEditSpecies = false;
            IsModifiedSpecies = false;
            IsExistingSpecies = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteSpeciesAsync(Tbl75Species viewModelSpeciesSelected)
    {
        Tbl75Species result = null;
        try
        {
            var deleteSpeciesId = viewModelSpeciesSelected.SpeciesId;
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                await DeleteSpecies(deleteSpeciesId);
            });
            result = viewModelSpeciesSelected;
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
            IsInEditSpecies = false;
            IsModifiedSpecies = false;
            IsExistingSpecies = true;
            IsDeletingSpecies = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteSpecies(int speciesid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var species = await AppData.Data.Tbl75Speciesses.FirstOrDefaultAsync(b => b.SpeciesId == speciesid);
            if (species != null)
            {
                await RemoveExtras.RemoveReferencesBySpeciesId(speciesid);
                await RemoveExtras.RemoveCommentsBySpeciesId(speciesid);
                await RemoveExtras.RemoveNamesBySpeciesId(speciesid);
                await RemoveExtras.RemoveImagesBySpeciesId(speciesid);
                await RemoveExtras.RemoveSynonymsBySpeciesId(speciesid);
                await RemoveExtras.RemoveGeographicsBySpeciesId(speciesid);

                //AppData.Data.Tbl75Speciesses.Remove(species);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Species]  

    #region [Methods Reference]

    public async Task AddReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90ExpertsAllList ??= [];
            Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsyncAlt()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, SpeciesId = SpeciesSelected.SpeciesId });
        });
    }
    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsyncAlt()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, SpeciesId = SpeciesSelected.SpeciesId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsyncAlt()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, SpeciesId = SpeciesSelected.SpeciesId });
        });
    }

    public async Task SaveReferenceAsync(Tbl90Reference viewModelReferenceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewReference)
            {
                IsNewReference = false;
            }
        });
        Tbl90Reference result = null;
        try
        {
            if (IsModifiedReference)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertReferenceAsync(viewModelReferenceSelected);
                    IsModifiedReference = false;
                });
            }
            else
            {
                IsModifiedReference = true;
            }
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
            IsInEditReference = false;
            IsModifiedReference = false;
            IsExistingReference = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteReferenceAsync(Tbl90Reference viewModelReferenceSelected)
    {
        if (viewModelReferenceSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl90Reference result = null;
        try
        {
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                var deletereferenceId = viewModelReferenceSelected.ReferenceId;
                await DeleteReference(deletereferenceId);
            });
            result = viewModelReferenceSelected;
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
            IsInEditReference = false;
            IsModifiedReference = false;
            IsExistingReference = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteReference(int referenceid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var reference = await AppData.Data.Tbl90References.FirstOrDefaultAsync(re => re.ReferenceId == referenceid);

            //AppData.Data.Tbl90References.Remove(reference);
            //await AppData.Data.SaveChangesAsync();
        });
    }

    #endregion [Methods Reference]

    #region [Methods Comment]

    public async Task AddComment()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditComment = true;
            IsNewComment = true;

            var id = SpeciesSelected.SpeciesId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment { SpeciesId = id });
        });
    }

    public async Task SaveCommentAsync(Tbl93Comment viewModelCommentSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewComment)
            {
                IsNewComment = false;
            }
        });

        Tbl93Comment result = null;
        try
        {
            if (IsModifiedComment)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertCommentAsync(viewModelCommentSelected);
                    IsModifiedComment = false;
                });
            }
            else
            {
                IsModifiedComment = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }
        if (result != null)
        {
            IsInEditComment = false;
            IsModifiedComment = false;
            IsExistingComment = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteCommentAsync(Tbl93Comment viewModelCommentSelected)
    {
        if (viewModelCommentSelected == null)
        {
            await AllDialogs.NoDatasetSelectedWarnMessageDialogAsync();
            return;
        }

        Tbl93Comment result = null;
        try
        {
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                var deletecommentId = viewModelCommentSelected.CommentId;
                await DeleteComment(deletecommentId);
            });
            result = viewModelCommentSelected;
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
            IsInEditComment = false;
            IsModifiedComment = false;
            IsExistingComment = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async Task DeleteComment(int commentid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var comment = await AppData.Data.Tbl93Comments.FirstOrDefaultAsync(co => co.CommentId == commentid);

            //AppData.Data.Tbl93Comments.Remove(comment);
            //await AppData.Data.SaveChangesAsync();
        });
    }

    #endregion [Methods Comment] 

    #region [Methods Name]

    public async Task AddNameAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditName = true;
            IsNewName = true;

            var  speciesId = SpeciesSelected.SpeciesId;

            Tbl78NamesList ??= [];
            Tbl78NamesList.Insert(index: 0, item: new Tbl78Name { SpeciesId = speciesId });
            return Task.CompletedTask;
        });
    }
    public async Task SaveNameAsync(Tbl78Name viewModelNameSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewName)
            {
                IsNewName = false;
            }
        });

        Tbl78Name result = null;
        try
        {
            if (IsModifiedName)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertNameAsync(viewModelNameSelected);
                    IsModifiedName = false;
                });
            }
            else
            {
                IsModifiedName = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }
        if (result != null)
        {
            IsInEditName = false;
            IsModifiedName = false;
            IsExistingName = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteNameAsync(Tbl78Name viewModelNameSelected)
    {
        Tbl78Name result = null;
        try
        {
            var deleteNameId = viewModelNameSelected.NameId;
            await DeleteName(deleteNameId);

            result = viewModelNameSelected;
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
            IsInEditName = false;
            IsModifiedName = false;
            IsExistingName = true;
            IsDeletingName = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    public static async Task DeleteName(int nameid)
    {
        var name = await AppData.Data.Tbl78Names.FirstOrDefaultAsync(co => co.NameId == nameid);

        //AppData.Data.Tbl78Names.Remove(name);
        //await AppData.Data.SaveChangesAsync();
    }

    #endregion [Methods Name]  

    #region [Methods Image]
    public async Task AddImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditImage = true;
            IsNewImage = true;

            var speciesId = SpeciesSelected.SpeciesId;

            Tbl81ImagesList ??= [];
            Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { SpeciesId = speciesId});
            return Task.CompletedTask;
        });
    }

    public async Task SaveImageAsync(Tbl81Image viewModelImageSelected, string viewModelSelectedPath)
    {
        if (IsNewImage)
        {
            IsNewImage = false;
        }

        Tbl81Image result = null;
        try
        {
            if (IsModifiedImage)
            {
                result = await _dataService.UpsertImageAsync(viewModelImageSelected, viewModelSelectedPath);
                IsModifiedImage = false;
            }
            else
            {
                IsModifiedImage = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }
        if (result != null)
        {
            IsInEditImage = false;
            IsModifiedImage = false;
            IsExistingImage = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteImageAsync(Tbl81Image viewModelImageSelected)
    {
        Tbl81Image result = null;
        try
        {
            var deleteImageId = viewModelImageSelected.ImageId;
            await DeleteImage(deleteImageId);

            result = viewModelImageSelected;
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
            IsInEditImage = false;
            IsModifiedImage = false;
            IsExistingImage = true;
            IsDeletingImage = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    public static async Task DeleteImage(int imageid)
    {
        var image = await AppData.Data.Tbl81Images.FirstOrDefaultAsync(co => co.ImageId == imageid);

        //AppData.Data.Tbl81Images.Remove(image);
        //await AppData.Data.SaveChangesAsync();
    }

    private static byte[] LoadImageData(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new BinaryReader(fs);
        var imageBytes = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return imageBytes;
    }

    #endregion [Methods Image]

    #region [Methods Synonym]
    public async Task AddSynonymAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditSynonym = true;
            IsNewSynonym = true;

           var speciesId = SpeciesSelected.SpeciesId;

            Tbl84SynonymsList ??= [];
            Tbl84SynonymsList.Insert(index: 0, item: new Tbl84Synonym { SpeciesId = speciesId});
            return Task.CompletedTask;
        });
    }
    public async Task SaveSynonymAsync(Tbl84Synonym viewModelSynonymSelected)
    {
        if (IsNewSynonym)
        {
            IsNewSynonym = false;
        }

        Tbl84Synonym result = null;
        try
        {
            if (IsModifiedSynonym)
            {
                result = await _dataService.UpsertSynonymAsync(viewModelSynonymSelected);
                IsModifiedSynonym = false;
            }
            else
            {
                IsModifiedSynonym = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }
        if (result != null)
        {
            IsInEditSynonym = false;
            IsModifiedSynonym = false;
            IsExistingSynonym = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteSynonymAsync(Tbl84Synonym viewModelSynonymSelected)
    {
        Tbl84Synonym result = null;
        try
        {
            var deleteSynonymId = viewModelSynonymSelected.SynonymId;
            await DeleteSynonym(deleteSynonymId);

            result = viewModelSynonymSelected;
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
            IsInEditSynonym = false;
            IsModifiedSynonym = false;
            IsExistingSynonym = true;
            IsDeletingSynonym = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    public static async Task DeleteSynonym(int synonymid)
    {
        var synonym = await AppData.Data.Tbl84Synonyms.FirstOrDefaultAsync(co => co.SynonymId == synonymid);

        //AppData.Data.Tbl84Synonyms.Remove(synonym);
        //await AppData.Data.SaveChangesAsync();
    }
    #endregion [Methods Synonym]

    #region [Methods Geographic]
    public async Task AddGeographicAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditGeographic = true;
            IsNewGeographic = true;

            var speciesId = SpeciesSelected.SpeciesId;

            Tbl87GeographicsList ??= [];
            Tbl87GeographicsList.Insert(index: 0, item: new Tbl87Geographic { SpeciesId = speciesId });
            return Task.CompletedTask;
        });
    }
    public async Task SaveGeographicAsync(Tbl87Geographic viewModelGeographicSelected)
    {
        if (IsNewGeographic)
        {
            IsNewGeographic = false;
        }

        Tbl87Geographic result = null;
        try
        {
            if (IsModifiedGeographic)
            {
                result = await _dataService.UpsertGeographicAsync(viewModelGeographicSelected);
                IsModifiedGeographic = false;
            }
            else
            {
                IsModifiedGeographic = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }
        if (result != null)
        {
            IsInEditGeographic = false;
            IsModifiedGeographic = false;
            IsExistingGeographic = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }
    public async Task DeleteGeographicAsync(Tbl87Geographic viewModelGeographicSelected)
    {
        Tbl87Geographic result = null;
        try
        {
            var deleteGeographicId = viewModelGeographicSelected.GeographicId;
            await DeleteGeographic(deleteGeographicId);

            result = viewModelGeographicSelected;
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
            IsInEditGeographic = false;
            IsModifiedGeographic = false;
            IsExistingGeographic = true;
            IsDeletingGeographic = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    public static async Task DeleteGeographic(int geographicid)
    {
        var geographic = await AppData.Data.Tbl87Geographics.FirstOrDefaultAsync(co => co.GeographicId == geographicid);

        //AppData.Data.Tbl87Geographics.Remove(geographic);
        //await AppData.Data.SaveChangesAsync();
    }
    #endregion [Methods Geographic]  

    //    Part 2


    #region All Properties

    #region Image

    private byte[] _selectedFilestream = null!;
    public byte[] SelectedFilestream
    {
        get => _selectedFilestream;
        set
        {
            _selectedFilestream = value; OnPropertyChanged();
        }
    }

    //   public new event PropertyChangedEventHandler PropertyChanged;

    private byte[] _imageBuffer = null!;
    public byte[] ImageBuffer
    {
        get => _imageBuffer;
        set
        {
            _imageBuffer = value;
            using var loadImageTask = LoadImageAsync();
        }
    }

    private ImageSource _imageSource = null!;
    public ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            _imageSource = value; OnPropertyChanged(nameof(ImageSource));
        }
    }

    private async Task LoadImageAsync()
    {
        using var ms = new InMemoryRandomAccessStream();
        // Writes the image byte array in an InMemoryRandomAccessStream
        // that is needed to set the source of BitmapImage.
        using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
        {
            writer.WriteBytes(_imageBuffer);
            await writer.StoreAsync();
        }

        var image = new BitmapImage();
        await image.SetSourceAsync(ms);
        ImageSource = image;
    }

    #endregion

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedSpecies { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedName { get; set; }
    public bool IsModifiedImage { get; set; }
    public bool IsModifiedSynonym { get; set; }
    public bool IsModifiedGeographic { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => GenusItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    private bool _isDeletingReference;
    public bool IsDeletingReference
    {
        get => _isDeletingReference;
        set => SetProperty(ref _isDeletingReference, value);
    }
    private bool _isDeletingComment;
    public bool IsDeletingComment
    {
        get => _isDeletingComment;
        set => SetProperty(ref _isDeletingComment, value);
    }
    private bool _isDeletingName;
    public bool IsDeletingName
    {
        get => _isDeletingName;
        set => SetProperty(ref _isDeletingName, value);
    }
    private bool _isDeletingImage;
    public bool IsDeletingImage
    {
        get => _isDeletingImage;
        set => SetProperty(ref _isDeletingImage, value);
    }
    private bool _isDeletingSynonym;
    public bool IsDeletingSynonym
    {
        get => _isDeletingSynonym;
        set => SetProperty(ref _isDeletingSynonym, value);
    }
    private bool _isDeletingGeographic;
    public bool IsDeletingGeographic
    {
        get => _isDeletingGeographic;
        set => SetProperty(ref _isDeletingGeographic, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    private bool _isNewReference;
    public bool IsNewReference
    {
        get => _isNewReference;
        set => SetProperty(ref _isNewReference, value);
    }
    private bool _isNewReferenceExpert;
    public bool IsNewReferenceExpert
    {
        get => _isNewReferenceExpert;
        set => SetProperty(ref _isNewReferenceExpert, value);
    }

    private bool _isNewReferenceSource;
    public bool IsNewReferenceSource
    {
        get => _isNewReferenceSource;
        set => SetProperty(ref _isNewReferenceSource, value);
    }

    private bool _isNewReferenceAuthor;
    public bool IsNewReferenceAuthor
    {
        get => _isNewReferenceAuthor;
        set => SetProperty(ref _isNewReferenceAuthor, value);
    }

    private bool _isNewComment;
    public bool IsNewComment
    {
        get => _isNewComment;
        set => SetProperty(ref _isNewComment, value);
    }
    private bool _isNewName;
    public bool IsNewName
    {
        get => _isNewName;
        set => SetProperty(ref _isNewName, value);
    }

    private bool _isNewImage;
    public bool IsNewImage
    {
        get => _isNewImage;
        set => SetProperty(ref _isNewImage, value);
    }

    private bool _isNewSynonym;
    public bool IsNewSynonym
    {
        get => _isNewSynonym;
        set => SetProperty(ref _isNewSynonym, value);
    }

    private bool _isNewGeographic;
    public bool IsNewGeographic
    {
        get => _isNewGeographic;
        set => SetProperty(ref _isNewGeographic, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }
    private bool _isExistingReference;

    public bool IsExistingReference
    {
        get => _isExistingReference;
        set => SetProperty(ref _isExistingReference, value);
    }
    private bool _isExistingComment;

    public bool IsExistingComment
    {
        get => _isExistingComment;
        set => SetProperty(ref _isExistingComment, value);
    }
    private bool _isExistingName;
    public bool IsExistingName
    {
        get => _isExistingName;
        set => SetProperty(ref _isExistingName, value);
    }

    private bool _isExistingImage;
    public bool IsExistingImage
    {
        get => _isExistingImage;
        set => SetProperty(ref _isExistingImage, value);
    }

    private bool _isExistingSynonym;
    public bool IsExistingSynonym
    {
        get => _isExistingSynonym;
        set => SetProperty(ref _isExistingSynonym, value);
    }

    private bool _isExistingGeographic;
    public bool IsExistingGeographic
    {
        get => _isExistingGeographic;
        set => SetProperty(ref _isExistingGeographic, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    private bool _isInEditReference = false;
    public bool IsInEditReference
    {
        get => _isInEditReference;
        set => SetProperty(ref _isInEditReference, value);
    }
    private bool _isInEditReferenceExpert = false;
    public bool IsInEditReferenceExpert
    {
        get => _isInEditReferenceExpert;
        set => SetProperty(ref _isInEditReferenceExpert, value);
    }
    private bool _isInEditReferenceSource = false;
    public bool IsInEditReferenceSource
    {
        get => _isInEditReferenceSource;
        set => SetProperty(ref _isInEditReferenceSource, value);
    }
    private bool _isInEditReferenceAuthor = false;
    public bool IsInEditReferenceAuthor
    {
        get => _isInEditReferenceAuthor;
        set => SetProperty(ref _isInEditReferenceAuthor, value);
    }

    private bool _isInEditComment = false;
    public bool IsInEditComment
    {
        get => _isInEditComment;
        set => SetProperty(ref _isInEditComment, value);
    }
    private bool _isInEditName = false;
    public bool IsInEditName
    {
        get => _isInEditName;
        set => SetProperty(ref _isInEditName, value);
    }
    private bool _isInEditImage = false;
    public bool IsInEditImage
    {
        get => _isInEditImage;
        set => SetProperty(ref _isInEditImage, value);
    }
    private bool _isInEditSynonym = false;
    public bool IsInEditSynonym
    {
        get => _isInEditSynonym;
        set => SetProperty(ref _isInEditSynonym, value);
    }
    private bool _isInEditGeographic = false;
    public bool IsInEditGeographic
    {
        get => _isInEditGeographic;
        set => SetProperty(ref _isInEditGeographic, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditSpecies()
    {
        IsInEditSpecies = true;
        IsModifiedSpecies = false;
    }

    public void StartEditReferenceExpert()
    {
        IsInEditReferenceExpert = true;
        IsModifiedReferenceExpert = false;
    }
    public void StartEditReferenceSource()
    {
        IsInEditReferenceSource = true;
        IsModifiedReferenceSource = false;
    }
    public void StartEditReferenceAuthor()
    {
        IsInEditReferenceAuthor = true;
        IsModifiedReferenceAuthor = false;
    }
    public void StartEditComment()
    {
        IsInEditComment = true;
        IsModifiedComment = false;
    }
    public void StartEditName()
    {
        IsInEditName = true;
        IsModifiedName = false;
    }
    public void StartEditImage()
    {
        IsInEditImage = true;
        IsModifiedImage = false;
    }
    public void StartEditSynonym()
    {
        IsInEditSynonym = true;
        IsModifiedSynonym = false;
    }
    public void StartEditGeographic()
    {
        IsInEditGeographic = true;
        IsModifiedGeographic = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditName() => await SaveNameAsync(NameSelected);
    public async void EndEditImage() => await SaveImageAsync(ImageSelected, SelectedPath);
    public async void EndEditSynonym() => await SaveSynonymAsync(SynonymSelected);
    public async void EndEditGeographic() => await SaveGeographicAsync(GeographicSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshName => !IsModifiedName && IsExistingName;
    public bool CanRefreshImage => !IsModifiedImage && IsExistingImage;
    public bool CanRefreshSynonym => !IsModifiedSynonym && IsExistingSynonym;
    public bool CanRefreshGeographic => !IsModifiedGeographic && IsExistingGeographic;
    //--------------CanRevert-------------------------------------
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertName => IsModifiedName && IsExistingName;
    public bool CanRevertImage => IsModifiedImage && IsExistingImage;
    public bool CanRevertSynonym => IsModifiedSynonym && IsExistingSynonym;
    public bool CanRevertGeographic => IsModifiedGeographic && IsExistingGeographic;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl75Species _speciesSelected;
    public Tbl75Species SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
    }

    private Tbl90Reference _referenceSelected;
    public Tbl90Reference ReferenceSelected
    {
        get => _referenceSelected;
        set => SetProperty(ref _referenceSelected, value);
    }

    private Tbl93Comment _commentSelected;
    public Tbl93Comment CommentSelected
    {
        get => _commentSelected;
        set => SetProperty(ref _commentSelected, value);
    }
    private Tbl78Name _nameSelected;
    public Tbl78Name NameSelected
    {
        get => _nameSelected;
        set => SetProperty(ref _nameSelected, value);
    }
    private Tbl81Image _imageSelected = null!;

    public Tbl81Image ImageSelected
    {
        get => _imageSelected;
        set
        {
            SetProperty(ref _imageSelected, value);
            // ImageSelected loose ImageFileStream
            if (_imageSelected == null || _imageSelected == null && _imageSelected!.CountId == 0)
            {
                return;
            }

            _dataSet = _fileStreamList.Find(t => t.ImageCountId == _imageSelected.CountId);

            _imageSelected.Filestream = _dataSet.ImageFileStream;
            _selectedFilestream = _dataSet.ImageFileStream;
        }
    }

    private Data _dataSet;

    public Data DataSet
    {
        get => _dataSet;
        set
        {
            _dataSet = value; OnPropertyChanged();
        }
    }
    private string _selectedPath = null!;

    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            _selectedPath = value; OnPropertyChanged();
        }
    }

    private Tbl84Synonym _synonymSelected;
    public Tbl84Synonym SynonymSelected
    {
        get => _synonymSelected;
        set => SetProperty(ref _synonymSelected, value);
    }

    private Tbl87Geographic _geographicSelected;

    public Tbl87Geographic GeographicSelected
    {
        get => _geographicSelected;
        set => SetProperty(ref _geographicSelected, value);
    }

    private Tbl90Reference _referenceExpertSelected;
    public Tbl90Reference ReferenceExpertSelected
    {
        get => _referenceExpertSelected;
        set => SetProperty(ref _referenceExpertSelected, value);
    }

    private Tbl90Reference _referenceSourceSelected;
    public Tbl90Reference ReferenceSourceSelected
    {
        get => _referenceSourceSelected;
        set => SetProperty(ref _referenceSourceSelected, value);
    }
    private Tbl90Reference _referenceAuthorSelected;
    public Tbl90Reference ReferenceAuthorSelected
    {
        get => _referenceAuthorSelected;
        set => SetProperty(ref _referenceAuthorSelected, value);
    }
    #endregion Selected Properties    

    #region Public Properties


    private int _speciesDataSetCount;

    public int SpeciesDataSetCount
    {
        get => _speciesDataSetCount;
        set
        {
            _speciesDataSetCount = value; OnPropertyChanged();
        }
    }

    private int _referenceDataSetCount;
    public int ReferenceDataSetCount
    {
        get => _referenceDataSetCount;
        set
        {
            _referenceDataSetCount = value; OnPropertyChanged();
        }
    }

    private int _commentDataSetCount;
    public int CommentDataSetCount
    {
        get => _commentDataSetCount;
        set
        {
            _commentDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _nameDataSetCount;
    public int NameDataSetCount
    {
        get => _nameDataSetCount;
        set
        {
            _nameDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _imageDataSetCount;
    public int ImageDataSetCount
    {
        get => _imageDataSetCount;
        set
        {
            _imageDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _synonymDataSetCount;
    public int SynonymDataSetCount
    {
        get => _synonymDataSetCount;
        set
        {
            _synonymDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _geographicDataSetCount;
    public int GeographicDataSetCount
    {
        get => _geographicDataSetCount;
        set
        {
            _geographicDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------
    private ObservableCollection<Tbl66Genus> _tbl66GenussesList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get => _tbl66GenussesList;
        set
        {
            _tbl66GenussesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsAllList;
    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsAllList
    {
        get => _tbl68SpeciesgroupsAllList;
        set
        {
            _tbl68SpeciesgroupsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferencesList;

    public ObservableCollection<Tbl90Reference> Tbl90ReferencesList
    {
        get => _tbl90ReferencesList;
        set
        {
            _tbl90ReferencesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefExpert> _tbl90ExpertsAllList;
    public ObservableCollection<Tbl90RefExpert> Tbl90ExpertsAllList
    {
        get => _tbl90ExpertsAllList;
        set
        {
            _tbl90ExpertsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefSource> _tbl90SourcesAllList;
    public ObservableCollection<Tbl90RefSource> Tbl90SourcesAllList
    {
        get => _tbl90SourcesAllList;
        set
        {
            _tbl90SourcesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl90RefAuthor> _tbl90AuthorsAllList;
    public ObservableCollection<Tbl90RefAuthor> Tbl90AuthorsAllList
    {
        get => _tbl90AuthorsAllList;
        set
        {
            _tbl90AuthorsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceAuthorsList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceAuthorsList
    {
        get => _tbl90ReferenceAuthorsList;
        set
        {
            _tbl90ReferenceAuthorsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceSourcesList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceSourcesList
    {
        get => _tbl90ReferenceSourcesList;
        set
        {
            _tbl90ReferenceSourcesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceExpertsList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceExpertsList
    {
        get => _tbl90ReferenceExpertsList;
        set
        {
            _tbl90ReferenceExpertsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl93Comment> _tbl93CommentsList;

    public ObservableCollection<Tbl93Comment> Tbl93CommentsList
    {
        get => _tbl93CommentsList;
        set
        {
            _tbl93CommentsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl63Infratribus> _tbl63InfratribussesAllList;
    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesAllList
    {
        get => _tbl63InfratribussesAllList;
        set
        {
            _tbl63InfratribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciesList;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesList
    {
        get => _tbl75SpeciesList;
        set
        {
            _tbl75SpeciesList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList;
    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl78Name> _tbl78NamesList;

    public ObservableCollection<Tbl78Name> Tbl78NamesList
    {
        get => _tbl78NamesList;
        set
        {
            _tbl78NamesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl81Image> _tbl81ImagesList;

    public ObservableCollection<Tbl81Image> Tbl81ImagesList
    {
        get => _tbl81ImagesList;
        set
        {
            _tbl81ImagesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl84Synonym> _tbl84SynonymsList;

    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsList
    {
        get => _tbl84SynonymsList;
        set
        {
            _tbl84SynonymsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl87Geographic> _tbl87GeographicsList;

    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsList
    {
        get => _tbl87GeographicsList;
        set
        {
            _tbl87GeographicsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblCountry> _tblCountriesAllList = null!;

    public ObservableCollection<TblCountry> TblCountriesAllList
    {
        get => _tblCountriesAllList;
        set
        {
            _tblCountriesAllList = value;
            OnPropertyChanged();
        }
    }
    //private void GetValueContinent()
    //{
    //    _continents =
    //    [
    //        new Continent { Name = "Africa" },
    //        new Continent { Name = "Antarctica" },
    //        new Continent { Name = "Asia" },
    //        new Continent { Name = "Australia" },
    //        new Continent { Name = "Central/South America" },
    //        new Continent { Name = "Europe" },
    //        new Continent { Name = "North America/Caribbean" }
    //    ];

    //    _selectedContinent = new Continent();
    //}

    private List<Continent> _continents = null!;

    public List<Continent> Continents
    {
        get => _continents;
        set
        {
            _continents = value;
            OnPropertyChanged();
        }
    }

    private Continent _selectedContinent = null!;

    public Continent SelectedContinent
    {
        get => _selectedContinent;
        set
        {
            _selectedContinent = value;
            OnPropertyChanged();
        }
    }


    public class Continent
    {
        public string Name
        {
            get; set;
        }
    }

    private ObservableCollection<TblCountry> _tblCountriesList = null!;

    public ObservableCollection<TblCountry> TblCountriesList
    {
        get => _tblCountriesList;
        set
        {
            _tblCountriesList = value;
            OnPropertyChanged(nameof(TblCountriesList));
        }
    }

    #endregion Public Properties

    #region Refresh Methods


    public void RefreshSpeciesItems()
    {
        SpeciesItems.Clear();
        if (Tbl75SpeciessesList != null)
        {
            foreach (var item in Tbl75SpeciessesList)
            {
                SpeciesItems.Add(item);
            }

            if (Tbl75SpeciessesList.Count == 0)
            {
                return;
            }

            if (SpeciesSelected == null && Tbl75SpeciessesList.Count != 0)
            {
                SpeciesSelected = SpeciesItems.FirstOrDefault()!;
            }
        }
    }

    public void RefreshReferenceItems()
    {
        ReferenceItems.Clear();
        if (Tbl90ReferencesList != null)
        {
            foreach (var item in Tbl90ReferencesList)
            {
                ReferenceItems.Add(item);
            }

            if (Tbl90ReferencesList.Count == 0)
            {
                return;
            }

            if (ReferenceSelected == null && Tbl90ReferencesList.Count != 0)
            {
                ReferenceSelected = ReferenceItems.FirstOrDefault()!;
            }
        }
    }
    public void RefreshCommentItems()
    {
        CommentItems.Clear();
        if (Tbl93CommentsList != null)
        {
            foreach (var item in Tbl93CommentsList)
            {
                CommentItems.Add(item);
            }

            if (Tbl93CommentsList.Count == 0)
            {
                return;
            }

            if (CommentSelected == null && Tbl93CommentsList.Count != 0)
            {
                CommentSelected = CommentItems.FirstOrDefault()!;
            }
        }
    }

    public void RefreshNameItems()
    {
        NameItems.Clear();
        if (Tbl78NamesList != null)
        {
            foreach (var item in Tbl78NamesList)
            {
                NameItems.Add(item);
            }

            if (Tbl78NamesList.Count == 0)
            {
                return;
            }

            if (NameSelected == null && Tbl78NamesList.Count != 0)
            {
                NameSelected = NameItems.FirstOrDefault()!;
            }
        }
    }

    public void RefreshImageItems()
    {
        ImageItems.Clear();
        if (Tbl81ImagesList != null)
        {
            foreach (var item in Tbl81ImagesList)
            {
                ImageItems.Add(item);
            }

            if (Tbl81ImagesList.Count == 0)
            {
                return;
            }

            if (ImageSelected == null && Tbl81ImagesList.Count != 0)
            {
                ImageSelected = ImageItems.FirstOrDefault()!;
            }
        }
    }

    public void RefreshSynonymItems()
    {
        SynonymItems.Clear();
        if (Tbl84SynonymsList != null)
        {
            foreach (var item in Tbl84SynonymsList)
            {
                SynonymItems.Add(item);
            }

            if (Tbl84SynonymsList.Count == 0)
            {
                return;
            }

            if (SynonymSelected == null && Tbl84SynonymsList.Count != 0)
            {
                SynonymSelected = SynonymItems.FirstOrDefault()!;
            }
        }
    }
    public void RefreshGeographicItems()
    {
        GeographicItems.Clear();
        if (Tbl87GeographicsList != null)
        {
            foreach (var item in Tbl87GeographicsList)
            {
                GeographicItems.Add(item);
            }

            if (Tbl87GeographicsList.Count == 0)
            {
                return;
            }

            if (GeographicSelected == null && Tbl87GeographicsList.Count != 0)
            {
                GeographicSelected = GeographicItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties
}
