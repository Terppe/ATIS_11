using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl12SubphylumViewModel Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Database;

public partial class Tbl12SubphylumViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl12Subphylum> SubphylumItems { get; } = [];

    public ObservableCollection<Tbl18Superclass> SuperclassItems { get; } = [];
    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    public string Search;
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl12SubphylumViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl06PhylumsAllList ??= [];
        Tbl06PhylumsAllList = [.. _dataService.GetPhylumAsync()];
        Tbl12SubphylumsAllList ??= [];
        Tbl12SubphylumsAllList = [.. _dataService.GetSubphylumAsync()];
        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];
    }
    public Tbl12SubphylumViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Subphylum]

    public async Task AddSubphylumAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditSubphylum = true;
            IsNewSubphylum = true;

            Tbl06PhylumsAllList ??= [];
            Tbl06PhylumsAllList = [.. _dataService.GetPhylumAsync()];

            var phylumId = Tbl06PhylumsAllList.First().PhylumId;

            Tbl12SubphylumsList ??= [];
            Tbl12SubphylumsList.Insert(index: 0,
                item: new Tbl12Subphylum { PhylumId = phylumId });
        });
    }

    public async Task SaveSubphylumAsync(Tbl12Subphylum viewModelSubphylumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSubphylum)
            {
                IsNewSubphylum = false;
            }
        });

        Tbl12Subphylum result = null;
        try
        {
            if (IsModifiedSubphylum)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSubphylumAsync(viewModelSubphylumSelected);
                    IsModifiedSubphylum = false;
                });
            }
            else
            {
                IsModifiedSubphylum = true;
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
            IsInEditSubphylum = false;
            IsModifiedSubphylum = false;
            IsExistingSubphylum = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteSubphylumAsync(Tbl12Subphylum viewModelSubphylumSelected)
    {
        Tbl12Subphylum result = null;
        try
        {
            var deleteSubphylumId = viewModelSubphylumSelected.SubphylumId;
            await DeleteSubphylum(deleteSubphylumId);

            result = viewModelSubphylumSelected;
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
            IsInEditSubphylum = false;
            IsModifiedSubphylum = false;
            IsExistingSubphylum = true;
            IsDeletingSubphylum = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteSubphylum(int subphylumid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var arrPlSpecies = new List<Tbl72PlSpecies>();
            var arrFiSpecies = new List<Tbl69FiSpecies>();
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

            var subphylum = await Context.Tbl12Subphylums.FirstOrDefaultAsync(b => b.SubphylumId == subphylumid);
            if (subphylum != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl18Superclasses(subphylumid, arrSuperclass, arrClass, arrSubclass,
                    arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily,
                    arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrFiSpecies,
                    arrPlSpecies);

                // remove Tbl72PlSpeciesses
                await RemoveExtras.RemoveArrPlSpeciesByPlSpeciesId(arrPlSpecies);
                // remove Tbl69FiSpeciesses
                await RemoveExtras.RemoveArrFiSpeciesByFiSpeciesId(arrFiSpecies);
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

                await RemoveExtras.RemoveReferencesBySubphylumId(subphylumid);
                await RemoveExtras.RemoveCommentsBySubphylumId(subphylumid);
                Context.Tbl12Subphylums.Remove(subphylum);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Subphylum ]  

    #region [Methods Reference]

    public async Task AddReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90ExpertsAllList ??= [];
            Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, SubphylumId = SubphylumSelected.SubphylumId });
        });
    }

    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, SubphylumId = SubphylumSelected.SubphylumId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, SubphylumId = SubphylumSelected.SubphylumId });
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
            var reference = await Context.Tbl90References.FirstOrDefaultAsync(re => re.ReferenceId == referenceid);

            Context.Tbl90References.Remove(reference);
            await Context.SaveChangesAsync();
        });
    }

    #endregion [Methods Reference]

    #region [Methods Comment]

    public async Task AddComment()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            var id = SubphylumSelected.SubphylumId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { SubphylumId = id });
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
            var comment = await Context.Tbl93Comments.FirstOrDefaultAsync(co => co.CommentId == commentid);

            Context.Tbl93Comments.Remove(comment);
            await Context.SaveChangesAsync();
        });
    }

    #endregion [Methods Comment] 

    #region [Methods Superclass]             

    public async Task AddSuperclassAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl12SubphylumsAllList ??= [];
            Tbl12SubphylumsAllList = [.. _dataService.GetSubphylumAsync()];

            var id = SubphylumSelected.SubphylumId;

            Tbl18SuperclassesList ??= [];
            Tbl18SuperclassesList.Insert(index: 0, item: new Tbl18Superclass { SubphylumId = id });
        });
    }

    public async Task SaveSuperclassAsync(Tbl18Superclass viewModelSuperclassSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSuperclass)
            {
                IsNewSuperclass = false;
            }
        });

        Tbl18Superclass result = null;
        try
        {
            if (IsModifiedSuperclass)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSuperclassAsync(viewModelSuperclassSelected);
                    IsModifiedSuperclass = false;
                });
            }
            else
            {
                IsModifiedSuperclass = true;
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
            IsInEditSuperclass = false;
            IsModifiedSuperclass = false;
            IsExistingSuperclass = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Superclass]   



    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------                 
    public bool IsModifiedSubphylum { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedSuperclass { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => SubphylumItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingSubphylum;
    public bool IsDeletingSubphylum
    {
        get => _isDeletingSubphylum;
        set => SetProperty(ref _isDeletingSubphylum, value);
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
    private bool _isDeletingSuperclass;
    public bool IsDeletingSuperclass
    {
        get => _isDeletingSuperclass;
        set => SetProperty(ref _isDeletingSuperclass, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewSubphylum;
    public bool IsNewSubphylum
    {
        get => _isNewSubphylum;
        set => SetProperty(ref _isNewSubphylum, value);
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
    private bool _isNewSuperclass;
    public bool IsNewSuperclass
    {
        get => _isNewSuperclass;
        set => SetProperty(ref _isNewSuperclass, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingSubphylum;
    public bool IsExistingSubphylum
    {
        get => _isExistingSubphylum;
        set => SetProperty(ref _isExistingSubphylum, value);
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
    private bool _isExistingSuperclass;
    public bool IsExistingSuperclass
    {
        get => _isExistingSuperclass;
        set => SetProperty(ref _isExistingSuperclass, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditSubphylum = false;
    public bool IsInEditSubphylum
    {
        get => _isInEditSubphylum;
        set => SetProperty(ref _isInEditSubphylum, value);
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
    private bool _isInEditSuperclass = false;
    public bool IsInEditSuperclass
    {
        get => _isInEditSuperclass;
        set => SetProperty(ref _isInEditSuperclass, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditSubphylum()
    {
        IsInEditSubphylum = true;
        IsModifiedSubphylum = false;
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
    public void StartEditSuperclass()
    {
        IsInEditSuperclass = true;
        IsModifiedSuperclass = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditSubphylum() => await SaveSubphylumAsync(SubphylumSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditSuperclass() => await SaveSuperclassAsync(SuperclassSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshSubphylum => !IsModifiedSubphylum && IsExistingSubphylum;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshSuperclass => !IsModifiedSuperclass && IsExistingSuperclass;
    //--------------CanRevert-------------------------------------
    public bool CanRevertSubphylum => IsModifiedSubphylum && IsExistingSubphylum;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertSuperclass => IsModifiedSuperclass && IsExistingSuperclass;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl12Subphylum _subphylumSelected;
    public Tbl12Subphylum SubphylumSelected
    {
         get => _subphylumSelected;
         set => SetProperty(ref _subphylumSelected, value);
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
    private Tbl18Superclass _superclassSelected;
    public Tbl18Superclass SuperclassSelected
    {
         get => _superclassSelected;
         set => SetProperty(ref _superclassSelected, value);
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

    private int _subphylumDataSetCount;

    public int SubphylumDataSetCount
    {
        get => _subphylumDataSetCount;
        set
        {
            _subphylumDataSetCount = value; OnPropertyChanged();
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
    private int _superclassDataSetCount;
    public int SuperclassDataSetCount
    {
        get => _superclassDataSetCount;
        set
        {
            _superclassDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------
    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsAllList;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsAllList
    {
        get => _tbl06PhylumsAllList;
        set
        {
            _tbl06PhylumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl12Subphylum> _tbl12SubphylumsList;
    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsList
    {
        get => _tbl12SubphylumsList;
        set
        {
            _tbl12SubphylumsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl12Subphylum> _tbl12SubphylumsAllList;
    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsAllList
    {
        get => _tbl12SubphylumsAllList;
        set
        {
            _tbl12SubphylumsAllList = value; OnPropertyChanged();
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
            _tbl90ReferenceAuthorsList = value; OnPropertyChanged(nameof(Tbl90ReferenceAuthorsList));
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceSourcesList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceSourcesList
    {
        get => _tbl90ReferenceSourcesList;
        set
        {
            _tbl90ReferenceSourcesList = value; OnPropertyChanged(nameof(Tbl90ReferenceSourcesList));
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceExpertsList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceExpertsList
    {
        get => _tbl90ReferenceExpertsList;
        set
        {
            _tbl90ReferenceExpertsList = value; OnPropertyChanged(nameof(Tbl90ReferenceExpertsList));
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

    private ObservableCollection<Tbl18Superclass> _tbl18SuperclassList;

    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesList
    {
        get => _tbl18SuperclassList;
        set
        {
            _tbl18SuperclassList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshSubphylumItems()
    {
        SubphylumItems.Clear();
        if (Tbl12SubphylumsList != null)
        {
            foreach (var item in Tbl12SubphylumsList)
            {
                SubphylumItems.Add(item);
            }

            if (Tbl12SubphylumsList.Count == 0)
            {
                return;
            }

            if (SubphylumSelected == null && Tbl12SubphylumsList.Count != 0)
            {
                SubphylumSelected = SubphylumItems.FirstOrDefault()!;
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
    public void RefreshSuperclassItems()
    {
        SuperclassItems.Clear();
        if (Tbl18SuperclassesList != null)
        {
            foreach (var item in Tbl18SuperclassesList)
            {
                SuperclassItems.Add(item);
            }

            if (Tbl18SuperclassesList.Count == 0)
            {
                return;
            }

            if (SuperclassSelected == null && Tbl18SuperclassesList.Count != 0)
            {
                SuperclassSelected = SuperclassItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties 

}
