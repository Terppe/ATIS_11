using System.Configuration;
using System.Reflection;
using ATIS.Models;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Services;

public partial class AtisDbContext : DbContext
{
    private string _connectionString;

    public AtisDbContext() : base()
    {
        //// Version with appsettings.json
        //var builder = new ConfigurationBuilder();
        //builder.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));
        ////builder.SetBasePath(Directory.GetCurrentDirectory());
        //builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //load base settings
        //var configuration = builder.Build();
        //_connectionString = configuration.GetConnectionString("MyDbConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //// Version with appsettings.json
        //optionsBuilder
        //    .UseSqlServer(_connectionString)
        //    // EF 2.0 make some warnings as error, just ignore them
        //    // .ConfigureWarnings(w => w.Ignore(CoreEventId.IncludeIgnoredWarning))
        //    // Allow logging sql parameters
        //    .EnableSensitiveDataLogging();

        // Version with App.Config  
        optionsBuilder.EnableSensitiveDataLogging(true);
        _connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        //    optionsBuilder?.UseSqlServer(_connectionString);
        // optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.EnableRetryOnFailure());

        optionsBuilder.UseSqlServer(_connectionString,
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Tbl03Regnum> Tbl03Regnums
    {
        get; set;
    }
    public DbSet<Tbl06Phylum> Tbl06Phylums
    {
        get; set;
    }
    public DbSet<Tbl09Division> Tbl09Divisions
    {
        get; set;
    }
    public DbSet<Tbl12Subphylum> Tbl12Subphylums
    {
        get; set;
    }
    public DbSet<Tbl15Subdivision> Tbl15Subdivisions
    {
        get; set;
    }
    public DbSet<Tbl18Superclass> Tbl18Superclasses
    {
        get; set;
    }
    public DbSet<Tbl21Class> Tbl21Classes
    {
        get; set;
    }
    public DbSet<Tbl24Subclass> Tbl24Subclasses
    {
        get; set;
    }
    public DbSet<Tbl27Infraclass> Tbl27Infraclasses
    {
        get; set;
    }
    public DbSet<Tbl30Legio> Tbl30Legios
    {
        get; set;
    }
    public DbSet<Tbl33Ordo> Tbl33Ordos
    {
        get; set;
    }
    public DbSet<Tbl36Subordo> Tbl36Subordos
    {
        get; set;
    }
    public DbSet<Tbl39Infraordo> Tbl39Infraordos
    {
        get; set;
    }
    public DbSet<Tbl42Superfamily> Tbl42Superfamilies
    {
        get; set;
    }
    public DbSet<Tbl45Family> Tbl45Families
    {
        get; set;
    }
    public DbSet<Tbl48Subfamily> Tbl48Subfamilies
    {
        get; set;
    }
    public DbSet<Tbl51Infrafamily> Tbl51Infrafamilies
    {
        get; set;
    }
    public DbSet<Tbl54Supertribus> Tbl54Supertribusses
    {
        get; set;
    }
    public DbSet<Tbl57Tribus> Tbl57Tribusses
    {
        get; set;
    }
    public DbSet<Tbl60Subtribus> Tbl60Subtribusses
    {
        get; set;
    }
    public DbSet<Tbl63Infratribus> Tbl63Infratribusses
    {
        get; set;
    }
    public DbSet<Tbl66Genus> Tbl66Genusses
    {
        get; set;
    }
    public DbSet<Tbl68Speciesgroup> Tbl68Speciesgroups
    {
        get; set;
    }
    public DbSet<Tbl69FiSpecies> Tbl69FiSpeciesses
    {
        get; set;
    }
    public DbSet<Tbl72PlSpecies> Tbl72PlSpeciesses
    {
        get; set;
    }
    //public DbSet<Tbl75Species> Tbl75Speciesses
    //{
    //    get; set;
    //}
    public DbSet<Tbl78Name> Tbl78Names
    {
        get; set;
    }
    public DbSet<Tbl81Image> Tbl81Images
    {
        get; set;
    }
    public DbSet<Tbl84Synonym> Tbl84Synonyms
    {
        get; set;
    }
    public DbSet<Tbl87Geographic> Tbl87Geographics
    {
        get; set;
    }

    public DbSet<Tbl90Reference> Tbl90References
    {
        get; set;
    }
    public DbSet<Tbl90RefAuthor> Tbl90RefAuthors
    {
        get; set;
    }
    public DbSet<Tbl90RefExpert> Tbl90RefExperts
    {
        get; set;
    }
    public DbSet<Tbl90RefSource> Tbl90RefSources
    {
        get; set;
    }
    public DbSet<Tbl93Comment> Tbl93Comments
    {
        get; set;
    }
    public DbSet<TblCountry> TblCountries
    {
        get; set;
    }
    public DbSet<TblUserProfile> TblUserProfiles
    {
        get; set;
    }

}
