using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl39InfraordoViewModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Database;

public partial class Tbl39InfraordoViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl39Infraordo> InfraordoItems { get; } = [];

    public ObservableCollection<Tbl42Superfamily> SuperfamilyItems { get; } = [];
    public ObservableCollection<Tbl36Subordo> SubordoItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    public string Search;
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl39InfraordoViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl36SubordosAllList ??= [];
        Tbl36SubordosAllList = [.. _dataService.GetSubordoAsync()];
        Tbl39InfraordosAllList ??= [];
        Tbl39InfraordosAllList = [.. _dataService.GetInfraordoAsync()];
        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];
    }
    public Tbl39InfraordoViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Infraordo]

    public async Task AddInfraordoAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditInfraordo = true;
            IsNewInfraordo = true;

            Tbl36SubordosAllList ??= [];
            Tbl36SubordosAllList = [.. _dataService.GetSubordoAsync()];

            var subordoId = Tbl36SubordosAllList.First().SubordoId;

            Tbl39InfraordosList ??= [];
            Tbl39InfraordosList.Insert(index: 0,
                item: new Tbl39Infraordo { SubordoId = subordoId });
        });
    }

    public async Task SaveInfraordoAsync(Tbl39Infraordo viewModelInfraordoSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewInfraordo)
            {
                IsNewInfraordo = false;
            }
        });

        Tbl39Infraordo result = null;
        try
        {
            if (IsModifiedInfraordo)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertInfraordoAsync(viewModelInfraordoSelected);
                    IsModifiedInfraordo = false;
                });
            }
            else
            {
                IsModifiedInfraordo = true;
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
            IsInEditInfraordo = false;
            IsModifiedInfraordo = false;
            IsExistingInfraordo = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteInfraordoAsync(Tbl39Infraordo viewModelInfraordoSelected)
    {
        Tbl39Infraordo result = null;
        try
        {
            var deleteInfraordoId = viewModelInfraordoSelected.InfraordoId;
            await DeleteInfraordo(deleteInfraordoId);

            result = viewModelInfraordoSelected;
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
            IsInEditInfraordo = false;
            IsModifiedInfraordo = false;
            IsExistingInfraordo = true;
            IsDeletingInfraordo = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }
    private async Task DeleteInfraordo(int infraordoid)
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
            var infraordo = await Context.Tbl39Infraordos.FirstOrDefaultAsync(b => b.InfraordoId == infraordoid);
            if (infraordo != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl42Superfamilies(infraordoid, arrSuperfamily, arrFamily, arrSubfamily,
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

                await RemoveExtras.RemoveReferencesByInfraordoId(infraordoid);
                await RemoveExtras.RemoveCommentsByInfraordoId(infraordoid);
                Context.Tbl39Infraordos.Remove(infraordo);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Infraordo ]  

    #region [Methods Reference]

    public async Task AddReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90ExpertsAllList ??= [];
            Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, InfraordoId = InfraordoSelected.InfraordoId });
        });
    }

    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, InfraordoId = InfraordoSelected.InfraordoId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, InfraordoId = InfraordoSelected.InfraordoId });
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
            var id = InfraordoSelected.InfraordoId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { InfraordoId = id });
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

    #region [Methods Superfamily]             

    public async Task AddSuperfamilyAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl39InfraordosAllList ??= [];
            Tbl39InfraordosAllList = [.. _dataService.GetInfraordoAsync()];

            var id = InfraordoSelected.InfraordoId;

            Tbl42SuperfamiliesList ??= [];
            Tbl42SuperfamiliesList.Insert(index: 0, item: new Tbl42Superfamily { InfraordoId = id });
        });
    }

    public async Task SaveSuperfamilyAsync(Tbl42Superfamily viewModelSuperfamilySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSuperfamily)
            {
                IsNewSuperfamily = false;
            }
        });

        Tbl42Superfamily result = null;
        try
        {
            if (IsModifiedSuperfamily)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSuperfamilyAsync(viewModelSuperfamilySelected);
                    IsModifiedSuperfamily = false;
                });
            }
            else
            {
                IsModifiedSuperfamily = true;
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
            IsInEditSuperfamily = false;
            IsModifiedSuperfamily = false;
            IsExistingSuperfamily = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Superfamily]   



    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------                 
    public bool IsModifiedInfraordo { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedSuperfamily { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => InfraordoItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingInfraordo;
    public bool IsDeletingInfraordo
    {
        get => _isDeletingInfraordo;
        set => SetProperty(ref _isDeletingInfraordo, value);
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
    private bool _isDeletingSuperfamily;
    public bool IsDeletingSuperfamily
    {
        get => _isDeletingSuperfamily;
        set => SetProperty(ref _isDeletingSuperfamily, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewInfraordo;
    public bool IsNewInfraordo
    {
        get => _isNewInfraordo;
        set => SetProperty(ref _isNewInfraordo, value);
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
    private bool _isNewSuperfamily;
    public bool IsNewSuperfamily
    {
        get => _isNewSuperfamily;
        set => SetProperty(ref _isNewSuperfamily, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingInfraordo;
    public bool IsExistingInfraordo
    {
        get => _isExistingInfraordo;
        set => SetProperty(ref _isExistingInfraordo, value);
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
    private bool _isExistingSuperfamily;
    public bool IsExistingSuperfamily
    {
        get => _isExistingSuperfamily;
        set => SetProperty(ref _isExistingSuperfamily, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditInfraordo = false;
    public bool IsInEditInfraordo
    {
        get => _isInEditInfraordo;
        set => SetProperty(ref _isInEditInfraordo, value);
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
    private bool _isInEditSuperfamily = false;
    public bool IsInEditSuperfamily
    {
        get => _isInEditSuperfamily;
        set => SetProperty(ref _isInEditSuperfamily, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditInfraordo()
    {
        IsInEditInfraordo = true;
        IsModifiedInfraordo = false;
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
    public void StartEditSuperfamily()
    {
        IsInEditSuperfamily = true;
        IsModifiedSuperfamily = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditInfraordo() => await SaveInfraordoAsync(InfraordoSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditSuperfamily() => await SaveSuperfamilyAsync(SuperfamilySelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshInfraordo => !IsModifiedInfraordo && IsExistingInfraordo;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshSuperfamily => !IsModifiedSuperfamily && IsExistingSuperfamily;
    //--------------CanRevert-------------------------------------
    public bool CanRevertInfraordo => IsModifiedInfraordo && IsExistingInfraordo;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertSuperfamily => IsModifiedSuperfamily && IsExistingSuperfamily;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl39Infraordo _infraordoSelected;
    public Tbl39Infraordo InfraordoSelected
    {
         get => _infraordoSelected;
         set => SetProperty(ref _infraordoSelected, value);
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
    private Tbl42Superfamily _superfamilySelected;
    public Tbl42Superfamily SuperfamilySelected
    {
         get => _superfamilySelected;
         set => SetProperty(ref _superfamilySelected, value);
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

    private int _infraordoDataSetCount;

    public int InfraordoDataSetCount
    {
        get => _infraordoDataSetCount;
        set
        {
            _infraordoDataSetCount = value; OnPropertyChanged();
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
    private int _superfamilyDataSetCount;
    public int SuperfamilyDataSetCount
    {
        get => _superfamilyDataSetCount;
        set
        {
            _superfamilyDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------
    private ObservableCollection<Tbl36Subordo> _tbl36SubordosAllList;
    public ObservableCollection<Tbl36Subordo> Tbl36SubordosAllList
    {
        get => _tbl36SubordosAllList;
        set
        {
            _tbl36SubordosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl39Infraordo> _tbl39InfraordosList;
    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosList
    {
        get => _tbl39InfraordosList;
        set
        {
            _tbl39InfraordosList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl39Infraordo> _tbl39InfraordosAllList;
    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosAllList
    {
        get => _tbl39InfraordosAllList;
        set
        {
            _tbl39InfraordosAllList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl42Superfamily> _tbl42SuperfamilyList;

    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesList
    {
        get => _tbl42SuperfamilyList;
        set
        {
            _tbl42SuperfamilyList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshInfraordoItems()
    {
        InfraordoItems.Clear();
        if (Tbl39InfraordosList != null)
        {
            foreach (var item in Tbl39InfraordosList)
            {
                InfraordoItems.Add(item);
            }

            if (Tbl39InfraordosList.Count == 0)
            {
                return;
            }

            if (InfraordoSelected == null && Tbl39InfraordosList.Count != 0)
            {
                InfraordoSelected = InfraordoItems.FirstOrDefault()!;
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
    public void RefreshSuperfamilyItems()
    {
        SuperfamilyItems.Clear();
        if (Tbl42SuperfamiliesList != null)
        {
            foreach (var item in Tbl42SuperfamiliesList)
            {
                SuperfamilyItems.Add(item);
            }

            if (Tbl42SuperfamiliesList.Count == 0)
            {
                return;
            }

            if (SuperfamilySelected == null && Tbl42SuperfamiliesList.Count != 0)
            {
                SuperfamilySelected = SuperfamilyItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties 

}
