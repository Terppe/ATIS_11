using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;
using static WinUI3Localizer.LanguageDictionary;

//    Tbl33OrdoViewModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Database;

public partial class Tbl33OrdoAltViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl33Ordo> OrdoItems { get; } = [];

    public ObservableCollection<Tbl36Subordo> SubordoItems { get; } = [];
    public ObservableCollection<Tbl30Legio> LegioItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl33OrdoAltViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl30LegiosAllList ??= [];
        Tbl30LegiosAllList = [.. _dataService.GetLegioAsync()];
        Tbl33OrdosAllList ??= [];
        Tbl33OrdosAllList = [.. _dataService.GetOrdoAsync()];
        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsyncAlt()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsyncAlt()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsyncAlt()];
    }
    public Tbl33OrdoAltViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    

    #region [Methods Ordo]

    public async Task AddOrdoAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditOrdo = true;
            IsNewOrdo = true;

            Tbl30LegiosAllList ??= [];
            Tbl30LegiosAllList = [.. _dataService.GetLegioAsync()];

            var legioId = Tbl30LegiosAllList.First().LegioId;

            Tbl33OrdosList ??= [];
            Tbl33OrdosList.Insert(index: 0, item: new Tbl33Ordo { LegioId = legioId });
        });
    }

    public async Task SaveOrdoAsync(Tbl33Ordo viewModelOrdoSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewOrdo)
            {
                IsNewOrdo = false;
            }
        });

        Tbl33Ordo result = null;
        try
        {
            if (IsModifiedOrdo)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertOrdoAsync(viewModelOrdoSelected);
                    IsModifiedOrdo = false;
                });
            }
            else
            {
                IsModifiedOrdo = true;
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
            IsInEditOrdo = false;
            IsModifiedOrdo = false;
            IsExistingOrdo = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteOrdoAsync(Tbl33Ordo viewModelOrdoSelected)
    {
        Tbl33Ordo result = null;
        try
        {
            var deleteOrdoId = viewModelOrdoSelected.OrdoId;
            await DeleteOrdo(deleteOrdoId);

            result = viewModelOrdoSelected;
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
            IsInEditOrdo = false;
            IsModifiedOrdo = false;
            IsExistingOrdo = true;
            IsDeletingOrdo = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteOrdo(int ordoid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var arrSpecies = new List<Tbl75Species>();
            var arrGenus = new List<Tbl66Genus>();
            var arrInfratribus = new List<Tbl63Infratribus>();
            var arrSubtribus = new List<Tbl60Subtribus>();
            var arrTribus = new List<Tbl57Tribus>();
            var arrSupertribus = new List<Tbl54Supertribus>();
            var arrInfrafamily = new List<Tbl51Infrafamily>();
            var arrSubfamily = new List<Tbl48Subfamily>();
            var arrFamily = new List<Tbl45Family>();
            var arrSuperfamily = new List<Tbl42Superfamily>();
            var arrInfraordo = new List<Tbl39Infraordo>();
            var arrSubordo = new List<Tbl36Subordo>();
            var ordo = await AppData.Data.Tbl33Ordos.FirstOrDefaultAsync(b => b.OrdoId == ordoid);
            if (ordo != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl36Subordos(ordoid, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily,
                    arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus,
                    arrSpecies);

                // remove Tbl75Speciesses
                await RemoveExtras.RemoveArrSpeciesBySpeciesId(arrSpecies);
                // remove Tbl66Genusses
                await RemoveExtras.RemoveArrGenusByGenusId(arrGenus);
                // remove Tbl63Infratribusses
                await RemoveExtras.RemoveArrInfratribusByInfratribusId(arrInfratribus);
                // remove Tbl60Subtribusses
                await RemoveExtras.RemoveArrSubtribusBySubtribusId(arrSubtribus);
                // remove Tbl57Tribusses
                await RemoveExtras.RemoveArrTribusByTribusId(arrTribus);
                // remove Tbl54Supertribusses
                await RemoveExtras.RemoveArrSupertribusBySupertribusId(arrSupertribus);
                // remove Tbl51Infrafamilies
                await RemoveExtras.RemoveArrInfrafamilyByInfrafamilyId(arrInfrafamily);
                // remove Tbl48Subfamilies
                await RemoveExtras.RemoveArrSubfamilyBySubfamilyId(arrSubfamily);
                // remove Tbl45Families
                await RemoveExtras.RemoveArrFamilyByFamilyId(arrFamily);
                // remove Tbl42Superfamilies
                await RemoveExtras.RemoveArrSuperfamilyBySuperfamilyId(arrSuperfamily);
                // remove Tbl39Infraordos
                await RemoveExtras.RemoveArrInfraordoByInfraordoId(arrInfraordo);
                // remove Tbl36Subordos
                await RemoveExtras.RemoveArrSubordoBySubordoId(arrSubordo);

                await RemoveExtras.RemoveReferencesByOrdoId(ordoid);
                await RemoveExtras.RemoveCommentsByOrdoId(ordoid);
                //AppData.Data.Tbl33Ordos.Remove(ordo);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Ordo ]  

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
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, OrdoId = OrdoSelected.OrdoId });
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
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, OrdoId = OrdoSelected.OrdoId });
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
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, OrdoId = OrdoSelected.OrdoId });
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

            var id = OrdoSelected.OrdoId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { OrdoId = id });
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

    #region [Methods Subordo]             

    public async Task SaveSubordoAsync(Tbl36Subordo viewModelSubordoSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSubordo)
            {
                IsNewSubordo = false;
            }
        });

        Tbl36Subordo result = null;
        try
        {
            if (IsModifiedSubordo)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSubordoAsync(viewModelSubordoSelected);
                    IsModifiedSubordo = false;
                });
            }
            else
            {
                IsModifiedSubordo = true;
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
            IsInEditSubordo = false;
            IsModifiedSubordo = false;
            IsExistingSubordo = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Subordo]   



    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------                 
    public bool IsModifiedOrdo { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedSubordo { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => OrdoItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingOrdo;
    public bool IsDeletingOrdo
    {
        get => _isDeletingOrdo;
        set => SetProperty(ref _isDeletingOrdo, value);
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
    private bool _isDeletingSubordo;
    public bool IsDeletingSubordo
    {
        get => _isDeletingSubordo;
        set => SetProperty(ref _isDeletingSubordo, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewOrdo;
    public bool IsNewOrdo
    {
        get => _isNewOrdo;
        set => SetProperty(ref _isNewOrdo, value);
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
    private bool _isNewSubordo;
    public bool IsNewSubordo
    {
        get => _isNewSubordo;
        set => SetProperty(ref _isNewSubordo, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingOrdo;
    public bool IsExistingOrdo
    {
        get => _isExistingOrdo;
        set => SetProperty(ref _isExistingOrdo, value);
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
    private bool _isExistingSubordo;
    public bool IsExistingSubordo
    {
        get => _isExistingSubordo;
        set => SetProperty(ref _isExistingSubordo, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditOrdo = false;
    public bool IsInEditOrdo
    {
        get => _isInEditOrdo;
        set => SetProperty(ref _isInEditOrdo, value);
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
    private bool _isInEditSubordo = false;
    public bool IsInEditSubordo
    {
        get => _isInEditSubordo;
        set => SetProperty(ref _isInEditSubordo, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditOrdo()
    {
        IsInEditOrdo = true;
        IsModifiedOrdo = false;
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
    public void StartEditSubordo()
    {
        IsInEditSubordo = true;
        IsModifiedSubordo = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditOrdo() => await SaveOrdoAsync(OrdoSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditSubordo() => await SaveSubordoAsync(SubordoSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshOrdo => !IsModifiedOrdo && IsExistingOrdo;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshSubordo => !IsModifiedSubordo && IsExistingSubordo;
    //--------------CanRevert-------------------------------------
    public bool CanRevertOrdo => IsModifiedOrdo && IsExistingOrdo;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertSubordo => IsModifiedSubordo && IsExistingSubordo;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl33Ordo _ordoSelected;
    public Tbl33Ordo OrdoSelected
    {
         get => _ordoSelected;
         set => SetProperty(ref _ordoSelected, value);
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
    private Tbl36Subordo _subordoSelected;
    public Tbl36Subordo SubordoSelected
    {
         get => _subordoSelected;
         set => SetProperty(ref _subordoSelected, value);
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

    private int _ordoDataSetCount;

    public int OrdoDataSetCount
    {
        get => _ordoDataSetCount;
        set
        {
            _ordoDataSetCount = value; OnPropertyChanged();
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
    private int _subordoDataSetCount;
    public int SubordoDataSetCount
    {
        get => _subordoDataSetCount;
        set
        {
            _subordoDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------
    private ObservableCollection<Tbl30Legio> _tbl30LegiosAllList;
    public ObservableCollection<Tbl30Legio> Tbl30LegiosAllList
    {
        get => _tbl30LegiosAllList;
        set
        {
            _tbl30LegiosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl33Ordo> _tbl33OrdosList;
    public ObservableCollection<Tbl33Ordo> Tbl33OrdosList
    {
        get => _tbl33OrdosList;
        set
        {
            _tbl33OrdosList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl33Ordo> _tbl33OrdosAllList;
    public ObservableCollection<Tbl33Ordo> Tbl33OrdosAllList
    {
        get => _tbl33OrdosAllList;
        set
        {
            _tbl33OrdosAllList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl36Subordo> _tbl36SubordoList;

    public ObservableCollection<Tbl36Subordo> Tbl36SubordosList
    {
        get => _tbl36SubordoList;
        set
        {
            _tbl36SubordoList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshOrdoItems()
    {
        OrdoItems.Clear();
        if (Tbl33OrdosList != null)
        {
            foreach (var item in Tbl33OrdosList)
            {
                OrdoItems.Add(item);
            }

            if (Tbl33OrdosList.Count == 0)
            {
                return;
            }

            if (OrdoSelected == null && Tbl33OrdosList.Count != 0)
            {
                OrdoSelected = OrdoItems.FirstOrDefault()!;
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
    public void RefreshSubordoItems()
    {
        SubordoItems.Clear();
        if (Tbl36SubordosList != null)
        {
            foreach (var item in Tbl36SubordosList)
            {
                SubordoItems.Add(item);
            }

            if (Tbl36SubordosList.Count == 0)
            {
                return;
            }

            if (SubordoSelected == null && Tbl36SubordosList.Count != 0)
            {
                SubordoSelected = SubordoItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties 

}
