using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;


//    Tbl69FiSpeciesViewModel Skriptdatum:  21.05.2024  10:32    

namespace ATIS.Views.Database;

public partial class Tbl69FiSpeciesViewModel : ObservableObject
{

    #region [Private Data Members]
    public IJsonNavigationViewService JsonNavigationViewService;

    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    private readonly AllDialogs _allDialogs = new();
    public ObservableCollection<Tbl69FiSpecies> FiSpeciesItems { get; } = [];

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

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };
    #endregion [Private Data Members]      

    #region [Constructor]
    public Tbl69FiSpeciesViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl66GenussesAllList ??= [];
        //   Tbl66GenussesAllList = new ObservableCollection<Tbl66Genus>(_dataService.GetGenusAsync());
        Tbl66GenussesAllList = _dataService.GetGenusAsync();
        Tbl68SpeciesgroupsAllList ??= [];
        // Tbl68SpeciesgroupsAllList = new ObservableCollection<Tbl68Speciesgroup>(_dataService.GetSpeciesgroupAsync());
        Tbl68SpeciesgroupsAllList = _dataService.GetSpeciesgroupAsync();

        TblCountriesAllList ??= [];
        TblCountriesAllList = _dataService.GetCountryAsync();

        Tbl90ExpertsAllList ??= [];
        Tbl90ExpertsAllList = _dataService.GetRefExpertAsync();
        Tbl90AuthorsAllList ??= [];
        Tbl90AuthorsAllList = _dataService.GetRefAuthorAsync();
        Tbl90SourcesAllList ??= [];
        Tbl90SourcesAllList = _dataService.GetRefSourceAsync();
        //  Languages = new List<Language>();
        //GetValueLanguage();
        //GetValueContinent();
        //GetValueMimeType();
    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods]

    public async Task SaveFiSpeciesAsync()
    {
        if (IsNewFiSpecies)
        {
            IsNewFiSpecies = false;
        }

        Tbl69FiSpecies result = null;
        try
        {
            if (IsModified)
            {
                result = await _dataService.UpsertFiSpeciesAsync(FiSpeciesSelected);
                IsModified = false;
            }
            else
            {
                IsModified = true;
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
            IsInEdit = false;
            IsModified = false;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public static async Task DeleteFiSpeciesAsync(int fispeciesid)
    {
        var fispecies = await Context.Tbl69FiSpeciesses.FirstOrDefaultAsync(b => b.FiSpeciesId == fispeciesid);
        if (fispecies != null)
        {
            await RemoveExtras.RemoveReferencesByFiSpeciesId(fispeciesid);
            await RemoveExtras.RemoveCommentsByFiSpeciesId(fispeciesid);
            await RemoveExtras.RemoveNamesByFiSpeciesId(fispeciesid);
            await RemoveExtras.RemoveImagesByFiSpeciesId(fispeciesid);
            await RemoveExtras.RemoveSynonymsByFiSpeciesId(fispeciesid);
            await RemoveExtras.RemoveGeographicsByFiSpeciesId(fispeciesid);

            Context.Tbl69FiSpeciesses.Remove(fispecies);
            await Context.SaveChangesAsync();
        }
    }

    public Task RefreshFiSpeciesAsync()
    {
        Tbl69FiSpeciessesList ??= [];
        Tbl69FiSpeciessesList = _dataService.GetFiSpeciesAsync();

        RefreshFiSpeciesItems();
        return Task.CompletedTask;
    }

    public static async Task DeleteReferenceAsync(int referenceid)
    {
        var reference = await Context.Tbl90References.FirstOrDefaultAsync(re => re.ReferenceId == referenceid);

        Context.Tbl90References.Remove(reference);
        await Context.SaveChangesAsync();
    }

    public static async Task DeleteCommentAsync(int commentid)
    {
        var comment = await Context.Tbl93Comments.FirstOrDefaultAsync(co => co.CommentId == commentid);

        Context.Tbl93Comments.Remove(comment);
        await Context.SaveChangesAsync();

    }

    public static async Task DeleteNameAsync(int nameid)
    {
        var name = await Context.Tbl78Names.FirstOrDefaultAsync(co => co.NameId == nameid);

        Context.Tbl78Names.Remove(name);
        await Context.SaveChangesAsync();
    }
    public static async Task DeleteImageAsync(int imageid)
    {
        var image = await Context.Tbl81Images.FirstOrDefaultAsync(co => co.ImageId == imageid);

        Context.Tbl81Images.Remove(image);
        await Context.SaveChangesAsync();
    }
    public static async Task DeleteSynonymAsync(int synonymid)
    {
        var synonym = await Context.Tbl84Synonyms.FirstOrDefaultAsync(co => co.SynonymId == synonymid);

        Context.Tbl84Synonyms.Remove(synonym);
        await Context.SaveChangesAsync();
    }
    public static async Task DeleteGeographicAsync(int geographicid)
    {
        var geographic = await Context.Tbl87Geographics.FirstOrDefaultAsync(co => co.GeographicId == geographicid);

        Context.Tbl87Geographics.Remove(geographic);
        await Context.SaveChangesAsync();
    }

    public void CancelEditsAsync()
    {
        if (IsNewFiSpecies)
        {
            AddNewFiSpeciesCanceled?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            RevertChangesAsync().ConfigureAwait(false);
        }
    }
    public async Task RevertChangesAsync()
    {
        IsInEdit = false;
        if (IsModified)
        {
            await RefreshFiSpeciesAsync();
            IsModified = false;
        }
    }

    #endregion [Methods]  


    //    Part 2    


    #region All Properties

    #region Image

    private byte[] _selectedFilestream = null!;
    public byte[] SelectedFilestream
    {
        get => _selectedFilestream;
        set
        {
            _selectedFilestream = value; OnPropertyChanged(nameof(SelectedFilestream));
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

    public bool IsModified { get; set; }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isDeleting;
    public bool IsDeleting
    {
        get => _isDeleting;
        set => SetProperty(ref _isDeleting, value);
    }

    private bool IsLoaded => FiSpeciesItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isNewFiSpecies;
    public bool IsNewFiSpecies
    {
        get => _isNewFiSpecies;
        set => SetProperty(ref _isNewFiSpecies, value);
    }
    private bool _isNewReference;
    public bool IsNewReference
    {
        get => _isNewReference;
        set => SetProperty(ref _isNewReference, value);
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


    private bool _isExistingFiSpecies;
    public bool IsExistingFiSpecies
    {
        get => _isExistingFiSpecies;
        set => SetProperty(ref _isExistingFiSpecies, value);
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

    private bool _isInEdit = false;
    public bool IsInEdit
    {
        get => _isInEdit;
        set => SetProperty(ref _isInEdit, value);
    }
    public void StartEdit()
    {
        IsInEdit = true;
        IsModified = false;
    }
    public static void BeginEdit()
    {
        // Not used.
    }
    public void CancelEdit() => CancelEditsAsync();
    public async void EndEdit() => await SaveFiSpeciesAsync();

    public event EventHandler AddNewFiSpeciesCanceled;
    public bool CanRefresh => !IsModified && IsExistingFiSpecies;

    public bool CanRevert => IsModified && IsExistingFiSpecies;

    #endregion [Propertys]     

    #region Selected Properties

    private Tbl69FiSpecies _fispeciesSelected;

    public Tbl69FiSpecies FiSpeciesSelected
    {
        get => _fispeciesSelected;
        set => SetProperty(ref _fispeciesSelected, value);
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

    #endregion Selected Properties    

    #region Public Properties

    private int _fispeciesDataSetCount;

    public int FiSpeciesDataSetCount
    {
        get => _fispeciesDataSetCount;
        set
        {
            _fispeciesDataSetCount = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl66Genus> _tbl66GenussesList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get => _tbl66GenussesList;
        set
        {
            _tbl66GenussesList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl69FiSpecies> _tbl69FiSpeciessesList;

    public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesList
    {
        get => _tbl69FiSpeciessesList;
        set
        {
            _tbl69FiSpeciessesList = value; OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl69FiSpecies> _tbl69FiSpeciessesAllList;
    public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesAllList
    {
        get => _tbl69FiSpeciessesAllList;
        set
        {
            _tbl69FiSpeciessesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl72PlSpecies> _tbl72PlSpeciessesAllList;
    public ObservableCollection<Tbl72PlSpecies> Tbl72PlSpeciessesAllList
    {
        get => _tbl72PlSpeciessesAllList;
        set
        {
            _tbl72PlSpeciessesAllList = value; OnPropertyChanged();
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

    private ObservableCollection<Tbl90Reference> _tbl90ReferencesList;

    public ObservableCollection<Tbl90Reference> Tbl90ReferencesList
    {
        get => _tbl90ReferencesList;
        set
        {
            _tbl90ReferencesList = value; OnPropertyChanged();
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
    private void GetValueContinent()
    {
        _continents =
        [
            new Continent { Name = "Africa" },
            new Continent { Name = "Antarctica" },
            new Continent { Name = "Asia" },
            new Continent { Name = "Australia" },
            new Continent { Name = "Central/South America" },
            new Continent { Name = "Europe" },
            new Continent { Name = "North America/Caribbean" }
        ];

        _selectedContinent = new Continent();
    }

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

    #endregion Public Properties   

    #region Refresh Methods

    public void RefreshFiSpeciesItems()
    {
        FiSpeciesItems.Clear();
        if (Tbl69FiSpeciessesList != null)
        {
            foreach (var item in Tbl69FiSpeciessesList)
            {
                FiSpeciesItems.Add(item);
            }

            if (Tbl69FiSpeciessesList.Count == 0)
            {
                return;
            }

            if (FiSpeciesSelected == null && Tbl69FiSpeciessesList.Count != 0)
            {
                FiSpeciesSelected = FiSpeciesItems.FirstOrDefault()!;
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
