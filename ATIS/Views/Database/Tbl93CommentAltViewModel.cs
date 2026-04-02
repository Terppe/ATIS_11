using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using ATIS.Infrastructure;

//    Tbl93CommentViewModel Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Database;

public partial class Tbl93CommentViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    //private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly IDataService _data;
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl93CommentViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }
    private void GetAllCollections()
    {
        Tbl03RegnumsAllList ??= [];
        Tbl03RegnumsAllList = _dataService.GetRegnumAsync();

        Tbl06PhylumsAllList ??= [];
        Tbl06PhylumsAllList = _dataService.GetPhylumAsync();

        Tbl12SubphylumsAllList ??= [];
        Tbl12SubphylumsAllList = _dataService.GetSubphylumAsync();

        Tbl18SuperclassesAllList ??= [];
        Tbl18SuperclassesAllList = _dataService.GetSuperclassAsync();

        Tbl21ClassesAllList ??= [];
        Tbl21ClassesAllList = _dataService.GetClassAsync();

        Tbl24SubclassesAllList ??= [];
        Tbl24SubclassesAllList = _dataService.GetSubclassAsync();

        Tbl27InfraclassesAllList ??= [];
        Tbl27InfraclassesAllList = _dataService.GetInfraclassAsync();

        Tbl30LegiosAllList ??= [];
        Tbl30LegiosAllList = _dataService.GetLegioAsync();

        Tbl33OrdosAllList ??= [];
        Tbl33OrdosAllList = _dataService.GetOrdoAsync();

        Tbl36SubordosAllList ??= [];
        Tbl36SubordosAllList = _dataService.GetSubordoAsync();

        Tbl39InfraordosAllList ??= [];
        Tbl39InfraordosAllList = _dataService.GetInfraordoAsync();

        Tbl42SuperfamiliesAllList ??= [];
        Tbl42SuperfamiliesAllList = _dataService.GetSuperfamilyAsync();

        Tbl45FamiliesAllList ??= [];
        Tbl45FamiliesAllList = _dataService.GetFamilyAsync();

        Tbl48SubfamiliesAllList ??= [];
        Tbl48SubfamiliesAllList = _dataService.GetSubfamilyAsync();

        Tbl51InfrafamiliesAllList ??= [];
        Tbl51InfrafamiliesAllList = _dataService.GetInfrafamilyAsync();

        Tbl54SupertribussesAllList ??= [];
        Tbl54SupertribussesAllList = _dataService.GetSupertribusAsync();

        Tbl57TribussesAllList ??= [];
        Tbl57TribussesAllList = _dataService.GetTribusAsync();

        Tbl60SubtribussesAllList ??= [];
        Tbl60SubtribussesAllList = _dataService.GetSubtribusAsync();

        Tbl63InfratribussesAllList ??= [];
        Tbl63InfratribussesAllList = _dataService.GetInfratribusAsyncAlt();

        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = _dataService.GetGenusAsync();

        Tbl75SpeciessesAllList ??= [];
        Tbl75SpeciessesAllList = _dataService.GetSpeciesAsync();


    }

    #endregion [Constructor]

    //    Part 1    

    #region [Methods Comment]
    public async Task AddCommentAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditComment = true;
            IsNewComment = true;

            Tbl93CommentsList ??= [];
            Tbl93CommentsList.Insert(index: 0, item: new Tbl93Comment {});
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
        Tbl93Comment result = null;
        try
        {
            var deleteCommentId = viewModelCommentSelected.CommentId;
            await DeleteComment(deleteCommentId);

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
            IsDeletingComment = true;
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
            var comment = await AppData.Data.Tbl93Comments.FirstOrDefaultAsync(b => b.CommentId == commentid);
            if (comment != null)
            {
                //AppData.Data.Tbl93Comments.Remove(comment);
                //await AppData.Data.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Comment ]

    //    Part 2    

    #region All Properties

    #region [Propertys]
    //------------IsModified--------------------------------------
    public bool IsModifiedComment { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => CommentItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingComment;
    public bool IsDeletingComment
    {
        get => _isDeletingComment;
        set => SetProperty(ref _isDeletingComment, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewComment;
    public bool IsNewComment
    {
        get => _isNewComment;
        set => SetProperty(ref _isNewComment, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingComment;
    public bool IsExistingComment
    {
        get => _isExistingComment;
        set => SetProperty(ref _isExistingComment, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditComment = false;
    public bool IsInEditComment
    {
        get => _isInEditComment;
        set => SetProperty(ref _isInEditComment, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditComment()
    {
        IsInEditComment = true;
        IsModifiedComment = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditComment() => await SaveCommentAsync(CommentSelected);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshComment => !IsModifiedComment && IsExistingComment;
    //--------------CanRevert-------------------------------------
    public bool CanRevertComment => IsModifiedComment && IsExistingComment;

    #endregion [Propertys]

    #region Selected Properties

    private Tbl93Comment _commentSelected;
    public Tbl93Comment CommentSelected
    {
        get => _commentSelected;
        set => SetProperty(ref _commentSelected, value);
    }

    #endregion Selected Properties     

    #region Public Properties

    private int _commentDataSetCount;
    public int CommentDataSetCount
    {
        get => _commentDataSetCount;
        set
        {
            _commentDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchCommentInfoOrId = "";

    public string SearchCommentInfoOrId
    {
        get => _searchCommentInfoOrId;
        set
        {
            _searchCommentInfoOrId = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsAllList = null!;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get => _tbl03RegnumsAllList;
        set
        {
            _tbl03RegnumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsAllList = null!;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsAllList
    {
        get => _tbl06PhylumsAllList;
        set
        {
            _tbl06PhylumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl09Division> _tbl09DivisionsAllList = null!;
    public ObservableCollection<Tbl09Division> Tbl09DivisionsAllList
    {
        get => _tbl09DivisionsAllList;
        set
        {
            _tbl09DivisionsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl12Subphylum> _tbl12SubphylumsAllList = null!;
    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsAllList
    {
        get => _tbl12SubphylumsAllList;
        set
        {
            _tbl12SubphylumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl15Subdivision> _tbl15SubdivisionsAllList = null!;
    public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsAllList
    {
        get => _tbl15SubdivisionsAllList;
        set
        {
            _tbl15SubdivisionsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl18Superclass> _tbl18SuperclassesAllList = null!;
    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesAllList
    {
        get => _tbl18SuperclassesAllList;
        set
        {
            _tbl18SuperclassesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl21Class> _tbl21ClassesAllList = null!;
    public ObservableCollection<Tbl21Class> Tbl21ClassesAllList
    {
        get => _tbl21ClassesAllList;
        set
        {
            _tbl21ClassesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl24Subclass> _tbl24SubclassesAllList = null!;
    public ObservableCollection<Tbl24Subclass> Tbl24SubclassesAllList
    {
        get => _tbl24SubclassesAllList;
        set
        {
            _tbl24SubclassesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl27Infraclass> _tbl27InfraclassesAllList = null!;
    public ObservableCollection<Tbl27Infraclass> Tbl27InfraclassesAllList
    {
        get => _tbl27InfraclassesAllList;
        set
        {
            _tbl27InfraclassesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl30Legio> _tbl30LegiosAllList = null!;
    public ObservableCollection<Tbl30Legio> Tbl30LegiosAllList
    {
        get => _tbl30LegiosAllList;
        set
        {
            _tbl30LegiosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl33Ordo> _tbl33OrdosAllList = null!;
    public ObservableCollection<Tbl33Ordo> Tbl33OrdosAllList
    {
        get => _tbl33OrdosAllList;
        set
        {
            _tbl33OrdosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl36Subordo> _tbl36SubordosAllList = null!;
    public ObservableCollection<Tbl36Subordo> Tbl36SubordosAllList
    {
        get => _tbl36SubordosAllList;
        set
        {
            _tbl36SubordosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl39Infraordo> _tbl39InfraordosAllList = null!;
    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosAllList
    {
        get => _tbl39InfraordosAllList;
        set
        {
            _tbl39InfraordosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl42Superfamily> _tbl42SuperfamiliesAllList = null!;
    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesAllList
    {
        get => _tbl42SuperfamiliesAllList;
        set
        {
            _tbl42SuperfamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl45Family> _tbl45FamiliesAllList = null!;
    public ObservableCollection<Tbl45Family> Tbl45FamiliesAllList
    {
        get => _tbl45FamiliesAllList;
        set
        {
            _tbl45FamiliesAllList = value; OnPropertyChanged(   );
        }
    }

    private ObservableCollection<Tbl48Subfamily> _tbl48SubfamiliesAllList = null!;
    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesAllList
    {
        get => _tbl48SubfamiliesAllList;
        set
        {
            _tbl48SubfamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl51Infrafamily> _tbl51InfrafamiliesAllList = null!;
    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesAllList
    {
        get => _tbl51InfrafamiliesAllList;
        set
        {
            _tbl51InfrafamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl54Supertribus> _tbl54SupertribussesAllList = null!;
    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesAllList
    {
        get => _tbl54SupertribussesAllList;
        set
        {
            _tbl54SupertribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl57Tribus> _tbl57TribussesAllList = null!;
    public ObservableCollection<Tbl57Tribus> Tbl57TribussesAllList
    {
        get => _tbl57TribussesAllList;
        set
        {
            _tbl57TribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl60Subtribus> _tbl60SubtribussesAllList = null!;
    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesAllList
    {
        get => _tbl60SubtribussesAllList;
        set
        {
            _tbl60SubtribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl63Infratribus> _tbl63InfratribussesAllList = null!;
    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesAllList
    {
        get => _tbl63InfratribussesAllList;
        set
        {
            _tbl63InfratribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList = null!;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList = null!;
    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value; OnPropertyChanged();
        }
    }


    #endregion Public Properties  

    #region Refresh Methods

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
                CommentSelected = CommentItems.FirstOrDefault();
            }
        }
    }

    #endregion Refresh Methods


    #endregion All Properties
}
