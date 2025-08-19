using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;

//    Tbl66GenusViewModel Skriptdatum:  13.06.2024  10:32    

namespace ATIS.Views.Database;

public partial class Tbl66GenusViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl69FiSpecies> SpeciesItems { get; } = [];
    public ObservableCollection<Tbl63Infratribus> InfratribusItems { get; } = [];

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    public string Search;

    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl66GenusViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl63InfratribussesAllList ??= [];
        Tbl63InfratribussesAllList = [.. _dataService.GetInfratribusAsync()];
        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];

        Tbl68SpeciesgroupsAllList ??= [];
        Tbl68SpeciesgroupsAllList = [.. _dataService.GetSpeciesgroupAsync()];

        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];
    }
    public Tbl66GenusViewModel()
    {

    }

    #endregion [Constructor]  

    //    Part 1    

    #region [Methods Genus]

    public async Task AddGenusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditGenus = true;
            IsNewGenus = true;

            Tbl63InfratribussesAllList ??= [];
            Tbl63InfratribussesAllList = [.. _dataService.GetInfratribusAsync()];

            var infratribusId = Tbl63InfratribussesAllList.First().InfratribusId;

            Tbl66GenussesList ??= [];
            Tbl66GenussesList.Insert(index: 0,
                item: new Tbl66Genus { InfratribusId = infratribusId });
        });
    }

    public async Task SaveGenusAsync(Tbl66Genus viewModelGenusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewGenus)
            {
                IsNewGenus = false;
            }
        });

        Tbl66Genus result = null;
        try
        {
            if (IsModifiedGenus)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {

                    result = await _dataService.UpsertGenusAsync(viewModelGenusSelected);
                    IsModifiedGenus = false;
                });
            }
            else
            {
                IsModifiedGenus = true;
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
            IsInEditGenus = false;
            IsModifiedGenus = false;
            IsExistingGenus = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteGenusAsync(Tbl66Genus viewModelGenusSelected)
    {
        Tbl66Genus result = null;
        try
        {
            var deleteGenusId = viewModelGenusSelected.GenusId;
            await DeleteGenus(deleteGenusId);

            result = viewModelGenusSelected;
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
            IsInEditGenus = false;
            IsModifiedGenus = false;
            IsExistingGenus = true;
            IsDeletingGenus = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteGenus(int genusid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var arrPlSpecies = new List<Tbl72PlSpecies>();
            var arrFiSpecies = new List<Tbl69FiSpecies>();
            var genus = await Context.Tbl66Genusses.FirstOrDefaultAsync(b => b.GenusId == genusid);
            if (genus != null)
            {
                //SaveToArray
                await RemoveExtras.SaveToArrayTbl69FiSpeciesses(genusid, arrFiSpecies);
                await RemoveExtras.SaveToArrayTbl72PlSpeciesses(genusid, arrPlSpecies);

                if (arrPlSpecies.Count != 0)
                {
                    // remove Tbl72PlSpeciesses
                    await RemoveExtras.RemoveArrPlSpeciesByPlSpeciesId(arrPlSpecies);
                }

                if (arrFiSpecies.Count != 0)
                {
                    // remove Tbl69FiSpeciesses
                    await RemoveExtras.RemoveArrFiSpeciesByFiSpeciesId(arrFiSpecies);
                }

                await RemoveExtras.RemoveReferencesByGenusId(genusid);
                await RemoveExtras.RemoveCommentsByGenusId(genusid);
                Context.Tbl66Genusses.Remove(genus);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Genus]

    #region [Methods Reference]

    public async Task AddReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90ExpertsAllList ??= [];
            Tbl90ExpertsAllList = [.. _dataService.GetRefExpertAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefExpertId = null, GenusId = GenusSelected.GenusId });
        });
    }
    public async Task AddReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90SourcesAllList ??= [];
            Tbl90SourcesAllList = [.. _dataService.GetRefSourceAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefSourceId = null, GenusId = GenusSelected.GenusId });
        });
    }
    public async Task AddReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Tbl90AuthorsAllList ??= [];
            Tbl90AuthorsAllList = [.. _dataService.GetRefAuthorAsync()];

            Tbl90ReferencesList ??= [];
            Tbl90ReferencesList.Insert(index: 0, item: new Tbl90Reference { RefAuthorId = null, GenusId = GenusSelected.GenusId });
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
            var id = GenusSelected.GenusId;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment() { GenusId = id });
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

    #region [Methods Species]

    public async Task AddSpeciesAsync()
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var speciesgroupId = 0;

            //Id search for first Dataset of Tbl68SpeciesgroupsList
            var collection1 = new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupName == "NotClear"));

            if (collection1.Count == 0)
            {
                await AllDialogs.ErrorMessageDialogAsync(StringsRes.DatasetNotExist + " " + StringsRes.SpeciesgroupNotClear);
            }
            else
            {
                speciesgroupId = collection1.First().SpeciesgroupId;
            }

            Tbl66GenussesAllList ??= [];
            Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];

            var genusid = GenusSelected.GenusId;

            Tbl75SpeciessesList ??= [];
            Tbl75SpeciessesList.Insert(index: 0, item: new Tbl69FiSpecies { GenusId = genusid, SpeciesgroupId = speciesgroupId });
        });
    }

    public async Task SaveSpeciesAsync(Tbl69FiSpecies viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpecies)
            {
                IsNewSpecies = false;
            }
        });

        Tbl69FiSpecies result = null;
        try
        {
            if (IsModifiedSpecies)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertFiSpeciesAsync(viewModelSpeciesSelected);
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

    #endregion [Methods Species]


    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedGenus { get; set; }
    public bool IsModifiedReference { get; set; }
    public bool IsModifiedReferenceExpert { get; set; }
    public bool IsModifiedReferenceSource { get; set; }
    public bool IsModifiedReferenceAuthor { get; set; }
    public bool IsModifiedComment { get; set; }
    public bool IsModifiedSpecies { get; set; }
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
    private bool _isDeletingGenus;
    public bool IsDeletingGenus
    {
        get => _isDeletingGenus;
        set => SetProperty(ref _isDeletingGenus, value);
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
    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewGenus;
    public bool IsNewGenus
    {
        get => _isNewGenus;
        set => SetProperty(ref _isNewGenus, value);
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
    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingGenus;
    public bool IsExistingGenus
    {
        get => _isExistingGenus;
        set => SetProperty(ref _isExistingGenus, value);
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
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }
    //-----------IsInEdit----------------------------------------
    private bool _isInEditGenus = false;
    public bool IsInEditGenus
    {
        get => _isInEditGenus;
        set => SetProperty(ref _isInEditGenus, value);
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
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditGenus()
    {
        IsInEditGenus = true;
        IsModifiedGenus = false;
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
    public void StartEditSpecies()
    {
        IsInEditSpecies = true;
        IsModifiedSpecies = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditGenus() => await SaveGenusAsync(GenusSelected);
    public async void EndEditReferenceExpert() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceSource() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditReferenceAuthor() => await SaveReferenceAsync(ReferenceSelected);
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    public async void EndEditSpecies() => await SaveSpeciesAsync(SpeciesSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshGenus => !IsModifiedGenus && IsExistingGenus;
    public bool CanRefreshReferenceExpert => !IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRefreshReferenceSource => !IsModifiedReferenceSource && IsExistingReference;
    public bool CanRefreshReferenceAuthor => !IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertGenus => IsModifiedGenus && IsExistingGenus;
    public bool CanRevertReferenceExpert => IsModifiedReferenceExpert && IsExistingReference;
    public bool CanRevertReferenceSource => IsModifiedReferenceSource && IsExistingReference;
    public bool CanRevertReferenceAuthor => IsModifiedReferenceAuthor && IsExistingReference;
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl66Genus _genusSelected;
    public Tbl66Genus GenusSelected
    {
        get => _genusSelected;
        set => SetProperty(ref _genusSelected, value);
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
    private Tbl69FiSpecies _speciesSelected;
    public Tbl69FiSpecies SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
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

    private int _genusDataSetCount;

    public int GenusDataSetCount
    {
        get => _genusDataSetCount;
        set
        {
            _genusDataSetCount = value; OnPropertyChanged();
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
    private int _speciesDataSetCount;
    public int SpeciesDataSetCount
    {
        get => _speciesDataSetCount;
        set
        {
            _speciesDataSetCount = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl69FiSpecies> _tbl75SpeciesList;

    public ObservableCollection<Tbl69FiSpecies> Tbl75SpeciessesList
    {
        get => _tbl75SpeciesList;
        set
        {
            _tbl75SpeciesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

    public void RefreshGenusItems()
    {
        GenusItems.Clear();
        if (Tbl66GenussesList != null)
        {
            foreach (var item in Tbl66GenussesList)
            {
                GenusItems.Add(item);
            }

            if (Tbl66GenussesList.Count == 0)
            {
                return;
            }

            if (GenusSelected == null && Tbl66GenussesList.Count != 0)
            {
                GenusSelected = GenusItems.FirstOrDefault()!;
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

    #endregion Refresh Methods

    #endregion All Properties 

}
