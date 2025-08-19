
using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl57TribusViewModel Skriptdatum:  22.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl57TribusViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl57Tribus> TribusItems { get; } = [];

    public ObservableCollection<Tbl60Subtribus> SubtribusItems { get; } = [];
    public ObservableCollection<Tbl54Supertribus> SupertribusItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    public string Search;
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl57TribusViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl54SupertribussesAllList ??= [];
        Tbl54SupertribussesAllList = [.. _dataService.GetSupertribusAsync()];
        Tbl57TribussesAllList ??= [];
        Tbl57TribussesAllList = [.. _dataService.GetTribusAsync()];
        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];
    }
    public Tbl57TribusViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Tribus]

    public async Task AddTribusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditTribus = true;
            IsNewTribus = true;

            Tbl54SupertribussesAllList ??= [];
            Tbl54SupertribussesAllList = [.. _dataService.GetSupertribusAsync()];

            var supertribusId = Tbl54SupertribussesAllList.First().SupertribusId;

            Tbl57TribussesList ??= [];
            Tbl57TribussesList.Insert(index: 0,
                item: new Tbl57Tribus { SupertribusId = supertribusId });
        });
    }

    public async Task SaveTribusAsync(Tbl57Tribus viewModelTribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewTribus)
            {
                IsNewTribus = false;
            }
        });

        Tbl57Tribus result = null;
        try
        {
            if (IsModifiedTribus)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertTribusAsync(viewModelTribusSelected);
                    IsModifiedTribus = false;
                });
            }
            else
            {
                IsModifiedTribus = true;
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
            IsInEditTribus = false;
            IsModifiedTribus = false;
            IsExistingTribus = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteTribusAsync(Tbl57Tribus viewModelTribusSelected)
    {
        Tbl57Tribus result = null;
        try
        {
            var deleteTribusId = viewModelTribusSelected.TribusId;
            await DeleteTribus(deleteTribusId);

            result = viewModelTribusSelected;
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
            IsInEditTribus = false;
            IsModifiedTribus = false;
            IsExistingTribus = true;
            IsDeletingTribus = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteTribus(int tribusid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var arrPlSpecies = new List<Tbl72PlSpecies>();
            var arrFiSpecies = new List<Tbl69FiSpecies>();
            var arrGenus = new List<Tbl66Genus>();
            var arrInfratribus = new List<Tbl63Infratribus>();
            var arrSubtribus = new List<Tbl60Subtribus>();
            var tribus = await Context.Tbl57Tribusses.FirstOrDefaultAsync(b => b.TribusId == tribusid);
            if (tribus != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl60Subtribusses(tribusid, arrSubtribus, arrInfratribus, arrGenus, arrFiSpecies, arrPlSpecies);

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

                await RemoveExtras.RemoveReferencesByTribusId(tribusid);
                await RemoveExtras.RemoveCommentsByTribusId(tribusid);
                Context.Tbl57Tribusses.Remove(tribus);
                await Context.SaveChangesAsync();
            }
        });

    }

    #endregion [Methods Tribus ]  

    #region [Methods Reference]

    public async Task AddReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90ExpertsAllList ??= [];
            Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, TribusId = TribusSelected.TribusId });
        });
    }

    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, TribusId = TribusSelected.TribusId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, TribusId = TribusSelected.TribusId });
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
            var id = TribusSelected.TribusId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { TribusId = id });
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

    #region [Methods Subtribus]             

    public async Task AddSubtribusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl57TribussesAllList ??= [];
            Tbl57TribussesAllList = [.. _dataService.GetTribusAsync()];

            var id = TribusSelected.TribusId;

            Tbl60SubtribussesList ??= [];
            Tbl60SubtribussesList.Insert(index: 0, item: new Tbl60Subtribus { TribusId = id });
        });
    }

    public async Task SaveSubtribusAsync(Tbl60Subtribus viewModelSubtribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSubtribus)
            {
                IsNewSubtribus = false;
            }
        });

        Tbl60Subtribus result = null;
        try
        {
            if (IsModifiedSubtribus)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSubtribusAsync(viewModelSubtribusSelected);
                    IsModifiedSubtribus = false;
                });
            }
            else
            {
                IsModifiedSubtribus = true;
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
            IsInEditSubtribus = false;
            IsModifiedSubtribus = false;
            IsExistingSubtribus = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Subtribus]   



    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------                 
    public bool IsModifiedTribus { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedSubtribus { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => TribusItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingTribus;
    public bool IsDeletingTribus
    {
        get => _isDeletingTribus;
        set => SetProperty(ref _isDeletingTribus, value);
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
    private bool _isDeletingSubtribus;
    public bool IsDeletingSubtribus
    {
        get => _isDeletingSubtribus;
        set => SetProperty(ref _isDeletingSubtribus, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewTribus;
    public bool IsNewTribus
    {
        get => _isNewTribus;
        set => SetProperty(ref _isNewTribus, value);
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
    private bool _isNewSubtribus;
    public bool IsNewSubtribus
    {
        get => _isNewSubtribus;
        set => SetProperty(ref _isNewSubtribus, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingTribus;
    public bool IsExistingTribus
    {
        get => _isExistingTribus;
        set => SetProperty(ref _isExistingTribus, value);
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
    private bool _isExistingSubtribus;
    public bool IsExistingSubtribus
    {
        get => _isExistingSubtribus;
        set => SetProperty(ref _isExistingSubtribus, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditTribus = false;
    public bool IsInEditTribus
    {
        get => _isInEditTribus;
        set => SetProperty(ref _isInEditTribus, value);
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
    private bool _isInEditSubtribus = false;
    public bool IsInEditSubtribus
    {
        get => _isInEditSubtribus;
        set => SetProperty(ref _isInEditSubtribus, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditTribus()
    {
        IsInEditTribus = true;
        IsModifiedTribus = false;
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
    public void StartEditSubtribus()
    {
        IsInEditSubtribus = true;
        IsModifiedSubtribus = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditTribus() => await SaveTribusAsync(TribusSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditSubtribus() => await SaveSubtribusAsync(SubtribusSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshTribus => !IsModifiedTribus && IsExistingTribus;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshSubtribus => !IsModifiedSubtribus && IsExistingSubtribus;
    //--------------CanRevert-------------------------------------
    public bool CanRevertTribus => IsModifiedTribus && IsExistingTribus;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertSubtribus => IsModifiedSubtribus && IsExistingSubtribus;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl57Tribus _tribusSelected;
    public Tbl57Tribus TribusSelected
    {
         get => _tribusSelected;
         set => SetProperty(ref _tribusSelected, value);
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
    private Tbl60Subtribus _subtribusSelected;
    public Tbl60Subtribus SubtribusSelected
    {
         get => _subtribusSelected;
         set => SetProperty(ref _subtribusSelected, value);
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

    private int _tribusDataSetCount;

    public int TribusDataSetCount
    {
        get => _tribusDataSetCount;
        set
        {
            _tribusDataSetCount = value; OnPropertyChanged();
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
    private int _subtribusDataSetCount;
    public int SubtribusDataSetCount
    {
        get => _subtribusDataSetCount;
        set
        {
            _subtribusDataSetCount = value; OnPropertyChanged();
        }
    }
    //---------------------------------------------------
    private ObservableCollection<Tbl54Supertribus> _tbl54SupertribussesAllList;
    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesAllList
    {
        get => _tbl54SupertribussesAllList;
        set
        {
            _tbl54SupertribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl57Tribus> _tbl57TribussesList;
    public ObservableCollection<Tbl57Tribus> Tbl57TribussesList
    {
        get => _tbl57TribussesList;
        set
        {
            _tbl57TribussesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl57Tribus> _tbl57TribussesAllList;
    public ObservableCollection<Tbl57Tribus> Tbl57TribussesAllList
    {
        get => _tbl57TribussesAllList;
        set
        {
            _tbl57TribussesAllList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl60Subtribus> _tbl60SubtribusList;

    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesList
    {
        get => _tbl60SubtribusList;
        set
        {
            _tbl60SubtribusList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshTribusItems()
    {
        TribusItems.Clear();
        if (Tbl57TribussesList != null)
        {
            foreach (var item in Tbl57TribussesList)
            {
                TribusItems.Add(item);
            }

            if (Tbl57TribussesList.Count == 0)
            {
                return;
            }

            if (TribusSelected == null && Tbl57TribussesList.Count != 0)
            {
                TribusSelected = TribusItems.FirstOrDefault()!;
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
    public void RefreshSubtribusItems()
    {
        SubtribusItems.Clear();
        if (Tbl60SubtribussesList != null)
        {
            foreach (var item in Tbl60SubtribussesList)
            {
                SubtribusItems.Add(item);
            }

            if (Tbl60SubtribussesList.Count == 0)
            {
                return;
            }

            if (SubtribusSelected == null && Tbl60SubtribussesList.Count != 0)
            {
                SubtribusSelected = SubtribusItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #endregion All Properties 

}
