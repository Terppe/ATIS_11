using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl03RegnumViewModel Skriptdatum:  30.12.2024  12:32      

namespace ATIS.Views.Database;

public partial class Tbl03RegnumViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _data;

    private readonly IDataService _dataService;
    public ObservableCollection<Tbl03Regnum> RegnumItems { get; } = [];

    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = [];
 //   public ObservableCollection<Tbl09Division> DivisionItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl03RegnumViewModel(IDataService data)
    {
        _data = data;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl03RegnumsAllList ??= [];
        Tbl03RegnumsAllList = _dataService.GetRegnumAsync();
        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = new ObservableCollection<Tbl90RefExpert>(_dataService.GetRefExpertAsyncAlt());
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = new ObservableCollection<Tbl90RefSource>(_dataService.GetRefSourceAsync());
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = new ObservableCollection<Tbl90RefAuthor>(_dataService.GetRefAuthorAsync());

    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Regnum]

    public async Task AddRegnumAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditRegnum = true;
            IsNewRegnum = true;

            Tbl03RegnumsList ??= [];
            Tbl03RegnumsList.Insert(index: 0, item: new Tbl03Regnum { });
        });
    }

    public async Task SaveRegnumAsync(Tbl03Regnum viewModelRegnumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewRegnum)
            {
                IsNewRegnum = false;
            }
        });

        Tbl03Regnum result = null;
        try
        {
            if (IsModifiedRegnum)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertRegnumAsync(viewModelRegnumSelected);
                    IsModifiedRegnum = false;
                });
            }
            else
            {
                IsModifiedRegnum = true;
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
            IsInEditRegnum = false;
            IsModifiedRegnum = false;
            IsExistingRegnum = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteRegnumAsync(Tbl03Regnum viewModelRegnumSelected)
    {
        Tbl03Regnum result = null;
        try
        {
            var deleteRegnumId = viewModelRegnumSelected.RegnumId;
            await DeleteRegnum(deleteRegnumId);

            result = viewModelRegnumSelected;
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
            IsInEditRegnum = false;
            IsModifiedRegnum = false;
            IsExistingRegnum = true;
            IsDeletingRegnum = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async Task DeleteRegnum(int regnumid)
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
            var arrOrdo = new List<Tbl33Ordo>();
            var arrLegio = new List<Tbl30Legio>();
            var arrInfraclass = new List<Tbl27Infraclass>();
            var arrSubclass = new List<Tbl24Subclass>();
            var arrClass = new List<Tbl21Class>();
            var arrSuperclass = new List<Tbl18Superclass>();
            var arrSubdivision = new List<Tbl15Subdivision>();
            var arrSubphylum = new List<Tbl12Subphylum>();
            var arrDivision = new List<Tbl09Division>();
            var arrPhylum = new List<Tbl06Phylum>();

            var regnum = await AppData.Data.Tbl03Regnums.FirstOrDefaultAsync(b => b.RegnumId == regnumid);
            if (regnum != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl06Phylums(regnumid, arrPhylum, arrSubphylum, arrSuperclass, arrClass,
                    arrSubclass, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily,
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
                // remove Tbl33Ordos
                await RemoveExtras.RemoveArrOrdoByOrdoId(arrOrdo);
                // remove Tbl30Legios
                await RemoveExtras.RemoveArrLegioByLegioId(arrLegio);
                // remove Tbl27Infraclasses
                await RemoveExtras.RemoveArrInfraclassByInfraclassId(arrInfraclass);
                // remove Tbl24Subclasses
                await RemoveExtras.RemoveArrSubclassBySubclassId(arrSubclass);
                // remove Tbl21Classes
                await RemoveExtras.RemoveArrClassByClassId(arrClass);
                // remove Tbl18Superclasses
                await RemoveExtras.RemoveArrSuperclassBySuperclassId(arrSuperclass);
                // remove Tbl12Subphylums
                await RemoveExtras.RemoveArrSubphylumBySubphylumId(arrSubphylum);
                // remove Tbl06Phylums
                await RemoveExtras.RemoveArrPhylumByPhylumId(arrPhylum);

                await RemoveExtras.RemoveReferencesByRegnumId(regnumid);
                await RemoveExtras.RemoveCommentsByRegnumId(regnumid);
                //AppData.Data.Tbl03Regnums.Remove(regnum);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Regnum ]  

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
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, RegnumId = RegnumSelected.RegnumId });
        });
    }

    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, RegnumId = RegnumSelected.RegnumId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditReference = true;
            IsNewReference = true;

            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, RegnumId = RegnumSelected.RegnumId });
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

            //Context.Tbl90References.Remove(reference);
            //await Context.SaveChangesAsync();
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

            var id = RegnumSelected.RegnumId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { RegnumId = id });
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

    #region [Methods Phylum]             

    public async Task SavePhylumAsync(Tbl06Phylum viewModelPhylumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewPhylum)
            {
                IsNewPhylum = false;
            }
        });
        Tbl06Phylum result = null;
        try
        {
            if (IsModifiedPhylum)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertPhylumAsync(viewModelPhylumSelected);
                    IsModifiedPhylum = false;
                });
            }
            else
            {
                IsModifiedPhylum = true;
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
            IsInEditPhylum = false;
            IsModifiedPhylum = false;
            IsExistingPhylum = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Phylum]   


    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------                 
    public bool IsModifiedRegnum { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedPhylum { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => RegnumItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingRegnum;
    public bool IsDeletingRegnum
    {
        get => _isDeletingRegnum;
        set => SetProperty(ref _isDeletingRegnum, value);
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
    private bool _isDeletingPhylum;
    public bool IsDeletingPhylum
    {
        get => _isDeletingPhylum;
        set => SetProperty(ref _isDeletingPhylum, value);
    }

    //------------IsNew---------------------------------------
    private bool _isNewRegnum;
    public bool IsNewRegnum
    {
        get => _isNewRegnum;
        set => SetProperty(ref _isNewRegnum, value);
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
    private bool _isNewPhylum;
    public bool IsNewPhylum
    {
        get => _isNewPhylum;
        set => SetProperty(ref _isNewPhylum, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingRegnum;
    public bool IsExistingRegnum
    {
        get => _isExistingRegnum;
        set => SetProperty(ref _isExistingRegnum, value);
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
    private bool _isExistingPhylum;
    public bool IsExistingPhylum
    {
        get => _isExistingPhylum;
        set => SetProperty(ref _isExistingPhylum, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditRegnum;
    public bool IsInEditRegnum
    {
        get => _isInEditRegnum;
        set => SetProperty(ref _isInEditRegnum, value);
    }

    private bool _isInEditReference;
    public bool IsInEditReference
    {
        get => _isInEditReference;
        set => SetProperty(ref _isInEditReference, value);
    }
    private bool _isInEditReferenceExpert;
    public bool IsInEditReferenceExpert
    {
        get => _isInEditReferenceExpert;
        set => SetProperty(ref _isInEditReferenceExpert, value);
    }
    private bool _isInEditReferenceSource;
    public bool IsInEditReferenceSource
    {
        get => _isInEditReferenceSource;
        set => SetProperty(ref _isInEditReferenceSource, value);
    }
    private bool _isInEditReferenceAuthor;
    public bool IsInEditReferenceAuthor
    {
        get => _isInEditReferenceAuthor;
        set => SetProperty(ref _isInEditReferenceAuthor, value);
    }
    private bool _isInEditComment;
    public bool IsInEditComment
    {
        get => _isInEditComment;
        set => SetProperty(ref _isInEditComment, value);
    }
    private bool _isInEditPhylum;
    public bool IsInEditPhylum
    {
        get => _isInEditPhylum;
        set => SetProperty(ref _isInEditPhylum, value);
    }

    //----------StartEdit-----------------------------------------
    public void StartEditRegnum()
    {
        IsInEditRegnum = true;
        IsModifiedRegnum = false;
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
    public void StartEditPhylum()
    {
        IsInEditPhylum = true;
        IsModifiedPhylum = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditRegnum() => await SaveRegnumAsync(RegnumSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditPhylum() => await SavePhylumAsync(PhylumSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshRegnum => !IsModifiedRegnum && IsExistingRegnum;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshPhylum => !IsModifiedPhylum && IsExistingPhylum;
    //--------------CanRevert-------------------------------------
    public bool CanRevertRegnum => IsModifiedRegnum && IsExistingRegnum;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertPhylum => IsModifiedPhylum && IsExistingPhylum;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl03Regnum _regnumSelected;

    public Tbl03Regnum RegnumSelected
    {
         get => _regnumSelected;
         set => SetProperty(ref _regnumSelected, value);
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
    private Tbl06Phylum _phylumSelected;

    public Tbl06Phylum PhylumSelected
    {
         get => _phylumSelected;
         set => SetProperty(ref _phylumSelected, value);
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

    private int _regnumDataSetCount;
    public int RegnumDataSetCount
    {
        get => _regnumDataSetCount;
        set
        {
            _regnumDataSetCount = value; OnPropertyChanged();
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

    private int _phylumDataSetCount;

    public int PhylumDataSetCount
    {
        get => _phylumDataSetCount;
        set
        {
            _phylumDataSetCount = value; OnPropertyChanged();
        }
    }

    //---------------------------------------------------
    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsList;

    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsList
    {
        get => _tbl03RegnumsList;
        set
        {
            _tbl03RegnumsList = value; OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsAllList;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get => _tbl03RegnumsAllList;
        set
        {
            _tbl03RegnumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsList;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsList
    {
        get => _tbl06PhylumsList;
        set
        {
            _tbl06PhylumsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsAllList;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsAllList
    {
        get => _tbl06PhylumsAllList;
        set
        {
            _tbl06PhylumsAllList = value; OnPropertyChanged();
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


    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshRegnumItems()
    {
        RegnumItems.Clear();
        if (Tbl03RegnumsList != null)
        {
            foreach (var item in Tbl03RegnumsList)
            {
                RegnumItems.Add(item);
            }

            if (Tbl03RegnumsList.Count == 0)
            {
                return;
            }

            if (RegnumSelected == null && Tbl03RegnumsList.Count != 0)
            {
                RegnumSelected = RegnumItems.FirstOrDefault()!;
            }
        }
    }
    public void RefreshPhylumItems()
    {
        PhylumItems.Clear();
        if (Tbl06PhylumsList != null)
        {
            foreach (var item in Tbl06PhylumsList)
            {
                PhylumItems.Add(item);
            }

            if (Tbl06PhylumsList.Count == 0)
            {
                return;
            }

            if (PhylumSelected == null && Tbl06PhylumsList.Count != 0)
            {
                PhylumSelected = PhylumItems.FirstOrDefault()!;
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

    #endregion Refresh Methods

    #endregion All Properties 

}
