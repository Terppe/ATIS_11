using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using Microsoft.EntityFrameworkCore;

//https://www.reddit.com/r/reactjs/comments/m4mhp7/how_to_update_state_in_useeffect_when_getting/

namespace ATIS.Services;

public partial class DataService : ObservableObject, IDataService
{
    private const string Value = "#";
    private static readonly AtisDbContext Context = new();
    // private readonly AllDialogs _allDialogs = new();

    #region CRUD

    #region Regnum
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync()
    {
        var collection = new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
            .AsNoTracking()
            .OrderBy(a => a.RegnumName)
            .ThenBy(a => a.Subregnum));
        return collection;
    }
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync(int regnumid)
    {
        var collection = new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
            .AsNoTracking()
            .Where(e => e.RegnumId == regnumid)
            .OrderBy(k => k.RegnumName)
            .ThenBy(k => k.Subregnum));

        return collection;
    }
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
                .Where(e => e.RegnumId == id))
            : new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
                .Where(e => search != null && e.RegnumName.StartsWith(search))
                .OrderBy(a => a.RegnumName)
                .ThenBy(a => a.Subregnum));
        return collection;
    }
    public async Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum regnum)
    {
        var current = await Context.Tbl03Regnums.FirstOrDefaultAsync(_regnum => _regnum.RegnumId == regnum.RegnumId);
        if (null == current)
        {
            regnum.CountId = RandomHelper.Randomnumber();
            regnum.Writer = Environment.UserName;
            regnum.WriterDate = DateTime.Now;
            regnum.Updater = Environment.UserName;
            regnum.UpdaterDate = DateTime.Now;
            Context.Tbl03Regnums.Add(regnum);
        }
        else
        {
            regnum.Updater = Environment.UserName;
            regnum.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(regnum);
        }
        await Context.SaveChangesAsync();
        return regnum;
    }
    public async Task DeleteRegnumAsync(int regnumid)
    {
        var regnum = await Context.Tbl03Regnums.FirstOrDefaultAsync(reg => reg.RegnumId == regnumid);
        if (regnum != null)
        {
            var references = await Context.Tbl90References.Where(reference => reference.RegnumId == regnumid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.RegnumId == regnumid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl03Regnums.Remove(regnum);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Regnum

    #region Phylum

    public ObservableCollection<Tbl06Phylum> GetPhylumAsync()
    {
        var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
            .AsNoTracking()
            .OrderBy(a => a.PhylumName));
        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumAsync(int phylumid)
    {
        var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
            .AsNoTracking()
            .Where(e => e.PhylumId == phylumid)
            .OrderBy(k => k.PhylumName)
        );

        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
                .Where(e => e.PhylumId == id))
            : new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
                .Where(e => search != null && e.PhylumName.StartsWith(search))
                .OrderBy(a => a.PhylumName));
        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumFromRegnumIdAsync(int regnumid)
    {
        var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
            .Where(e => e.RegnumId == regnumid)
            .OrderBy(k => k.PhylumName)
        );

        return collection;
    }
    public async Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum phylum)
    {
        var current = await Context.Tbl06Phylums.FirstOrDefaultAsync(_phy => _phy.PhylumId == phylum.PhylumId);
        if (null == current)
        {
            phylum.CountId = RandomHelper.Randomnumber();
            phylum.Writer = Environment.UserName;
            phylum.WriterDate = DateTime.Now;
            phylum.Updater = Environment.UserName;
            phylum.UpdaterDate = DateTime.Now;
            Context.Tbl06Phylums.Add(phylum);
        }
        else
        {
            phylum.Updater = Environment.UserName;
            phylum.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(phylum);
        }
        await Context.SaveChangesAsync();
        return phylum;
    }
    public async Task DeletePhylumAsync(int phylumid)
    {
        var phylum = await Context.Tbl06Phylums.FirstOrDefaultAsync(b => b.PhylumId == phylumid);
        if (phylum != null)
        {
            //necessary to delete before
            var subphylums = await Context.Tbl12Subphylums.Where(a => a.PhylumId == phylumid).ToListAsync();
            foreach (var subphylum in subphylums)
            {
                for (var i = 0; i <= subphylums.Count; i++)
                {
                    var subphylumid = subphylum.SubphylumId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubphylumId == subphylumid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubphylumId == subphylumid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl12Subphylums.RemoveRange(subphylums);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(reference => reference.PhylumId == phylumid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.PhylumId == phylumid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl06Phylums.Remove(phylum);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Phylum

    #region Division                

    public ObservableCollection<Tbl09Division> GetDivisionAsync()
    {
        var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
            .AsNoTracking()
            .OrderBy(a => a.DivisionName));
        return collection;
    }

    public ObservableCollection<Tbl09Division> GetDivisionAsync(int divisionid)
    {
        var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
            .AsNoTracking()
            .Where(e => e.DivisionId == divisionid)
            .OrderBy(k => k.DivisionName)
        );

        return collection;
    }

    public ObservableCollection<Tbl09Division> GetDivisionAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => e.DivisionId == id))
            : new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => search != null && e.DivisionName.StartsWith(search))
                .OrderBy(a => a.DivisionName));
        return collection;
    }

    public ObservableCollection<Tbl09Division> GetDivisionFromRegnumIdAsync(int regnumid)
    {
        var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
            .Where(e => e.RegnumId == regnumid)
            .OrderBy(k => k.DivisionName)
        );

        return collection;
    }

    public async Task<Tbl09Division> UpsertDivisionAsync(Tbl09Division division)
    {
        var current = await Context.Tbl09Divisions.FirstOrDefaultAsync(c => c.DivisionId == division.DivisionId);
        if (null == current)
        {
            division.CountId = RandomHelper.Randomnumber();
            division.Writer = Environment.UserName;
            division.WriterDate = DateTime.Now;
            division.Updater = Environment.UserName;
            division.UpdaterDate = DateTime.Now;
            Context.Tbl09Divisions.Add(division);
        }
        else
        {
            division.Updater = Environment.UserName;
            division.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(division);
        }
        await Context.SaveChangesAsync();
        return division;
    }

    public async Task DeleteDivisionAsync(int divisionid)
    {
        var division = await Context.Tbl09Divisions.FirstOrDefaultAsync(b => b.DivisionId == divisionid);
        if (division != null)
        {
            //necessary to delete before
            var subdivisions = await Context.Tbl15Subdivisions.Where(a => a.DivisionId == divisionid).ToListAsync();
            foreach (var subdivision in subdivisions)
            {
                for (var i = 0; i <= subdivisions.Count; i++)
                {
                    var subdivisionid = subdivision.SubdivisionId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubdivisionId == subdivisionid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubdivisionId == subdivisionid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl15Subdivisions.RemoveRange(subdivisions);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(reference => reference.DivisionId == divisionid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.DivisionId == divisionid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl09Divisions.Remove(division);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Division

    #region Subphylum

    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync()
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
            .AsNoTracking()
            .OrderBy(a => a.SubphylumName));
        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(int subphylumid)
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
        .AsNoTracking()
            .Where(e => e.SubphylumId == subphylumid)
            .OrderBy(k => k.SubphylumName)
        );

        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id))
            : new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => search != null && e.SubphylumName.StartsWith(search))
                .OrderBy(a => a.SubphylumName));
        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumFromPhylumIdAsync(int phylumid)
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
            .Where(e => e.PhylumId == phylumid)
            .OrderBy(k => k.SubphylumName)
        );

        return collection;
    }
    public async Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum subphylum)
    {
        var current = await Context.Tbl12Subphylums.FirstOrDefaultAsync(a => a.SubphylumId == subphylum.SubphylumId);
        if (null == current)
        {
            subphylum.CountId = RandomHelper.Randomnumber();
            subphylum.Writer = Environment.UserName;
            subphylum.WriterDate = DateTime.Now;
            subphylum.Updater = Environment.UserName;
            subphylum.UpdaterDate = DateTime.Now;
            Context.Tbl12Subphylums.Add(subphylum);
        }
        else
        {
            subphylum.Updater = Environment.UserName;
            subphylum.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subphylum);
        }
        await Context.SaveChangesAsync();
        return subphylum;
    }
    public async Task DeleteSubphylumAsync(int subphylumid)
    {
        var subphylum = await Context.Tbl12Subphylums.FirstOrDefaultAsync(reg => reg.SubphylumId == subphylumid);
        if (subphylum != null)
        {
            //necessary to delete before
            var superclasss = await Context.Tbl18Superclasses.Where(a => a.SubphylumId == subphylumid).ToListAsync();
            foreach (var superclass in superclasss)
            {
                for (var i = 0; i <= superclasss.Count; i++)
                {
                    var superclassid = superclass.SuperclassId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            Context.Tbl18Superclasses.RemoveRange(superclasss);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(r => r.SubphylumId == subphylumid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(c => c.SubphylumId == subphylumid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl12Subphylums.Remove(subphylum);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Subphylum

    #region Subdivision
    public ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync()
    {
        var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
            .AsNoTracking()
            .OrderBy(a => a.SubdivisionName));
        return collection;
    }

    public ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync(int subdivisionid)
    {
        var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
            .AsNoTracking()
            .Where(e => e.SubdivisionId == subdivisionid)
            .OrderBy(k => k.SubdivisionName)
        );

        return collection;
    }

    public ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => e.SubdivisionId == id))
            : new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => search != null && e.SubdivisionName.StartsWith(search))
                .OrderBy(a => a.SubdivisionName));
        return collection;
    }

    public ObservableCollection<Tbl15Subdivision> GetSubdivisionFromDivisionIdAsync(int divisionid)
    {
        var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
            .Where(e => e.DivisionId == divisionid)
            .OrderBy(k => k.SubdivisionName)
        );

        return collection;
    }

    public async Task<Tbl15Subdivision> UpsertSubdivisionAsync(Tbl15Subdivision subdivision)
    {
        var current = await Context.Tbl15Subdivisions.FirstOrDefaultAsync(a => a.SubdivisionId == subdivision.SubdivisionId);
        if (null == current)
        {
            subdivision.CountId = RandomHelper.Randomnumber();
            subdivision.Writer = Environment.UserName;
            subdivision.WriterDate = DateTime.Now;
            subdivision.Updater = Environment.UserName;
            subdivision.UpdaterDate = DateTime.Now;
            Context.Tbl15Subdivisions.Add(subdivision);
        }
        else
        {
            subdivision.Updater = Environment.UserName;
            subdivision.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subdivision);
        }
        await Context.SaveChangesAsync();
        return subdivision;
    }

    public async Task DeleteSubdivisionAsync(int subdivisionid)
    {
        var subdivision = await Context.Tbl15Subdivisions.FirstOrDefaultAsync(reg => reg.SubdivisionId == subdivisionid);
        if (subdivision != null)
        {
            //necessary to delete before
            var superclasss = await Context.Tbl18Superclasses.Where(a => a.SubdivisionId == subdivisionid).ToListAsync();
            foreach (var superclass in superclasss)
            {
                for (var i = 0; i <= superclasss.Count; i++)
                {
                    var superclassid = superclass.SuperclassId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            Context.Tbl18Superclasses.RemoveRange(superclasss);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(r => r.SubdivisionId == subdivisionid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(c => c.SubdivisionId == subdivisionid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl15Subdivisions.Remove(subdivision);
            await Context.SaveChangesAsync();
        }
    }


    #endregion Subdivision

    #region Superclass

    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync()
    {
        var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
            .AsNoTracking()
            .OrderBy(a => a.SuperclassName));
        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync(int superclassid)
    {
        var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
            .AsNoTracking()
            .Where(e => e.SuperclassId == superclassid)
            .OrderBy(k => k.SuperclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id))
            : new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => search != null && e.SuperclassName.StartsWith(search))
                .OrderBy(a => a.SuperclassName));
        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassFromSubphylumIdAsync(int subphylumid)
    {
        var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
            .Where(e => e.SubphylumId == subphylumid)
            .OrderBy(k => k.SuperclassName)
        );

        return collection;
    }

    public async Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass superclass)
    {
        var current = await Context.Tbl18Superclasses.FirstOrDefaultAsync(a => a.SuperclassId == superclass.SuperclassId);
        if (null == current)
        {
            superclass.CountId = RandomHelper.Randomnumber();
            superclass.Writer = Environment.UserName;
            superclass.WriterDate = DateTime.Now;
            superclass.Updater = Environment.UserName;
            superclass.UpdaterDate = DateTime.Now;
            Context.Tbl18Superclasses.Add(superclass);
        }
        else
        {
            superclass.Updater = Environment.UserName;
            superclass.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(superclass);
        }
        await Context.SaveChangesAsync();
        return superclass;
    }

    public async Task DeleteSuperclassAsync(int superclassid)
    {
        var superclass = await Context.Tbl18Superclasses.FirstOrDefaultAsync(reg => reg.SuperclassId == superclassid);
        if (superclass != null)
        {
            //necessary to delete before evtl noch weiter nach oben bis FiSpecies 
            var classes = await Context.Tbl21Classes.Where(a => a.SuperclassId == superclassid).ToListAsync();
            foreach (var class1 in classes)
            {
                for (var i = 0; i <= classes.Count; i++)
                {
                    var classid = class1.ClassId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.ClassId == classid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.ClassId == classid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            Context.Tbl21Classes.RemoveRange(classes);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(r => r.SuperclassId == superclassid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(c => c.SuperclassId == superclassid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl18Superclasses.Remove(superclass);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Superclass

    #region Class                

    public ObservableCollection<Tbl21Class> GetClassAsync()
    {
        var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
            .AsNoTracking()
            .OrderBy(a => a.ClassName));
        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassAsync(int classid)
    {
        var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
            .AsNoTracking()
            .Where(e => e.ClassId == classid)
            .OrderBy(k => k.ClassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => e.ClassId == id))
            : new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => search != null && e.ClassName.StartsWith(search))
                .OrderBy(a => a.ClassName));
        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassFromSuperclassIdAsync(int superclassid)
    {
        var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
            .Where(e => e.SuperclassId == superclassid)
            .OrderBy(k => k.ClassName)
        );

        return collection;
    }

    public async Task<Tbl21Class> UpsertClassAsync(Tbl21Class class1)
    {
        var current = await Context.Tbl21Classes.FirstOrDefaultAsync(c => c.ClassId == class1.ClassId);
        if (null == current)
        {
            class1.CountId = RandomHelper.Randomnumber();
            class1.Writer = Environment.UserName;
            class1.WriterDate = DateTime.Now;
            class1.Updater = Environment.UserName;
            class1.UpdaterDate = DateTime.Now;
            Context.Tbl21Classes.Add(class1);
        }
        else
        {
            class1.Updater = Environment.UserName;
            class1.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(class1);
        }
        await Context.SaveChangesAsync();
        return class1;                
    }

    public async Task DeleteClassAsync(int classid)
    {
        var class1 = await Context.Tbl21Classes.FirstOrDefaultAsync(b => b.ClassId == classid);
        if (class1 != null)
        {
            //necessary to delete before
            var subclasss = await Context.Tbl24Subclasses.Where(a => a.ClassId == classid).ToListAsync();
            foreach (var subclass in subclasss)
            {
                for (var i = 0; i <= subclasss.Count; i++)
                {
                    var subclassid = subclass.SubclassId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubclassId == subclassid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubclassId == subclassid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl24Subclasses.RemoveRange(subclasss);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(reference => reference.ClassId == classid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.ClassId == classid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl21Classes.Remove(class1);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Class                       

    #region Subclass                

    public ObservableCollection<Tbl24Subclass> GetSubclassAsync()
    {
        var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
            .AsNoTracking()
            .OrderBy(a => a.SubclassName));
        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassAsync(int subclassid)
    {
        var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
            .AsNoTracking()
            .Where(e => e.SubclassId == subclassid)
            .OrderBy(k => k.SubclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => e.SubclassId == id))
            : new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => search != null && e.SubclassName.StartsWith(search))
                .OrderBy(a => a.SubclassName));
        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassFromClassIdAsync(int classeid)
    {
        var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
            .Where(e => e.ClassId == classeid)
            .OrderBy(k => k.SubclassName)
        );

        return collection;
    }

    public async Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass subclass)
    {
        var current = await Context.Tbl24Subclasses.FirstOrDefaultAsync(c => c.SubclassId == subclass.SubclassId);
        if (null == current)
        {
            subclass.CountId = RandomHelper.Randomnumber();
            subclass.Writer = Environment.UserName;
            subclass.WriterDate = DateTime.Now;
            subclass.Updater = Environment.UserName;
            subclass.UpdaterDate = DateTime.Now;
            Context.Tbl24Subclasses.Add(subclass);
        }
        else
        {
            subclass.Updater = Environment.UserName;
            subclass.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subclass);
        }
        await Context.SaveChangesAsync();
        return subclass;
    }

    public async Task DeleteSubclassAsync(int subclassid)
    {
        var subclass = await Context.Tbl24Subclasses.FirstOrDefaultAsync(b => b.SubclassId == subclassid);
        if (subclass != null)
        {
            //necessary to delete before
            var infraclasss = await Context.Tbl27Infraclasses.Where(a => a.SubclassId == subclassid).ToListAsync();
            foreach (var infraclass in infraclasss)
            {
                for (var i = 0; i <= infraclasss.Count; i++)
                {
                    var infraclassid = infraclass.InfraclassId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfraclassId == infraclassid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfraclassId == infraclassid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl27Infraclasses.RemoveRange(infraclasss);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(reference => reference.SubclassId == subclassid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SubclassId == subclassid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl24Subclasses.Remove(subclass);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Subclass                       

    #region Infraclass                

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync()
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
            .AsNoTracking()
            .OrderBy(a => a.InfraclassName));
        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(int infraclassid)
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
            .AsNoTracking()
            .Where(e => e.InfraclassId == infraclassid)
            .OrderBy(k => k.InfraclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id))
            : new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => search != null && e.InfraclassName.StartsWith(search))
                .OrderBy(a => a.InfraclassName));
        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassFromSubclassIdAsync(int subclassid)
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
            .Where(e => e.SubclassId == subclassid)
            .OrderBy(k => k.InfraclassName)
        );

        return collection;
    }

    public async Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass infraclass)
    {
        var current = await Context.Tbl27Infraclasses.FirstOrDefaultAsync(c => c.InfraclassId == infraclass.InfraclassId);
        if (null == current)
        {
            infraclass.CountId = RandomHelper.Randomnumber();
            infraclass.Writer = Environment.UserName;
            infraclass.WriterDate = DateTime.Now;
            infraclass.Updater = Environment.UserName;
            infraclass.UpdaterDate = DateTime.Now;
            Context.Tbl27Infraclasses.Add(infraclass);
        }
        else
        {
            infraclass.Updater = Environment.UserName;
            infraclass.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(infraclass);
        }
        await Context.SaveChangesAsync();
        return infraclass;
    }

    public async Task DeleteInfraclassAsync(int infraclassid)
    {
        var infraclass = await Context.Tbl27Infraclasses.FirstOrDefaultAsync(b => b.InfraclassId == infraclassid);
        if (infraclass != null)
        {
            //necessary to delete before
            var legios = await Context.Tbl30Legios.Where(a => a.InfraclassId == infraclassid).ToListAsync();
            foreach (var legio in legios)
            {
                for (var i = 0; i <= legios.Count; i++)
                {
                    var legioid = legio.LegioId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.LegioId == legioid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.LegioId == legioid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl30Legios.RemoveRange(legios);
            await Context.SaveChangesAsync();

            var references = await Context.Tbl90References.Where(reference => reference.InfraclassId == infraclassid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.InfraclassId == infraclassid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl27Infraclasses.Remove(infraclass);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Infraclass                       

    #region Legio                

    public ObservableCollection<Tbl30Legio> GetLegioAsync()
    {
        var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
            .AsNoTracking()
            .OrderBy(a => a.LegioName));
        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioAsync(int legioid)
    {
        var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
            .AsNoTracking()
            .Where(e => e.LegioId == legioid)
            .OrderBy(k => k.LegioName)
        );

        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => e.LegioId == id))
            : new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => search != null && e.LegioName.StartsWith(search))
                .OrderBy(a => a.LegioName));
        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioFromInfraclassIdAsync(int infraclassid)
    {
        var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
            .Where(e => e.InfraclassId == infraclassid)
            .OrderBy(k => k.LegioName)
        );

        return collection;
    }

    public async Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio legio)
    {
        var current = await Context.Tbl30Legios.FirstOrDefaultAsync(c => c.LegioId == legio.LegioId);
        if (null == current)
        {
            legio.CountId = RandomHelper.Randomnumber();
            legio.Writer = Environment.UserName;
            legio.WriterDate = DateTime.Now;
            legio.Updater = Environment.UserName;
            legio.UpdaterDate = DateTime.Now;
            Context.Tbl30Legios.Add(legio);
        }
        else
        {
            legio.Updater = Environment.UserName;
            legio.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(legio);
        }
        await Context.SaveChangesAsync();
        return legio;
    }

    public async Task DeleteLegioAsync(int legioid)
    {
        var legio = await Context.Tbl30Legios.FirstOrDefaultAsync(b => b.LegioId == legioid);
        if (legio != null)
        {
            //necessary to delete before
            //------ Ordo -------------
            var ordos = await Context.Tbl33Ordos.Where(a => a.LegioId == legioid).ToListAsync();
            foreach (var ordo in ordos)
            {
                for (var i = 0; i <= ordos.Count; i++)
                {
                    var ordoid = ordo.OrdoId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.OrdoId == ordoid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.OrdoId == ordoid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl33Ordos.RemoveRange(ordos);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.LegioId == legioid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.LegioId == legioid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl30Legios.Remove(legio);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Legio                       

    #region Ordo                

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync()
    {
        var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
            .AsNoTracking()
            .OrderBy(a => a.OrdoName));
        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync(int ordoid)
    {
        var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
            .AsNoTracking()
            .Where(e => e.OrdoId == ordoid)
            .OrderBy(k => k.OrdoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => e.OrdoId == id))
            : new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => search != null && e.OrdoName.StartsWith(search))
                .OrderBy(a => a.OrdoName));
        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoFromLegioIdAsync(int legioid)
    {
        var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
            .Where(e => e.LegioId == legioid)
            .OrderBy(k => k.OrdoName)
        );

        return collection;
    }

    public async Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo ordo)
    {
        var current = await Context.Tbl33Ordos.FirstOrDefaultAsync(c => c.OrdoId == ordo.OrdoId);
        if (null == current)
        {
            ordo.CountId = RandomHelper.Randomnumber();
            ordo.Writer = Environment.UserName;
            ordo.WriterDate = DateTime.Now;
            ordo.Updater = Environment.UserName;
            ordo.UpdaterDate = DateTime.Now;
            Context.Tbl33Ordos.Add(ordo);
        }
        else
        {
            ordo.Updater = Environment.UserName;
            ordo.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(ordo);
        }
        await Context.SaveChangesAsync();
        return ordo;
    }

    public async Task DeleteOrdoAsync(int ordoid)
    {
        var ordo = await Context.Tbl33Ordos.FirstOrDefaultAsync(b => b.OrdoId == ordoid);
        if (ordo != null)
        {
            //necessary to delete before
            //------ Subordo -------------
            var subordos = await Context.Tbl36Subordos.Where(a => a.OrdoId == ordoid).ToListAsync();
            foreach (var subordo in subordos)
            {
                for (var i = 0; i <= subordos.Count; i++)
                {
                    var subordoid = subordo.SubordoId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubordoId == subordoid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubordoId == subordoid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl36Subordos.RemoveRange(subordos);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.OrdoId == ordoid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.OrdoId == ordoid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl33Ordos.Remove(ordo);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Ordo                       

    #region Subordo                

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync()
    {
        var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
            .AsNoTracking()
            .OrderBy(a => a.SubordoName));
        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync(int subordoid)
    {
        var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
            .AsNoTracking()
            .Where(e => e.SubordoId == subordoid)
            .OrderBy(k => k.SubordoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => e.SubordoId == id))
            : new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => search != null && e.SubordoName.StartsWith(search))
                .OrderBy(a => a.SubordoName));
        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoFromOrdoIdAsync(int ordoid)
    {
        var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
            .Where(e => e.OrdoId == ordoid)
            .OrderBy(k => k.SubordoName)
        );

        return collection;
    }

    public async Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo subordo)
    {
        var current = await Context.Tbl36Subordos.FirstOrDefaultAsync(c => c.SubordoId == subordo.SubordoId);
        if (null == current)
        {
            subordo.CountId = RandomHelper.Randomnumber();
            subordo.Writer = Environment.UserName;
            subordo.WriterDate = DateTime.Now;
            subordo.Updater = Environment.UserName;
            subordo.UpdaterDate = DateTime.Now;
            Context.Tbl36Subordos.Add(subordo);
        }
        else
        {
            subordo.Updater = Environment.UserName;
            subordo.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subordo);
        }
        await Context.SaveChangesAsync();
        return subordo;
    }

    public async Task DeleteSubordoAsync(int subordoid)
    {
        var subordo = await Context.Tbl36Subordos.FirstOrDefaultAsync(b => b.SubordoId == subordoid);
        if (subordo != null)
        {
            //necessary to delete before
            //------ Infraordo -------------
            var infraordos = await Context.Tbl39Infraordos.Where(a => a.SubordoId == subordoid).ToListAsync();
            foreach (var infraordo in infraordos)
            {
                for (var i = 0; i <= infraordos.Count; i++)
                {
                    var infraordoid = infraordo.InfraordoId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfraordoId == infraordoid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfraordoId == infraordoid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl39Infraordos.RemoveRange(infraordos);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.SubordoId == subordoid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SubordoId == subordoid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl36Subordos.Remove(subordo);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Subordo                       

    #region Infraordo                

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync()
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
            .AsNoTracking()
            .OrderBy(a => a.InfraordoName));
        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(int infraordoid)
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
            .AsNoTracking()
            .Where(e => e.InfraordoId == infraordoid)
            .OrderBy(k => k.InfraordoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id))
            : new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => search != null && e.InfraordoName.StartsWith(search))
                .OrderBy(a => a.InfraordoName));
        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoFromSubordoIdAsync(int subordoid)
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
            .Where(e => e.SubordoId == subordoid)
            .OrderBy(k => k.InfraordoName)
        );

        return collection;
    }

    public async Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo infraordo)
    {
        var current = await Context.Tbl39Infraordos.FirstOrDefaultAsync(c => c.InfraordoId == infraordo.InfraordoId);
        if (null == current)
        {
            infraordo.CountId = RandomHelper.Randomnumber();
            infraordo.Writer = Environment.UserName;
            infraordo.WriterDate = DateTime.Now;
            infraordo.Updater = Environment.UserName;
            infraordo.UpdaterDate = DateTime.Now;
            Context.Tbl39Infraordos.Add(infraordo);
        }
        else
        {
            infraordo.Updater = Environment.UserName;
            infraordo.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(infraordo);
        }
        await Context.SaveChangesAsync();
        return infraordo;
    }

    public async Task DeleteInfraordoAsync(int infraordoid)
    {
        var infraordo = await Context.Tbl39Infraordos.FirstOrDefaultAsync(b => b.InfraordoId == infraordoid);
        if (infraordo != null)
        {
            //necessary to delete before
            //------ Superfamily -------------
            var superfamilys = await Context.Tbl42Superfamilies.Where(a => a.InfraordoId == infraordoid).ToListAsync();
            foreach (var superfamily in superfamilys)
            {
                for (var i = 0; i <= superfamilys.Count; i++)
                {
                    var superfamilyid = superfamily.SuperfamilyId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl42Superfamilies.RemoveRange(superfamilys);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.InfraordoId == infraordoid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.InfraordoId == infraordoid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl39Infraordos.Remove(infraordo);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Infraordo                       

    #region Superfamily                

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync()
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
            .AsNoTracking()
            .OrderBy(a => a.SuperfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(int superfamilyid)
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
            .AsNoTracking()
            .Where(e => e.SuperfamilyId == superfamilyid)
            .OrderBy(k => k.SuperfamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id))
            : new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => search != null && e.SuperfamilyName.StartsWith(search))
                .OrderBy(a => a.SuperfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyFromInfraordoIdAsync(int infraordoid)
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
            .Where(e => e.InfraordoId == infraordoid)
            .OrderBy(k => k.SuperfamilyName)
        );

        return collection;
    }

    public async Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily superfamily)
    {
        var current = await Context.Tbl42Superfamilies.FirstOrDefaultAsync(c => c.SuperfamilyId == superfamily.SuperfamilyId);
        if (null == current)
        {
            superfamily.CountId = RandomHelper.Randomnumber();
            superfamily.Writer = Environment.UserName;
            superfamily.WriterDate = DateTime.Now;
            superfamily.Updater = Environment.UserName;
            superfamily.UpdaterDate = DateTime.Now;
            Context.Tbl42Superfamilies.Add(superfamily);
        }
        else
        {
            superfamily.Updater = Environment.UserName;
            superfamily.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(superfamily);
        }
        await Context.SaveChangesAsync();
        return superfamily;
    }

    public async Task DeleteSuperfamilyAsync(int superfamilyid)
    {
        var superfamily = await Context.Tbl42Superfamilies.FirstOrDefaultAsync(b => b.SuperfamilyId == superfamilyid);
        if (superfamily != null)
        {
            //necessary to delete before
            //------ Family -------------
            var familys = await Context.Tbl45Families.Where(a => a.SuperfamilyId == superfamilyid).ToListAsync();
            foreach (var family in familys)
            {
                for (var i = 0; i <= familys.Count; i++)
                {
                    var familyid = family.FamilyId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.FamilyId == familyid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.FamilyId == familyid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl45Families.RemoveRange(familys);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.SuperfamilyId == superfamilyid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SuperfamilyId == superfamilyid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl42Superfamilies.Remove(superfamily);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Superfamily                       

    #region Family                

    public ObservableCollection<Tbl45Family> GetFamilyAsync()
    {
        var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
            .AsNoTracking()
            .OrderBy(a => a.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyAsync(int familyid)
    {
        var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
            .AsNoTracking()
            .Where(e => e.FamilyId == familyid)
            .OrderBy(k => k.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => e.FamilyId == id))
            : new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => search != null && e.FamilyName.StartsWith(search))
                .OrderBy(a => a.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyFromSuperfamilyIdAsync(int superfamilyid)
    {
        var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
            .Where(e => e.SuperfamilyId == superfamilyid)
            .OrderBy(k => k.FamilyName)
        );

        return collection;
    }

    public async Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family family)
    {
        var current = await Context.Tbl45Families.FirstOrDefaultAsync(c => c.FamilyId == family.FamilyId);
        if (null == current)
        {
            family.CountId = RandomHelper.Randomnumber();
            family.Writer = Environment.UserName;
            family.WriterDate = DateTime.Now;
            family.Updater = Environment.UserName;
            family.UpdaterDate = DateTime.Now;
            Context.Tbl45Families.Add(family);
        }
        else
        {
            family.Updater = Environment.UserName;
            family.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(family);
        }
        await Context.SaveChangesAsync();
        return family;
    }

    public async Task DeleteFamilyAsync(int familyid)
    {
        var family = await Context.Tbl45Families.FirstOrDefaultAsync(b => b.FamilyId == familyid);
        if (family != null)
        {
            //necessary to delete before
            //------ Subfamily -------------
            var subfamilys = await Context.Tbl48Subfamilies.Where(a => a.FamilyId == familyid).ToListAsync();
            foreach (var subfamily in subfamilys)
            {
                for (var i = 0; i <= subfamilys.Count; i++)
                {
                    var subfamilyid = subfamily.SubfamilyId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl48Subfamilies.RemoveRange(subfamilys);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.FamilyId == familyid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.FamilyId == familyid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl45Families.Remove(family);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Family                       

    #region Subfamily                

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync()
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
            .AsNoTracking()
            .OrderBy(a => a.SubfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(int subfamilyid)
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
            .AsNoTracking()
            .Where(e => e.SubfamilyId == subfamilyid)
            .OrderBy(k => k.SubfamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id))
            : new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => search != null && e.SubfamilyName.StartsWith(search))
                .OrderBy(a => a.SubfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyFromFamilyIdAsync(int familyid)
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
            .Where(e => e.FamilyId == familyid)
            .OrderBy(k => k.SubfamilyName)
        );

        return collection;
    }

    public async Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily subfamily)
    {
        var current = await Context.Tbl48Subfamilies.FirstOrDefaultAsync(c => c.SubfamilyId == subfamily.SubfamilyId);
        if (null == current)
        {
            subfamily.CountId = RandomHelper.Randomnumber();
            subfamily.Writer = Environment.UserName;
            subfamily.WriterDate = DateTime.Now;
            subfamily.Updater = Environment.UserName;
            subfamily.UpdaterDate = DateTime.Now;
            Context.Tbl48Subfamilies.Add(subfamily);
        }
        else
        {
            subfamily.Updater = Environment.UserName;
            subfamily.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subfamily);
        }
        await Context.SaveChangesAsync();
        return subfamily;
    }

    public async Task DeleteSubfamilyAsync(int subfamilyid)
    {
        var subfamily = await Context.Tbl48Subfamilies.FirstOrDefaultAsync(b => b.SubfamilyId == subfamilyid);
        if (subfamily != null)
        {
            //necessary to delete before
            //------ Infrafamily -------------
            var infrafamilys = await Context.Tbl51Infrafamilies.Where(a => a.SubfamilyId == subfamilyid).ToListAsync();
            foreach (var infrafamily in infrafamilys)
            {
                for (var i = 0; i <= infrafamilys.Count; i++)
                {
                    var infrafamilyid = infrafamily.InfrafamilyId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl51Infrafamilies.RemoveRange(infrafamilys);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.SubfamilyId == subfamilyid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SubfamilyId == subfamilyid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl48Subfamilies.Remove(subfamily);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Subfamily                       

    #region Infrafamily                

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync()
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
            .AsNoTracking()
            .OrderBy(a => a.InfrafamilyName));
        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(int infrafamilyid)
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
            .AsNoTracking()
            .Where(e => e.InfrafamilyId == infrafamilyid)
            .OrderBy(k => k.InfrafamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id))
            : new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => search != null && e.InfrafamilyName.StartsWith(search))
                .OrderBy(a => a.InfrafamilyName));
        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyFromSubfamilyIdAsync(int subfamilyid)
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
            .Where(e => e.SubfamilyId == subfamilyid)
            .OrderBy(k => k.InfrafamilyName)
        );

        return collection;
    }

    public async Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily infrafamily)
    {
        var current = await Context.Tbl51Infrafamilies.FirstOrDefaultAsync(c => c.InfrafamilyId == infrafamily.InfrafamilyId);
        if (null == current)
        {
            infrafamily.CountId = RandomHelper.Randomnumber();
            infrafamily.Writer = Environment.UserName;
            infrafamily.WriterDate = DateTime.Now;
            infrafamily.Updater = Environment.UserName;
            infrafamily.UpdaterDate = DateTime.Now;
            Context.Tbl51Infrafamilies.Add(infrafamily);
        }
        else
        {
            infrafamily.Updater = Environment.UserName;
            infrafamily.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(infrafamily);
        }
        await Context.SaveChangesAsync();
        return infrafamily;
    }

    public async Task DeleteInfrafamilyAsync(int infrafamilyid)
    {
        var infrafamily = await Context.Tbl51Infrafamilies.FirstOrDefaultAsync(b => b.InfrafamilyId == infrafamilyid);
        if (infrafamily != null)
        {
            //necessary to delete before
            //------ Supertribus -------------
            var supertribuss = await Context.Tbl54Supertribusses.Where(a => a.InfrafamilyId == infrafamilyid).ToListAsync();
            foreach (var supertribus in supertribuss)
            {
                for (var i = 0; i <= supertribuss.Count; i++)
                {
                    var supertribusid = supertribus.SupertribusId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SupertribusId == supertribusid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SupertribusId == supertribusid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl54Supertribusses.RemoveRange(supertribuss);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.InfrafamilyId == infrafamilyid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.InfrafamilyId == infrafamilyid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl51Infrafamilies.Remove(infrafamily);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Infrafamily                       

    #region Supertribus                

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync()
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
            .AsNoTracking()
            .OrderBy(a => a.SupertribusName));
        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(int supertribusid)
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
            .AsNoTracking()
            .Where(e => e.SupertribusId == supertribusid)
            .OrderBy(k => k.SupertribusName)
        );

        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id))
            : new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => search != null && e.SupertribusName.StartsWith(search))
                .OrderBy(a => a.SupertribusName));
        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusFromInfrafamilyIdAsync(int infrafamilyid)
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
            .Where(e => e.InfrafamilyId == infrafamilyid)
            .OrderBy(k => k.SupertribusName)
        );

        return collection;
    }

    public async Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus supertribus)
    {
        var current = await Context.Tbl54Supertribusses.FirstOrDefaultAsync(c => c.SupertribusId == supertribus.SupertribusId);
        if (null == current)
        {
            supertribus.CountId = RandomHelper.Randomnumber();
            supertribus.Writer = Environment.UserName;
            supertribus.WriterDate = DateTime.Now;
            supertribus.Updater = Environment.UserName;
            supertribus.UpdaterDate = DateTime.Now;
            Context.Tbl54Supertribusses.Add(supertribus);
        }
        else
        {
            supertribus.Updater = Environment.UserName;
            supertribus.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(supertribus);
        }
        await Context.SaveChangesAsync();
        return supertribus;
    }

    public async Task DeleteSupertribusAsync(int supertribusid)
    {
        var supertribus = await Context.Tbl54Supertribusses.FirstOrDefaultAsync(b => b.SupertribusId == supertribusid);
        if (supertribus != null)
        {
            //necessary to delete before
            //------ Tribus -------------
            var tribuss = await Context.Tbl57Tribusses.Where(a => a.SupertribusId == supertribusid).ToListAsync();
            foreach (var tribus in tribuss)
            {
                for (var i = 0; i <= tribuss.Count; i++)
                {
                    var tribusid = tribus.TribusId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.TribusId == tribusid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.TribusId == tribusid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl57Tribusses.RemoveRange(tribuss);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.SupertribusId == supertribusid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SupertribusId == supertribusid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl54Supertribusses.Remove(supertribus);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Supertribus                       

    #region Tribus                

    public ObservableCollection<Tbl57Tribus> GetTribusAsync()
    {
        var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
            .AsNoTracking()
            .OrderBy(a => a.TribusName));
        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusAsync(int tribusid)
    {
        var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
            .AsNoTracking()
            .Where(e => e.TribusId == tribusid)
            .OrderBy(k => k.TribusName)
        );

        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => e.TribusId == id))
            : new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => search != null && e.TribusName.StartsWith(search))
                .OrderBy(a => a.TribusName));
        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusFromSupertribusIdAsync(int supertribusid)
    {
        var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
            .Where(e => e.SupertribusId == supertribusid)
            .OrderBy(k => k.TribusName)
        );

        return collection;
    }

    public async Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus tribus)
    {
        var current = await Context.Tbl57Tribusses.FirstOrDefaultAsync(c => c.TribusId == tribus.TribusId);
        if (null == current)
        {
            tribus.CountId = RandomHelper.Randomnumber();
            tribus.Writer = Environment.UserName;
            tribus.WriterDate = DateTime.Now;
            tribus.Updater = Environment.UserName;
            tribus.UpdaterDate = DateTime.Now;
            Context.Tbl57Tribusses.Add(tribus);
        }
        else
        {
            tribus.Updater = Environment.UserName;
            tribus.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(tribus);
        }
        await Context.SaveChangesAsync();
        return tribus;
    }

    public async Task DeleteTribusAsync(int tribusid)
    {
        var tribus = await Context.Tbl57Tribusses.FirstOrDefaultAsync(b => b.TribusId == tribusid);
        if (tribus != null)
        {
            //necessary to delete before
            //------ Subtribus -------------
            var subtribuss = await Context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
            foreach (var subtribus in subtribuss)
            {
                for (var i = 0; i <= subtribuss.Count; i++)
                {
                    var subtribusid = subtribus.SubtribusId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubtribusId == subtribusid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubtribusId == subtribusid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl60Subtribusses.RemoveRange(subtribuss);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.TribusId == tribusid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.TribusId == tribusid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl57Tribusses.Remove(tribus);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Tribus                       

    #region Subtribus                

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync()
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
            .AsNoTracking()
            .OrderBy(a => a.SubtribusName));
        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(int subtribusid)
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
            .AsNoTracking()
            .Where(e => e.SubtribusId == subtribusid)
            .OrderBy(k => k.SubtribusName)
        );

        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id))
            : new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => search != null && e.SubtribusName.StartsWith(search))
                .OrderBy(a => a.SubtribusName));
        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusFromTribusIdAsync(int tribusid)
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
            .Where(e => e.TribusId == tribusid)
            .OrderBy(k => k.SubtribusName)
        );

        return collection;
    }

    public async Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus subtribus)
    {
        var current = await Context.Tbl60Subtribusses.FirstOrDefaultAsync(c => c.SubtribusId == subtribus.SubtribusId);
        if (null == current)
        {
            subtribus.CountId = RandomHelper.Randomnumber();
            subtribus.Writer = Environment.UserName;
            subtribus.WriterDate = DateTime.Now;
            subtribus.Updater = Environment.UserName;
            subtribus.UpdaterDate = DateTime.Now;
            Context.Tbl60Subtribusses.Add(subtribus);
        }
        else
        {
            subtribus.Updater = Environment.UserName;
            subtribus.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(subtribus);
        }
        await Context.SaveChangesAsync();
        return subtribus;
    }

    public async Task DeleteSubtribusAsync(int subtribusid)
    {
        var subtribus = await Context.Tbl60Subtribusses.FirstOrDefaultAsync(b => b.SubtribusId == subtribusid);
        if (subtribus != null)
        {
            //necessary to delete before
            //------ Infratribus -------------
            var infratribuss = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
            foreach (var infratribus in infratribuss)
            {
                for (var i = 0; i <= infratribuss.Count; i++)
                {
                    var infratribusid = infratribus.InfratribusId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfratribusId == infratribusid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfratribusId == infratribusid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl63Infratribusses.RemoveRange(infratribuss);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.SubtribusId == subtribusid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.SubtribusId == subtribusid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl60Subtribusses.Remove(subtribus);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Subtribus                       

    #region Infratribus                

    public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync()
    {
        var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
            .AsNoTracking()
            .OrderBy(a => a.InfratribusName));
        return collection;
    }

    public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(int infratribusid)
    {
        var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
            .AsNoTracking()
            .Where(e => e.InfratribusId == infratribusid)
            .OrderBy(k => k.InfratribusName)
        );

        return collection;
    }

    public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => e.InfratribusId == id))
            : new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => search != null && e.InfratribusName.StartsWith(search))
                .OrderBy(a => a.InfratribusName));
        return collection;
    }

    public ObservableCollection<Tbl63Infratribus> GetInfratribusFromSubtribusIdAsync(int subtribusid)
    {
        var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
            .Where(e => e.SubtribusId == subtribusid)
            .OrderBy(k => k.InfratribusName)
        );

        return collection;
    }

    public async Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus infratribus)
    {
        var current = await Context.Tbl63Infratribusses.FirstOrDefaultAsync(c => c.InfratribusId == infratribus.InfratribusId);
        if (null == current)
        {
            infratribus.CountId = RandomHelper.Randomnumber();
            infratribus.Writer = Environment.UserName;
            infratribus.WriterDate = DateTime.Now;
            infratribus.Updater = Environment.UserName;
            infratribus.UpdaterDate = DateTime.Now;
            Context.Tbl63Infratribusses.Add(infratribus);
        }
        else
        {
            infratribus.Updater = Environment.UserName;
            infratribus.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(infratribus);
        }
        await Context.SaveChangesAsync();
        return infratribus;
    }

    public async Task DeleteInfratribusAsync(int infratribusid)
    {
        var infratribus = await Context.Tbl63Infratribusses.FirstOrDefaultAsync(b => b.InfratribusId == infratribusid);
        if (infratribus != null)
        {
            //necessary to delete before
            //------ Genus -------------
            var genuss = await Context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid).ToListAsync();
            foreach (var genus in genuss)
            {
                for (var i = 0; i <= genuss.Count; i++)
                {
                    var genusid = genus.GenusId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.GenusId == genusid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.GenusId == genusid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl66Genusses.RemoveRange(genuss);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.InfratribusId == infratribusid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.InfratribusId == infratribusid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl63Infratribusses.Remove(infratribus);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Infratribus                       

    #region Genus                

    public ObservableCollection<Tbl66Genus> GetGenusAsync()
    {
        var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(a => a.GenusName));
        return collection;
    }

    public ObservableCollection<Tbl66Genus> GetGenusAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
            .AsNoTracking()
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.GenusName)
        );

        return collection;
    }

    public ObservableCollection<Tbl66Genus> GetGenusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => e.GenusId == id))
            : new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => search != null && e.GenusName.StartsWith(search))
                .OrderBy(a => a.GenusName));
        return collection;
    }

    public ObservableCollection<Tbl66Genus> GetGenusFromInfratribusIdAsync(int infratribusid)
    {
        var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
            .Where(e => e.InfratribusId == infratribusid)
            .OrderBy(k => k.GenusName)
        );

        return collection;
    }

    public async Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus genus)
    {
        var current = await Context.Tbl66Genusses.FirstOrDefaultAsync(c => c.GenusId == genus.GenusId);
        if (null == current)
        {
            genus.CountId = RandomHelper.Randomnumber();
            genus.Writer = Environment.UserName;
            genus.WriterDate = DateTime.Now;
            genus.Updater = Environment.UserName;
            genus.UpdaterDate = DateTime.Now;
            Context.Tbl66Genusses.Add(genus);
        }
        else
        {
            genus.Updater = Environment.UserName;
            genus.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(genus);
        }
        await Context.SaveChangesAsync();
        return genus;
    }

    public async Task DeleteGenusAsync(int genusid)
    {
        var genus = await Context.Tbl66Genusses.FirstOrDefaultAsync(b => b.GenusId == genusid);
        if (genus != null)
        {
            //necessary to delete before
            //------ FiSpecies -------------
            var fispeciess = await Context.Tbl69FiSpeciesses.Where(a => a.GenusId == genusid).ToListAsync();
            foreach (var fispecies in fispeciess)
            {
                for (var i = 0; i <= fispeciess.Count; i++)
                {
                    var fispeciesid = fispecies.FiSpeciesId;
                    var singleref = Context.Tbl90References.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
                    if (singleref != null)
                    {
                        Context.Tbl90References.Remove(singleref);
                        await Context.SaveChangesAsync();
                    }
                    var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
                    if (singlecom != null)
                    {
                        Context.Tbl93Comments.Remove(singlecom);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            Context.Tbl69FiSpeciesses.RemoveRange(fispeciess);
            await Context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await Context.Tbl90References.Where(reference => reference.GenusId == genusid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.GenusId == genusid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl66Genusses.Remove(genus);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Genus

    #region Speciesgroup                

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync()
    {
        var collection = new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
            .AsNoTracking()
            .OrderBy(a => a.SpeciesgroupName)
            .ThenBy(a => a.Subspeciesgroup));
        return collection;
    }

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(int speciesgroupid)
    {
        var collection = new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
            .AsNoTracking()
            .Where(e => e.SpeciesgroupId == speciesgroupid)
            .OrderBy(k => k.SpeciesgroupName)
            .ThenBy(a => a.Subspeciesgroup));

        return collection;
    }

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
                .Where(e => e.SpeciesgroupId == id))
            : new ObservableCollection<Tbl68Speciesgroup>(Context.Tbl68Speciesgroups
                .Where(e => search != null && e.SpeciesgroupName.StartsWith(search))
                .OrderBy(a => a.SpeciesgroupName)
                .ThenBy(a => a.Subspeciesgroup));
        return collection;
    }

    public async Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup speciesgroup)
    {
        var current = await Context.Tbl68Speciesgroups.FirstOrDefaultAsync(c => c.SpeciesgroupId == speciesgroup.SpeciesgroupId);
        if (null == current)
        {
            speciesgroup.CountId = RandomHelper.Randomnumber();
            speciesgroup.Writer = Environment.UserName;
            speciesgroup.WriterDate = DateTime.Now;
            speciesgroup.Updater = Environment.UserName;
            speciesgroup.UpdaterDate = DateTime.Now;
            Context.Tbl68Speciesgroups.Add(speciesgroup);
        }
        else
        {
            speciesgroup.Updater = Environment.UserName;
            speciesgroup.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(speciesgroup);
        }
        await Context.SaveChangesAsync();
        return speciesgroup;
    }

    public async Task DeleteSpeciesgroupAsync(int speciesgroupid)
    {
        var speciesgroup = await Context.Tbl68Speciesgroups.FirstOrDefaultAsync(b => b.SpeciesgroupId == speciesgroupid);
        if (speciesgroup != null)
        {
            //necessary to delete before
            //------ FiSpecies -------------
            //var fispeciess = await Context.Tbl69FiSpeciesses.Where(a => a.SpeciesgroupId == speciesgroupid).ToListAsync();
            //foreach (var fispecies in fispeciess)
            //{
            //    for (var i = 0; i <= fispeciess.Count; i++)
            //    {
            //        var fispeciesid = fispecies.FiSpeciesId;
            //        var singleref = Context.Tbl90References.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
            //        if (singleref != null)
            //        {
            //            Context.Tbl90References.Remove(singleref);
            //            await Context.SaveChangesAsync();
            //        }
            //        var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
            //        if (singlecom != null)
            //        {
            //            Context.Tbl93Comments.Remove(singlecom);
            //            await Context.SaveChangesAsync();
            //        }
            //    }
            //}
            //Context.Tbl69FiSpeciesses.RemoveRange(fispeciess);
            //await Context.SaveChangesAsync();

            Context.Tbl68Speciesgroups.Remove(speciesgroup);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Speciesgroup 

    #region FiSpecies                

    public ObservableCollection<Tbl69FiSpecies> GetSpeciesAsync()
    {
        //var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
        //    .AsNoTracking()
        //    .OrderBy(a => a.FiSpeciesName));
        //return collection;

        var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.FiSpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));
        return collection;
    }
    public ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync()
    {
        //var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
        //    .AsNoTracking()
        //    .OrderBy(a => a.FiSpeciesName));
        //return collection;

        var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.FiSpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));
        return collection;
    }

    public ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync(int fispeciesid)
    {
        var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
            .AsNoTracking()
            .Where(e => e.FiSpeciesId == fispeciesid)
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.FiSpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));

        return collection;
    }

    public ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => e.FiSpeciesId == id))
            : new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => search != null && e.FiSpeciesName.StartsWith(search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
        return collection;
    }

    public ObservableCollection<Tbl69FiSpecies> GetFiSpeciesFromGenusNameAsync(string search)
    {
        //search about GenusName
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => e.FiSpeciesId == id))
            : new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => e.Tbl66Genusses.GenusName.StartsWith(search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );
        return collection;
    }

    public ObservableCollection<Tbl69FiSpecies> GetFiSpeciesFromGenusIdAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.FiSpeciesName)
        );

        return collection;
    }

    public async Task<Tbl69FiSpecies> UpsertFiSpeciesAsync(Tbl69FiSpecies fispecies)
    {
        var current = await Context.Tbl69FiSpeciesses.FirstOrDefaultAsync(c => c.FiSpeciesId == fispecies.FiSpeciesId);
        if (null == current)
        {
            fispecies.CountId = RandomHelper.Randomnumber();
            fispecies.Writer = Environment.UserName;
            fispecies.WriterDate = DateTime.Now;
            fispecies.Updater = Environment.UserName;
            fispecies.UpdaterDate = DateTime.Now;
            Context.Tbl69FiSpeciesses.Add(fispecies);
        }
        else
        {
            fispecies.Updater = Environment.UserName;
            fispecies.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(fispecies);
        }
        await Context.SaveChangesAsync();
        return fispecies;
    }

    public async Task DeleteFiSpeciesAsync(int fispeciesid)
    {
        var fispecies = await Context.Tbl69FiSpeciesses.FirstOrDefaultAsync(b => b.FiSpeciesId == fispeciesid);
        if (fispecies != null)
        {
            //necessary to delete before
            var references = await Context.Tbl90References.Where(reference => reference.FiSpeciesId == fispeciesid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.FiSpeciesId == fispeciesid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl69FiSpeciesses.Remove(fispecies);
            await Context.SaveChangesAsync();
        }
    }

    #endregion FiSpecies                       

    #region PlSpecies                

    public ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync()
    {
        //var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
        //    .AsNoTracking()
        //    .OrderBy(a => a.PlSpeciesName));
        //return collection;

        var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.PlSpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));
        return collection;

    }

    public ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync(int plspeciesid)
    {
        var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
            .AsNoTracking()
            .Where(e => e.PlSpeciesId == plspeciesid)
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.PlSpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));

        return collection;
    }

    public ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => e.PlSpeciesId == id))
            : new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => search != null && e.PlSpeciesName.StartsWith(search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
        return collection;
    }

    public ObservableCollection<Tbl72PlSpecies> GetPlSpeciesFromGenusNameAsync(string search)
    {
        //search about GenusName
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => e.PlSpeciesId == id))
            : new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => e.Tbl66Genusses.GenusName.StartsWith(search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );
        return collection;
    }

    public ObservableCollection<Tbl72PlSpecies> GetPlSpeciesFromGenusIdAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.PlSpeciesName)
        );

        return collection;
    }

    public async Task<Tbl72PlSpecies> UpsertPlSpeciesAsync(Tbl72PlSpecies plspecies)
    {
        var current = await Context.Tbl72PlSpeciesses.FirstOrDefaultAsync(c => c.PlSpeciesId == plspecies.PlSpeciesId);
        if (null == current)
        {
            plspecies.CountId = RandomHelper.Randomnumber();
            plspecies.Writer = Environment.UserName;
            plspecies.WriterDate = DateTime.Now;
            plspecies.Updater = Environment.UserName;
            plspecies.UpdaterDate = DateTime.Now;
            Context.Tbl72PlSpeciesses.Add(plspecies);
        }
        else
        {
            plspecies.Updater = Environment.UserName;
            plspecies.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(plspecies);
        }
        await Context.SaveChangesAsync();
        return plspecies;
    }

    public async Task DeletePlSpeciesAsync(int plspeciesid)
    {
        var plspecies = await Context.Tbl72PlSpeciesses.FirstOrDefaultAsync(b => b.PlSpeciesId == plspeciesid);
        if (plspecies != null)
        {
            var references = await Context.Tbl90References.Where(reference => reference.PlSpeciesId == plspeciesid).ToListAsync();
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();

            var comments = await Context.Tbl93Comments.Where(comment => comment.PlSpeciesId == plspeciesid).ToListAsync();
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();

            Context.Tbl72PlSpeciesses.Remove(plspecies);
            await Context.SaveChangesAsync();
        }
    }

    #endregion PlSpecies                       

    #region Name                  

    public ObservableCollection<Tbl78Name> GetNameAsync()
    {
        var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
            .AsNoTracking()
            .OrderBy(a => a.NameName));
        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameAsync(int nameid)
    {
        var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
            .AsNoTracking()
            .Where(e => e.NameId == nameid)
            .OrderBy(k => k.NameName)
        );

        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl78Name>(Context.Tbl78Names
                .Where(e => e.NameId == id))
            : new ObservableCollection<Tbl78Name>(Context.Tbl78Names
                .Where(e => search != null && e.NameName.StartsWith(search))
                .OrderBy(a => a.NameName));
        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameFromFiSpeciesIdAsync(int fispeciesid)
    {
        var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
            .Where(e => e.FiSpeciesId == fispeciesid)
            .OrderBy(k => k.NameName)
        );

        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameFromPlSpeciesIdAsync(int plspeciesid)
    {
        var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
            .Where(e => e.PlSpeciesId == plspeciesid)
            .OrderBy(k => k.NameName)
        );

        return collection;
    }

    public async Task<Tbl78Name> UpsertNameAsync(Tbl78Name name)
    {
        var current = await Context.Tbl78Names.FirstOrDefaultAsync(c => c.NameId == name.NameId);
        if (null == current)
        {
            name.CountId = RandomHelper.Randomnumber();
            name.Writer = Environment.UserName;
            name.WriterDate = DateTime.Now;
            name.Updater = Environment.UserName;
            name.UpdaterDate = DateTime.Now;
            Context.Tbl78Names.Add(name);
        }
        else
        {
            name.Updater = Environment.UserName;
            name.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(name);
        }
        await Context.SaveChangesAsync();
        return name;
    }

    public async Task DeleteNameAsync(int nameid)
    {
        var names = await Context.Tbl78Names.FirstOrDefaultAsync(b => b.NameId == nameid);
        if (names != null)
        {
            Context.Tbl78Names.Remove(names);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Name

    #region Image                  

    public ObservableCollection<Tbl81Image> GetImageAsync()
    {
        var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageAsync(int imageid)
    {
        var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
            .AsNoTracking()
            .Where(e => e.ImageId == imageid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl81Image>(Context.Tbl81Images
                .Where(e => e.ImageId == id))
            : new ObservableCollection<Tbl81Image>(Context.Tbl81Images
                .Where(e => search != null && e.Info.StartsWith(search))
                .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageFromFiSpeciesIdAsync(int fispeciesid)
    {
        var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
            .Where(e => e.FiSpeciesId == fispeciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageFromPlSpeciesIdAsync(int plspeciesid)
    {
        var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
            .Where(e => e.PlSpeciesId == plspeciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<Tbl81Image> UpsertImageAsync(Tbl81Image image, string selectedpath)
    {
        var current = await Context.Tbl81Images.FirstOrDefaultAsync(c => c.ImageId == image.ImageId);
        if (null == current)
        {
            image.CountId = RandomHelper.Randomnumber();
            image.Writer = Environment.UserName;
            image.WriterDate = DateTime.Now;
            image.Updater = Environment.UserName;
            image.UpdaterDate = DateTime.Now;
            image.FilestreamId = Guid.NewGuid();
            image.Filestream = LoadImageData(selectedpath);

            Context.Tbl81Images.Add(image);
        }
        else
        {
            image.Updater = Environment.UserName;
            image.UpdaterDate = DateTime.Now;
            image.Filestream = LoadImageData(selectedpath);
            Context.Entry(current).CurrentValues.SetValues(image);
        }
        await Context.SaveChangesAsync();
        return image;
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


    public async Task DeleteImageAsync(int imageid)
    {
        var images = await Context.Tbl81Images.FirstOrDefaultAsync(b => b.ImageId == imageid);
        if (images != null)
        {
            Context.Tbl81Images.Remove(images);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Image                  

    #region Synonym                  

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync()
    {
        var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
            .AsNoTracking()
            .OrderBy(a => a.SynonymName));
        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync(int synonymid)
    {
        var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
            .AsNoTracking()
            .Where(e => e.SynonymId == synonymid)
            .OrderBy(k => k.SynonymName)
        );

        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
                .Where(e => e.SynonymId == id))
            : new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
                .Where(e => search != null && e.SynonymName.StartsWith(search))
                .OrderBy(a => a.SynonymName));
        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymFromFiSpeciesIdAsync(int fispeciesid)
    {
        var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
            .Where(e => e.FiSpeciesId == fispeciesid)
            .OrderBy(k => k.SynonymName)
        );

        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymFromPlSpeciesIdAsync(int plspeciesid)
    {
        var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
            .Where(e => e.PlSpeciesId == plspeciesid)
            .OrderBy(k => k.SynonymName)
        );

        return collection;
    }

    public async Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym synonym)
    {
        var current = await Context.Tbl84Synonyms.FirstOrDefaultAsync(c => c.SynonymId == synonym.SynonymId);
        if (null == current)
        {
            synonym.CountId = RandomHelper.Randomnumber();
            synonym.Writer = Environment.UserName;
            synonym.WriterDate = DateTime.Now;
            synonym.Updater = Environment.UserName;
            synonym.UpdaterDate = DateTime.Now;
            Context.Tbl84Synonyms.Add(synonym);
        }
        else
        {
            synonym.Updater = Environment.UserName;
            synonym.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(synonym);
        }
        await Context.SaveChangesAsync();
        return synonym;
    }

    public async Task DeleteSynonymAsync(int synonymid)
    {
        var synonyms = await Context.Tbl84Synonyms.FirstOrDefaultAsync(b => b.SynonymId == synonymid);
        if (synonyms != null)
        {
            Context.Tbl84Synonyms.Remove(synonyms);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Name                  

    #region Geographic                  

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync()
    {
        var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync(int geographicid)
    {
        var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
            .AsNoTracking()
            .Where(e => e.GeographicId == geographicid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
                .Where(e => e.GeographicId == id))
            : new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
                .Where(e => search != null && e.Info.StartsWith(search))
                .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicFromFiSpeciesIdAsync(int fispeciesid)
    {
        var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
            .Where(e => e.FiSpeciesId == fispeciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicFromPlSpeciesIdAsync(int plspeciesid)
    {
        var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
            .Where(e => e.PlSpeciesId == plspeciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic geographic)
    {
        var current = await Context.Tbl87Geographics.FirstOrDefaultAsync(c => c.GeographicId == geographic.GeographicId);
        if (null == current)
        {
            geographic.CountId = RandomHelper.Randomnumber();
            geographic.Writer = Environment.UserName;
            geographic.WriterDate = DateTime.Now;
            geographic.Updater = Environment.UserName;
            geographic.UpdaterDate = DateTime.Now;
            Context.Tbl87Geographics.Add(geographic);
        }
        else
        {
            geographic.Updater = Environment.UserName;
            geographic.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(geographic);
        }
        await Context.SaveChangesAsync();
        return geographic;
    }

    public async Task DeleteGeographicAsync(int geographicid)
    {
        var geographics = await Context.Tbl87Geographics.FirstOrDefaultAsync(b => b.GeographicId == geographicid);
        if (geographics != null)
        {
            Context.Tbl87Geographics.Remove(geographics);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Image                  



    #region Reference

    public ObservableCollection<Tbl90Reference> GetReferenceAsync()
    {
        var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }
    public ObservableCollection<Tbl90Reference> GetReferenceAsync(int referenceid)
    {
        var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
            .AsNoTracking()
            .Where(e => e.ReferenceId == referenceid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }
    public Task<ObservableCollection<Tbl90Reference>> GetReferenceAsync(string search)
    {
        ObservableCollection<Tbl90Reference> collection = null!;
        collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
            .AsNoTracking()
            .Where(e => search != null && e.Info.StartsWith(search))
            .OrderBy(a => a.Info));

        return Task.FromResult(collection);
    }
    public async Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference reference)
    {
        var current = await Context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == reference.ReferenceId);
        if (null == current)
        {
            reference.CountId = RandomHelper.Randomnumber();
            reference.Writer = Environment.UserName;
            reference.WriterDate = DateTime.Now;
            reference.Updater = Environment.UserName;
            reference.UpdaterDate = DateTime.Now;
            Context.Tbl90References.Add(reference);
        }
        else
        {
            reference.Updater = Environment.UserName;
            reference.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(reference);
        }
        await Context.SaveChangesAsync();
        return reference;
    }
    public async Task DeleteReferenceAsync(int referenceid)
    {
        var reference = await Context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == referenceid);
        if (null != reference)
        {
            Context.Tbl90References.Remove(reference);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Reference

    #region RefAuthor

    public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync()
    {
        //var collection = new ObservableCollection<Tbl90RefAuthor>(Context.Tbl90RefAuthors
        //    .AsNoTracking()
        //    .OrderBy(k => k.RefAuthorName)
        //    .ThenBy(a => a.BookName)
        //    .ThenBy(a => a.Page1));
        var collection = new ObservableCollection<Tbl90RefAuthor>(Context.Tbl90RefAuthors
            .AsNoTracking()
            .OrderBy(x => x.RefAuthorName)
            .ThenBy(y => y.ArticelTitle)
            .ThenBy(y => y.BookName)
            .ThenBy(y => y.Page1)
            .ThenBy(y => y.Publisher)
            .ThenBy(y => y.PublicationPlace));

        return collection;
    }

    public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync(int refauthorid)
    {
        var collection = new ObservableCollection<Tbl90RefAuthor>(Context.Tbl90RefAuthors
            .Where(e => e.RefAuthorId == refauthorid)
            .OrderBy(k => k.RefAuthorName)
            .ThenBy(a => a.BookName)
            .ThenBy(a => a.Page1));
        
        return collection;
    }

    public async Task<ObservableCollection<Tbl90RefAuthor>> GetRefAuthorAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl90RefAuthor>(Context.Tbl90RefAuthors
                .Where(e => e.RefAuthorId == id))
            : new ObservableCollection<Tbl90RefAuthor>(Context.Tbl90RefAuthors
                .Where(e => e.RefAuthorName.StartsWith(search))
                .OrderBy(a => a.RefAuthorName)
                .ThenBy(a => a.BookName)
                .ThenBy(a => a.Page1));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor refauthor)
    {
        var current = await Context.Tbl90RefAuthors.FirstOrDefaultAsync(_comment => _comment.RefAuthorId == refauthor.RefAuthorId);
        if (null == current)
        {
            if (refauthor.Valid != null)
            {
            }
            else
            {
                refauthor.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
            }

            refauthor.CountId = RandomHelper.Randomnumber();
            refauthor.Writer = Environment.UserName;
            refauthor.WriterDate = DateTime.Now;
            refauthor.Updater = Environment.UserName;
            refauthor.UpdaterDate = DateTime.Now;
            Context.Tbl90RefAuthors.Add(refauthor);
        }
        else
        {
            Context.Entry(current).CurrentValues.SetValues(refauthor);
        }
        await Context.SaveChangesAsync();
        return refauthor;
    }


    public async Task DeleteRefAuthorAsync(int refauthorid)
    {
        var reference = await Context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == refauthorid);
        if (null != reference)
        {
            Context.Tbl90References.Remove(reference);
            await Context.SaveChangesAsync();
        }
    }

    #endregion RefAuthor

    #region RefSource

    public ObservableCollection<Tbl90RefSource> GetRefSourceAsync()
    {
        var collection = new ObservableCollection<Tbl90RefSource>(Context.Tbl90RefSources
            .AsNoTracking()
            .OrderBy(a => a.RefSourceName));
        return collection;
    }

    public ObservableCollection<Tbl90RefSource> GetRefSourceAsync(int refsourceid)
    {
        var collection = new ObservableCollection<Tbl90RefSource>(Context.Tbl90RefSources
            .AsNoTracking()
            .Where(e => e.RefSourceId == refsourceid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<ObservableCollection<Tbl90RefSource>> GetRefSourceAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl90RefSource>(Context.Tbl90RefSources
                .Where(e => e.RefSourceId == id))
            : new ObservableCollection<Tbl90RefSource>(Context.Tbl90RefSources
                .Where(e => e.RefSourceName.StartsWith(search))
                .OrderBy(a => a.RefSourceName)
            );
        await Task.CompletedTask;
        return collection;
    }

    public async Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource refsource)
    {
        var current = await Context.Tbl90RefSources.FirstOrDefaultAsync(a => a.RefSourceId == refsource.RefSourceId);
        if (null == current)
        {
            if (refsource.Valid != null)
            {
            }
            else
            {
                refsource.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
            }

            refsource.CountId = RandomHelper.Randomnumber();
            refsource.Writer = Environment.UserName;
            refsource.WriterDate = DateTime.Now;
            refsource.Updater = Environment.UserName;
            refsource.UpdaterDate = DateTime.Now;
            Context.Tbl90RefSources.Add(refsource);
        }
        else
        {
            Context.Entry(current).CurrentValues.SetValues(refsource);
        }
        await Context.SaveChangesAsync();
        return refsource;
    }


    public async Task DeleteRefSourceAsync(int refsourceid)
    {
        var reference = await Context.Tbl90RefSources.FirstOrDefaultAsync(_reference => _reference.RefSourceId == refsourceid);
        if (null != reference)
        {
            Context.Tbl90RefSources.Remove(reference);
            await Context.SaveChangesAsync();
        }
    }

    #endregion RefSource

    #region RefExpert

    public ObservableCollection<Tbl90RefExpert> GetRefExpertAsync()
    {
        var collection = new ObservableCollection<Tbl90RefExpert>(Context.Tbl90RefExperts
            .OrderBy(k => k.RefExpertName));
        return collection;
    }

    public ObservableCollection<Tbl90RefExpert> GetRefExpertAsync(int refexpertid)
    {
        var collection = new ObservableCollection<Tbl90RefExpert>(Context.Tbl90RefExperts
            .Where(e => e.RefExpertId == refexpertid)
            .OrderBy(k => k.RefExpertName)
        );
        return collection;
    }

    public async Task<ObservableCollection<Tbl90RefExpert>> GetRefExpertAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl90RefExpert>(Context.Tbl90RefExperts
                .Where(e => e.RefExpertId == id))
            : new ObservableCollection<Tbl90RefExpert>(Context.Tbl90RefExperts
                .Where(e => e.RefExpertName.StartsWith(search))
                .OrderBy(a => a.RefExpertName)
            );
        await Task.CompletedTask;
        return collection;
    }

    public async Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert refexpert)
    {
        var current = await Context.Tbl90RefExperts.FirstOrDefaultAsync(_comment => _comment.RefExpertId == refexpert.RefExpertId);
        if (null == current)
        {
            if (refexpert.Valid != null)
            {
            }
            else
            {
                refexpert.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
            }

            refexpert.CountId = RandomHelper.Randomnumber();
            refexpert.Writer = Environment.UserName;
            refexpert.WriterDate = DateTime.Now;
            refexpert.Updater = Environment.UserName;
            refexpert.UpdaterDate = DateTime.Now;
            Context.Tbl90RefExperts.Add(refexpert);
        }
        else
        {
            Context.Entry(current).CurrentValues.SetValues(refexpert);
        }
        await Context.SaveChangesAsync();
        return refexpert;
    }


    public async Task DeleteRefExpertAsync(int refexpertid)
    {
        var reference = await Context.Tbl90RefExperts.FirstOrDefaultAsync(_reference => _reference.RefExpertId == refexpertid);
        if (null != reference)
        {
            Context.Tbl90RefExperts.Remove(reference);
            await Context.SaveChangesAsync();
        }
    }

    //-----------------------------------------------------------
    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceAuthorsCollectionOrderByInfoFromGenusIdAndRefSourceIdIsNullAndRefExpertIdIsNull(int? genusId)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefSourceId == null && e.RefExpertId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceSourcesCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefExpertIdIsNull(int? genusId)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefAuthorId == null && e.RefExpertId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceExpertsCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefSourceIdIsNull(int? genusId)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefAuthorId == null && e.RefSourceId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }
    //-----------------------------------------------------------

    #endregion RefExpert

    #region Comment

    public ObservableCollection<Tbl93Comment> GetCommentAsync()
    {
        var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl93Comment> GetCommentAsync(int commentid)
    {
        var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
            .AsNoTracking()
            .Where(e => e.CommentId == commentid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<ObservableCollection<Tbl93Comment>> GetCommentAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.CommentId == id))
            : new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.Info.StartsWith(search))
                .OrderBy(a => a.Info));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment comment)
    {
        var current = await Context.Tbl93Comments.FirstOrDefaultAsync(_comment => _comment.CommentId == comment.CommentId);
        if (null == current)
        {
            comment.CountId = RandomHelper.Randomnumber();
            comment.Writer = Environment.UserName;
            comment.WriterDate = DateTime.Now;
            comment.Updater = Environment.UserName;
            comment.UpdaterDate = DateTime.Now;
            Context.Tbl93Comments.Add(comment);
        }
        else
        {
            comment.Updater = Environment.UserName;
            comment.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(comment);
        }
        await Context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(int commentid)
    {
        var comment = await Context.Tbl93Comments.FirstOrDefaultAsync(_comment => _comment.CommentId == commentid);
        if (null != comment)
        {
            Context.Tbl93Comments.Remove(comment);
            await Context.SaveChangesAsync();
        }
    }


    #endregion Comment

    #region Country
    public ObservableCollection<TblCountry> GetCountryAsync()
    {
        var collection = new ObservableCollection<TblCountry>(Context.TblCountries
            .AsNoTracking()
            .OrderBy(a => a.Name));
        return collection;
    }

    public ObservableCollection<TblCountry> GetCountryAsync(int countryid)
    {
        var collection = new ObservableCollection<TblCountry>(Context.TblCountries
        .AsNoTracking()
            .Where(e => e.CountryId == countryid)
            .OrderBy(k => k.Name)
        );

        return collection;
    }

    public async Task<ObservableCollection<TblCountry>> GetCountryAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblCountry>(Context.TblCountries
                .Where(e => e.CountryId == id))
            : new ObservableCollection<TblCountry>(Context.TblCountries
                .Where(e => e.Name.StartsWith(search))
                .OrderBy(a => a.Name));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblCountry> UpsertCountryAsync(TblCountry country)
    {
        var current = await Context.TblCountries.FirstOrDefaultAsync(b => b.CountryId == country.CountryId);
        if (null == current)
        {
            //country.CountId = RandomHelper.Randomnumber();
            //country.Writer = Environment.UserName;
            //country.WriterDate = DateTime.Now;
            //country.Updater = Environment.UserName;
            //country.UpdaterDate = DateTime.Now;
            Context.TblCountries.Add(country);
        }
        else
        {
            //country.Updater = Environment.UserName;
            //country.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(country);
        }
        await Context.SaveChangesAsync();
        return country;
    }

    public async Task DeleteCountryAsync(int countryid)
    {
        var country = await Context.TblCountries.FirstOrDefaultAsync(c => c.CountryId == countryid);
        if (null != country)
        {
            Context.TblCountries.Remove(country);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Country

    #region UserProfile
    public ObservableCollection<TblUserProfile> GetUserProfileAsync()
    {
        var collection = new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
            .AsNoTracking()
            .OrderBy(a => a.Email));
        return collection;
    }

    public ObservableCollection<TblUserProfile> GetUserProfileAsync(int userprofileid)
    {
        var collection = new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
        .AsNoTracking()
            .Where(e => e.UserProfileId == userprofileid)
            .OrderBy(k => k.Email)
        );

        return collection;
    }

    public ObservableCollection<TblUserProfile> GetEmailFromUserProfileAsync(string email)
    {
        var collection = new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
            .Where(e => e.Email == email)
            .OrderBy(k => k.Email)
        );
        return collection;

    }


    public async Task<ObservableCollection<TblUserProfile>> GetUserProfileAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
                .Where(e => e.UserProfileId == id))
            : new ObservableCollection<TblUserProfile>(Context.TblUserProfiles
                .Where(e => e.Email.StartsWith(search))
                .OrderBy(a => a.Email));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile userprofile)
    {
        var current = await Context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofile.UserProfileId);
        if (null == current)
        {
            userprofile.CountId = RandomHelper.Randomnumber();
          //  userprofile.Role = "User";
          ////  userprofile.Password = Crypt.CalculateHash(Password, Email);
          //  userprofile.Password = "Test";
            userprofile.Writer = Environment.UserName;
            userprofile.WriterDate = DateTime.Now;
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            Context.TblUserProfiles.Add(userprofile);
        }
        else
        {
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(userprofile);
        }
        await Context.SaveChangesAsync();
        return userprofile;
    }

    public async Task<TblUserProfile> UpsertEmailFromUserProfileAsync(TblUserProfile userprofile)
    {
        var current = await Context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofile.UserProfileId);
        if (null == current)
        {
          //  userprofile.CountId = RandomHelper.Randomnumber();
            //  userprofile.Role = "User";
            ////  userprofile.Password = Crypt.CalculateHash(Password, Email);
          //  userprofile.Password = Crypt.CalculateHash(clearTextPasswordNew, Email);

            userprofile.Writer = Environment.UserName;
            userprofile.WriterDate = DateTime.Now;
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            Context.TblUserProfiles.Add(userprofile);
        }
        else
        {
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(userprofile);
        }
        await Context.SaveChangesAsync();
        return userprofile;
    }

    public async Task DeleteUserProfileAsync(int userprofileid)
    {
        var userprofile = await Context.TblUserProfiles.FirstOrDefaultAsync(c => c.UserProfileId == userprofileid);
        if (null != userprofile)
        {
            Context.TblUserProfiles.Remove(userprofile);
            await Context.SaveChangesAsync();
        }
    }


    #endregion UserProfile


    #region Food
    public ObservableCollection<TblFood> GetFoodAsync()
    {
        var collection = new ObservableCollection<TblFood>(Context.TblFoods
            .AsNoTracking()
            .OrderBy(a => a.FoodName));
        return collection;
    }

    public ObservableCollection<TblFood> GetFoodAsync(int foodid)
    {
        var collection = new ObservableCollection<TblFood>(Context.TblFoods
        .AsNoTracking()
            .Where(e => e.FoodId == foodid)
            .OrderBy(k => k.FoodName)
        );

        return collection;
    }

    public async Task<ObservableCollection<TblFood>> GetFoodAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblFood>(Context.TblFoods
                .Where(e => e.FoodId == id))
            : new ObservableCollection<TblFood>(Context.TblFoods
                .Where(e => e.FoodName.StartsWith(search))
                .OrderBy(a => a.FoodName));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblFood> UpsertFoodAsync(TblFood food)
    {
        var current = await Context.TblFoods.FirstOrDefaultAsync(c => c.FoodId == food.FoodId);
        if (null == current)
        {
            food.CountId = RandomHelper.Randomnumber();
            food.Writer = Environment.UserName;
            food.WriterDate = DateTime.Now;
            food.Updater = Environment.UserName;
            food.UpdaterDate = DateTime.Now;

            Context.TblFoods.Add(food);
        }
        else
        {
            food.Updater = Environment.UserName;
            food.UpdaterDate = DateTime.Now;
            Context.Entry(current).CurrentValues.SetValues(food);
        }
        await Context.SaveChangesAsync();
        return food;
    }

   // public async Task<TblFood> UpsertFoodAsync(TblFood food)
   // {
    //    var current = await Context.TblFoods.FirstOrDefaultAsync(b => b.FoodId == food.FoodId);
    //    if (null == current)
    //    {
    //        //country.CountId = RandomHelper.Randomnumber();
    //        //country.Writer = Environment.UserName;
    //        //country.WriterDate = DateTime.Now;
    //        //country.Updater = Environment.UserName;
    //        //country.UpdaterDate = DateTime.Now;
    //        Context.TblFoods.Add(food);
    //    }
    //    else
    //    {
    //        //country.Updater = Environment.UserName;
    //        //country.UpdaterDate = DateTime.Now;
    //        Context.Entry(current).CurrentValues.SetValues(food);
    //    }
    //    await Context.SaveChangesAsync();
    //    return food;
    //}

    public async Task DeleteFoodAsync(int foodid)
    {
        var food = await Context.TblFoods.FirstOrDefaultAsync(c => c.FoodId == foodid);
        if (null != food)
        {
            Context.TblFoods.Remove(food);
            await Context.SaveChangesAsync();
        }
    }

    #endregion Food

    #endregion CRUD

    #region Search_Gets
    public ObservableCollection<Tbl03Regnum> GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(string filterText)
    {
        if (Context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
                    .Where(e => e.RegnumName.StartsWith(filterText) &&
                                e.RegnumName.Contains(Value) == false ||
                                e.Subregnum.Contains(filterText) ||
                                e.EngName.Contains(filterText) ||
                                e.GerName.Contains(filterText) ||
                                e.FraName.Contains(filterText) ||
                                e.PorName.Contains(filterText)
                    )
                    .OrderBy(e => e.RegnumName)
                    .ThenBy(e => e.Subregnum)
                );
                return collection;
            
        }
        return null!;
    }

    public ObservableCollection<Tbl06Phylum> GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(string filterText)
    {
        if (Context.Tbl06Phylums != null)
        {
            
                var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
                    .Where(e => e.PhylumName.StartsWith(filterText) && 
                                e.PhylumName.Contains(Value) == false ||
                                e.EngName.Contains(filterText) ||
                                e.GerName.Contains(filterText) ||
                                e.FraName.Contains(filterText) ||
                                e.PorName.Contains(filterText)
                    )
                    .OrderBy(a => a.PhylumName)
                );
                return collection;
            
        }
        return null!;
    }

    public ObservableCollection<Tbl09Division> GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(string filterText)
    {
        if (Context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => e.DivisionName.StartsWith(filterText) &&
                            e.DivisionName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.DivisionName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl12Subphylum> GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(string filterText)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => e.SubphylumName.StartsWith(filterText) &&
                            e.SubphylumName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubphylumName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl15Subdivision> GetTbl15SubdivisionsCollectionOrderBySubdivisionNameFromFilterText(string filterText)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => e.SubdivisionName.StartsWith(filterText) &&
                            e.SubdivisionName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubdivisionName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl18Superclass> GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(string filterText)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SuperclassName.StartsWith(filterText) &&
                            e.SuperclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SuperclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl21Class> GetTbl21ClassesCollectionOrderByClassNameFromFilterText(string filterText)
    {
        if (Context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => e.ClassName.StartsWith(filterText) &&
                            e.ClassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.ClassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl24Subclass> GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(string filterText)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => e.SubclassName.StartsWith(filterText) &&
                            e.SubclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl27Infraclass> GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(string filterText)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => e.InfraclassName.StartsWith(filterText) &&
                            e.InfraclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfraclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl30Legio> GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(string filterText)
    {
        if (Context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => e.LegioName.StartsWith(filterText) &&
                            e.LegioName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.LegioName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl33Ordo> GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(string filterText)
    {
        if (Context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => e.OrdoName.StartsWith(filterText) &&
                            e.OrdoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.OrdoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl36Subordo> GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(string filterText)
    {
        if (Context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => e.SubordoName.StartsWith(filterText) &&
                            e.SubordoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubordoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl39Infraordo> GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(string filterText)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => e.InfraordoName.StartsWith(filterText) &&
                            e.InfraordoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfraordoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl42Superfamily> GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(string filterText)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyName.StartsWith(filterText) &&
                            e.SuperfamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SuperfamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl45Family> GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(string filterText)
    {
        if (Context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => e.FamilyName.StartsWith(filterText) &&
                            e.FamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.FamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl48Subfamily> GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(string filterText)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => e.SubfamilyName.StartsWith(filterText) &&
                            e.SubfamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubfamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl51Infrafamily> GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(string filterText)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyName.StartsWith(filterText) &&
                            e.InfrafamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfrafamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl54Supertribus> GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(string filterText)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => e.SupertribusName.StartsWith(filterText) &&
                            e.SupertribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SupertribusName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl57Tribus> GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(string filterText)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => e.TribusName.StartsWith(filterText) &&
                            e.TribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.TribusName));

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl60Subtribus> GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(string filterText)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => e.SubtribusName.StartsWith(filterText) &&
                            e.SubtribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubtribusName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl63Infratribus> GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(string filterText)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => e.InfratribusName.StartsWith(filterText) &&
                            e.InfratribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfratribusName));
            return collection;
        }
        return null!;
    }


    public ObservableCollection<Tbl66Genus> GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(string filterText)
    {
        if (Context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => e.GenusName.StartsWith(filterText) &&
                            e.GenusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.GenusName));

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl69FiSpecies>
        GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Include(a => a.Tbl66Genusses)
                .Where(e => e.FiSpeciesName.StartsWith(filterText) &&
                            e.FiSpeciesName.Contains(Value) == false ||
                            e.Tbl66Genusses.GenusName.Contains(filterText) ||
                            e.Subspecies.Contains(filterText) ||
                            e.Divers.Contains(filterText) ||
                            e.TradeName.Contains(filterText) ||
                            e.Author.Contains(filterText)
                )
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl72PlSpecies> GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(
        string filterText)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(a => a.Tbl66Genusses)
                .Where(e => e.PlSpeciesName.StartsWith(filterText) &&
                            e.PlSpeciesName.Contains(Value) == false ||
                            e.Tbl66Genusses.GenusName.Contains(filterText) ||
                            e.Subspecies.Contains(filterText) ||
                            e.Divers.Contains(filterText) ||
                            e.TradeName.Contains(filterText) ||
                            e.Author.Contains(filterText)
                )
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl78Name> GetTbl78NamesCollectionOrderByNameNameFromFilterText(string filterText)
    {
        if (Context.Tbl78Names != null)
        {
            var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
                .Include(a => a.Tbl69FiSpeciesses)
                .Include(a => a.Tbl72PlSpeciesses)
                .Where(e => e.NameName.StartsWith(filterText) &&
                            e.Tbl69FiSpeciesses.FiSpeciesId == e.FiSpeciesId &&
                            e.Tbl72PlSpeciesses.PlSpeciesId == e.PlSpeciesId
                )
                .OrderBy(a => a.NameName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl84Synonym> GetTbl84SynonymsCollectionOrderBySynonymNameFromFilterText(string filterText)
    {
        if (Context.Tbl84Synonyms != null)
        {
            var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
                .Include(a => a.Tbl69FiSpeciesses)
                .Include(a => a.Tbl72PlSpeciesses)
                .Where(e => e.SynonymName.StartsWith(filterText) &&
                            e.Tbl69FiSpeciesses.FiSpeciesId == e.FiSpeciesId &&
                            e.Tbl72PlSpeciesses.PlSpeciesId == e.PlSpeciesId
                )
                .OrderBy(a => a.SynonymName));
            return collection;
        }
        return null!;
    }

    #endregion Search_Gets

    #region Report_Gets

    #region Regnum
    public Tbl03Regnum GetRegnumSingleByRegnumIdAndHash(int id)
    {
        if (Context.Tbl03Regnums != null)
        {
            var single = Context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id &&
                                                                   a.RegnumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl03Regnum GetRegnumSingleByRegnumId(int id)
    {
        if (Context.Tbl03Regnums != null)
        {
            var single = Context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumId(int id)
    {
        if (Context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
                .Where(e => e.RegnumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl06Phylum> CollPhylumsByRegnumIdAndHash(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
            .Where(e => e.RegnumId == id &&
                        e.PhylumName.Contains(Value) == false)
            .OrderBy(a => a.PhylumName));
            return collection;
        }
        return null!;
        ;
    }
    public ObservableCollection<Tbl09Division> CollDivisionsByRegnumIdAndHash(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => e.RegnumId == id &&
                            e.DivisionName.Contains(Value) == false)
                .OrderBy(a => a.DivisionName));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByRegnumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.RegnumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByRegnumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.RegnumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByRegnumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.RegnumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByRegnumId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.RegnumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Regnum

    #region Phylum
    public Tbl06Phylum GetPhylumSingleByPhylumIdAndHash(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var single = Context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id &&
                                                                  a.PhylumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl06Phylum GetPhylumSingleByPhylumId(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var single = Context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumId(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
                .Where(e => e.PhylumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl12Subphylum> CollSubphylumsByPhylumIdAndHash(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => e.PhylumId == id &&
                            e.SubphylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int RegnumIdFromPhylumsCollectionSelect(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var dbset = Context.Tbl06Phylums.SingleOrDefault(p => p.PhylumId == id);

            if (dbset != null)
            {
                return (int)dbset.RegnumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumIdAndHash(int id)
    {
        if (Context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(Context.Tbl03Regnums
                .Where(e => e.RegnumId == id &&
                            e.RegnumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByPhylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.PhylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByPhylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.PhylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByPhylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.PhylumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByPhylumId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.PhylumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Phylum

    #region Division
    public Tbl09Division GetDivisionSingleByDivisionIdAndHash(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var single = Context.Tbl09Divisions.SingleOrDefault(a => a.DivisionId == id &&
                                                                    a.DivisionName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl09Division GetDivisionSingleByDivisionId(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var single = Context.Tbl09Divisions.SingleOrDefault(a => a.DivisionId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl09Division> CollDivisionsByDivisionId(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => e.DivisionId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl15Subdivision> CollSubdivisionsByDivisionIdAndHash(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => e.DivisionId == id &&
                            e.SubdivisionName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int RegnumIdFromDivisionsCollectionSelect(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var dbset = Context.Tbl09Divisions
                .SingleOrDefault(p => p.DivisionId == id);

            if (dbset != null)
            {
                return dbset.RegnumId;
            }
        }

        return 0;
    }
    //ForeignKey

    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByDivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.DivisionId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByDivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.DivisionId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByDivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.DivisionId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByDivisionId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.DivisionId == id));
            return collection;
        }
        return null!;
    }
    #endregion Division

    #region Subphylum
    public Tbl12Subphylum GetSubphylumSingleBySubphylumIdAndHash(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var single = Context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id &&
                                                                     a.SubphylumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl12Subphylum GetSubphylumSingleBySubphylumId(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var single = Context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumId(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySubphylumIdAndHash(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SubphylumId == id &&
                            e.SuperclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int PhylumIdFromSubphylumsCollectionSelect(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var dbset = Context.Tbl12Subphylums
                .SingleOrDefault(p => p.SubphylumId == id);

            if (dbset != null)
            {
                return dbset.PhylumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumIdAndHash(int id)
    {
        if (Context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(Context.Tbl06Phylums
                .Where(e => e.PhylumId == id &&
                            e.PhylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsBySubphylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubphylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubphylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubphylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubphylumId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubphylumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubphylumId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubphylumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Subphylum

    #region Subdivision

    public Tbl15Subdivision GetSubdivisionSingleBySubdivisionIdAndHash(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var single = Context.Tbl15Subdivisions.SingleOrDefault(a => a.SubdivisionId == id &&
                                                                        a.SubdivisionName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }

        return null!;
    }
    public Tbl15Subdivision GetSubdivisionSingleBySubdivisionId(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var single = Context.Tbl15Subdivisions.SingleOrDefault(a => a.SubdivisionId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl15Subdivision> CollSubdivisionsBySubdivisionId(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => e.SubdivisionId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySubdivisionIdAndHash(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SubdivisionId == id &&
                            e.SuperclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int DivisionIdFromSubdivisionsCollectionSelect(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var dbset = Context.Tbl15Subdivisions
                .SingleOrDefault(p => p.SubdivisionId == id);

            if (dbset == null)
            {
                return 0;
            }

            return dbset.DivisionId;
        }
        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl09Division> CollDivisionsByDivisionIdAndHash(int id)
    {
        if (Context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(Context.Tbl09Divisions
                .Where(e => e.DivisionId == id &&
                            e.DivisionName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsBySubdivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubdivisionId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubdivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubdivisionId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubdivisionId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubdivisionId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubdivisionId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubdivisionId == id));
            return collection;
        }
        return null!;
    }
    #endregion Subdivision

    #region Superclass
    public Tbl18Superclass GetSuperclassSingleBySuperclassIdAndHash(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var single = Context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == id &&
                                                                      a.SuperclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }

        return null!;
    }

    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassId(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl21Class> CollClassesBySuperclassIdAndHash(int id)
    {
        if (Context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => e.SuperclassId == id &&
                            e.ClassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubdivisionIdFromSuperclassesCollectionSelect(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var dbset = Context.Tbl18Superclasses
                .SingleOrDefault(p => p.SuperclassId == id);

            if (dbset != null)
            {
                return dbset.SubdivisionId;
            }
        }

        return 0;
    }
    public int SubphylumIdFromSuperclassesCollectionSelect(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var dbset = Context.Tbl18Superclasses
                .SingleOrDefault(p => p.SuperclassId == id);

            if (dbset != null)
            {
                return dbset.SubphylumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumIdAndHash(int id)
    {
        if (Context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(Context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id &&
                            e.SubphylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl15Subdivision> CollSubdivisionsBySubdivisionIdAndHash(int id)
    {
        if (Context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(Context.Tbl15Subdivisions
                .Where(e => e.SubdivisionId == id &&
                            e.SubdivisionName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsBySuperclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SuperclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySuperclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SuperclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySuperclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SuperclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySuperclassId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SuperclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion Superclass

    #region Class
    public Tbl21Class GetClassSingleByClassIdAndHash(int id)
    {
        if (Context.Tbl21Classes != null)
        {
            var single = Context.Tbl21Classes.SingleOrDefault(a => a.ClassId == id &&
                                                                 a.ClassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }

        return null!;
    }

    public ObservableCollection<Tbl21Class> CollClassesByClassId(int id)
    {
        if (Context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => e.ClassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl24Subclass> CollSubclassesByClassIdAndHash(int id)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => e.ClassId == id &&
                            e.SubclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SuperclassIdFromClassesCollectionSelect(int id)
    {
        if (Context.Tbl21Classes != null)
        {
            var dbset = Context.Tbl21Classes
                .SingleOrDefault(p => p.ClassId == id);

            if (dbset != null)
            {
                return dbset.SuperclassId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassIdAndHash(int id)
    {
        if (Context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(Context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id &&
                            e.SuperclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByClassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.ClassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByClassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.ClassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByClassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.ClassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByClassId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.ClassId == id));
            return collection;
        }
        return null!;
    }
    #endregion Class

    #region Subclass
    public Tbl24Subclass GetSubclassSingleBySubclassIdAndHash(int id)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var single = Context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == id &&
                                                                      a.SubclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassId(int id)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => e.SubclassId == id));
            //var collection = new ObservableCollection<Tbl24Subclass>(_uow.Tbl24Subclasses
            //    .Find(e => e.SubclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl27Infraclass> CollInfraclassesBySubclassIdAndHash(int id)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => e.SubclassId == id &&
                            e.InfraclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.SubclassId == id &&
            //               e.InfraclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int ClassIdFromSubclassesCollectionSelect(int id)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var dbset = Context.Tbl24Subclasses
                .SingleOrDefault(p => p.SubclassId == id);

            if (dbset != null)
            {
                return dbset.ClassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl21Class> CollClassesByClassIdAndHash(int id)
    {
        if (Context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(Context.Tbl21Classes
                .Where(e => e.ClassId == id &&
                            e.ClassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl21Class>(_uow.Tbl21Classes
            //    .Find(e => e.ClassId == id &&
            //               e.ClassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    // References
    public ObservableCollection<Tbl90Reference> CollExpertsBySubclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubclassId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubclassId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infraclass
    public Tbl27Infraclass GetInfraclassSingleByInfraclassIdAndHash(int id)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var single = Context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == id &&
                                                                      a.InfraclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassId(int id)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.InfraclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl30Legio> CollLegiosByInfraclassIdAndHash(int id)
    {
        if (Context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => e.InfraclassId == id &&
                            e.LegioName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.InfraclassId == id &&
            //               e.LegioName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubclassIdFromInfraclassesCollectionSelect(int id)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var dbset = Context.Tbl27Infraclasses
                .SingleOrDefault(p => p.InfraclassId == id);

            if (dbset != null)
            {
                return dbset.SubclassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassIdAndHash(int id)
    {
        if (Context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(Context.Tbl24Subclasses
                .Where(e => e.SubclassId == id &&
                            e.SubclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl24Subclass>(_uow.Tbl24Subclasses
            //    .Find(e => e.SubclassId == id &&
            //               e.SubclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollExpertsByInfraclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfraclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfraclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfraclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfraclassId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfraclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfraclassId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.InfraclassId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfraclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Legio
    public Tbl30Legio GetLegioSingleByLegioIdAndHash(int id)
    {
        if (Context.Tbl30Legios != null)
        {
            var single = Context.Tbl30Legios.SingleOrDefault(a => a.LegioId == id &&
                                                                 a.LegioName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl30Legio> CollLegiosByLegioId(int id)
    {
        if (Context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => e.LegioId == id));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.LegioId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl33Ordo> CollOrdosByLegioIdAndHash(int id)
    {
        if (Context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => e.LegioId == id &&
                            e.OrdoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.LegioId == id &&
            //               e.OrdoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfraclassIdFromLegiosCollectionSelect(int id)
    {
        if (Context.Tbl30Legios != null)
        {
            var dbset = Context.Tbl30Legios
                .SingleOrDefault(p => p.LegioId == id);

            if (dbset != null)
            {
                return dbset.InfraclassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassIdAndHash(int id)
    {
        if (Context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(Context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id &&
                            e.InfraclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.InfraclassId == id &&
            //               e.InfraclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollExpertsByLegioId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.LegioId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByLegioId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.LegioId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByLegioId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.LegioId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByLegioId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.LegioId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.LegioId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Ordo
    public Tbl33Ordo GetOrdoSingleByOrdoIdAndHash(int id)
    {
        if (Context.Tbl33Ordos != null)
        {
            var single = Context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == id &&
                                                                a.OrdoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl33Ordo> CollOrdosByOrdoId(int id)
    {
        if (Context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => e.OrdoId == id));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.OrdoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl36Subordo> CollSubordosByOrdoIdAndHash(int id)
    {
        if (Context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => e.OrdoId == id &&
                            e.SubordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.OrdoId == id &&
            //               e.SubordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int LegioIdFromOrdosCollectionSelect(int id)
    {
        if (Context.Tbl33Ordos != null)
        {
            var dbset = Context.Tbl33Ordos
                .SingleOrDefault(p => p.OrdoId == id);

            if (dbset != null)
            {
                return dbset.LegioId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl30Legio> CollLegiosByLegioIdAndHash(int id)
    {
        if (Context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(Context.Tbl30Legios
                .Where(e => e.LegioId == id &&
                            e.LegioName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.LegioId == id &&
            //               e.LegioName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByOrdoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.OrdoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByOrdoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.OrdoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByOrdoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.OrdoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByOrdoId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.OrdoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.OrdoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subordo
    public Tbl36Subordo GetSubordoSingleBySubordoIdAndHash(int id)
    {
        if (Context.Tbl36Subordos != null)
        {
            var single = Context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == id &&
                                                                   a.SubordoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl36Subordo> CollSubordosBySubordoId(int id)
    {
        if (Context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => e.SubordoId == id));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.SubordoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl39Infraordo> CollInfraordosBySubordoIdAndHash(int id)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => e.SubordoId == id &&
                            e.InfraordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.SubordoId == id &&
            //               e.InfraordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int OrdoIdFromSubordosCollectionSelect(int id)
    {
        if (Context.Tbl36Subordos != null)
        {
            var dbset = Context.Tbl36Subordos
                .SingleOrDefault(p => p.SubordoId == id);

            if (dbset != null)
            {
                return dbset.OrdoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl33Ordo> CollOrdosByOrdoIdAndHash(int id)
    {
        if (Context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(Context.Tbl33Ordos
                .Where(e => e.OrdoId == id &&
                            e.OrdoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.OrdoId == id &&
            //               e.OrdoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubordoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubordoId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubordoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubordoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infraordo
    public Tbl39Infraordo GetInfraordoSingleByInfraordoIdAndHash(int id)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var single = Context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == id &&
                                                                     a.InfraordoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoId(int id)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.InfraordoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesByInfraordoIdAndHash(int id)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => e.InfraordoId == id &&
                            e.SuperfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.InfraordoId == id &&
            //               e.SuperfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubordoIdFromInfraordosCollectionSelect(int id)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var dbset = Context.Tbl39Infraordos
                .SingleOrDefault(p => p.InfraordoId == id);

            if (dbset != null)
            {
                return dbset.SubordoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl36Subordo> CollSubordosBySubordoIdAndHash(int id)
    {
        if (Context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(Context.Tbl36Subordos
                .Where(e => e.SubordoId == id &&
                            e.SubordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.SubordoId == id &&
            //               e.SubordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfraordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfraordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfraordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfraordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfraordoId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfraordoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfraordoId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.InfraordoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfraordoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Superfamily
    public Tbl42Superfamily GetSuperfamilySingleBySuperfamilyIdAndHash(int id)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var single = Context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == id &&
                                                                       a.SuperfamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyId(int id)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.SuperfamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl45Family> CollFamiliesBySuperfamilyIdAndHash(int id)
    {
        if (Context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => e.SuperfamilyId == id &&
                            e.FamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.SuperfamilyId == id &&
            //               e.FamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfraordoIdFromSuperfamiliesCollectionSelect(int id)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var dbset = Context.Tbl42Superfamilies
                .SingleOrDefault(p => p.SuperfamilyId == id);

            if (dbset != null)
            {
                return dbset.InfraordoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoIdAndHash(int id)
    {
        if (Context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(Context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id &&
                            e.InfraordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.InfraordoId == id &&
            //               e.InfraordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySuperfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SuperfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySuperfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SuperfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySuperfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SuperfamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySuperfamilyId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SuperfamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SuperfamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Family
    public Tbl45Family GetFamilySingleByFamilyIdAndHash(int id)
    {
        if (Context.Tbl45Families != null)
        {
            var single = Context.Tbl45Families.SingleOrDefault(a => a.FamilyId == id &&
                                                                  a.FamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl45Family> CollFamiliesByFamilyId(int id)
    {
        if (Context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => e.FamilyId == id));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.FamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesByFamilyIdAndHash(int id)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => e.FamilyId == id &&
                            e.SubfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.FamilyId == id &&
            //               e.SubfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SuperfamilyIdFromFamiliesCollectionSelect(int id)
    {
        if (Context.Tbl45Families != null)
        {
            var dbset = Context.Tbl45Families
                .SingleOrDefault(p => p.FamilyId == id);

            if (dbset != null)
            {
                return dbset.SuperfamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyIdAndHash(int id)
    {
        if (Context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(Context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id &&
                            e.SuperfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.SuperfamilyId == id &&
            //               e.SuperfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByFamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.FamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByFamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.FamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByFamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.FamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByFamilyId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.FamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.FamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subfamily
    public Tbl48Subfamily GetSubfamilySingleBySubfamilyIdAndHash(int id)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var single = Context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == id &&
                                                                     a.SubfamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyId(int id)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.SubfamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesBySubfamilyIdAndHash(int id)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => e.SubfamilyId == id &&
                            e.InfrafamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.SubfamilyId == id &&
            //               e.InfrafamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int FamilyIdFromSubfamiliesCollectionSelect(int id)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var dbset = Context.Tbl48Subfamilies
                .SingleOrDefault(p => p.SubfamilyId == id);

            if (dbset != null)
            {
                return dbset.FamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl45Family> CollFamiliesByFamilyIdAndHash(int id)
    {
        if (Context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(Context.Tbl45Families
                .Where(e => e.FamilyId == id &&
                            e.FamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.FamilyId == id &&
            //               e.FamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubfamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubfamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubfamilyId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubfamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubfamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infrafamily
    public Tbl51Infrafamily GetInfrafamilySingleByInfrafamilyIdAndHash(int id)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var single = Context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == id &&
                                                                       a.InfrafamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyId(int id)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.InfrafamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl54Supertribus> CollSupertribussesByInfrafamilyIdAndHash(int id)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => e.InfrafamilyId == id &&
                            e.SupertribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.InfrafamilyId == id &&
            //               e.SupertribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubfamilyIdFromInfrafamiliesCollectionSelect(int id)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var dbset = Context.Tbl51Infrafamilies
                .SingleOrDefault(p => p.InfrafamilyId == id);

            if (dbset != null)
            {
                return dbset.SubfamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyIdAndHash(int id)
    {
        if (Context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(Context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id &&
                            e.SubfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.SubfamilyId == id &&
            //               e.SubfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfrafamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfrafamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfrafamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfrafamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfrafamilyId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfrafamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfrafamilyId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.InfrafamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfrafamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Supertribus
    public Tbl54Supertribus GetSupertribusSingleBySupertribusIdAndHash(int id)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var single = Context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == id &&
                                                                       a.SupertribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusId(int id)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.SupertribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl57Tribus> CollTribussesBySupertribusIdAndHash(int id)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => e.SupertribusId == id &&
                            e.TribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.SupertribusId == id &&
            //               e.TribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfrafamilyIdFromSupertribussesCollectionSelect(int id)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var dbset = Context.Tbl54Supertribusses
                .SingleOrDefault(p => p.SupertribusId == id);

            if (dbset != null)
            {
                return dbset.InfrafamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyIdAndHash(int id)
    {
        if (Context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(Context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id &&
                            e.InfrafamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.InfrafamilyId == id &&
            //               e.InfrafamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySupertribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SupertribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySupertribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SupertribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySupertribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SupertribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySupertribusId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SupertribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SupertribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Tribus
    public Tbl57Tribus GetTribusSingleByTribusIdAndHash(int id)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var single = Context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == id &&
                                                                  a.TribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl57Tribus> CollTribussesByTribusId(int id)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => e.TribusId == id));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.TribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl60Subtribus> CollSubtribussesByTribusIdAndHash(int id)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => e.TribusId == id &&
                            e.SubtribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.TribusId == id &&
            //               e.SubtribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SupertribusIdFromTribussesCollectionSelect(int id)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var dbset = Context.Tbl57Tribusses
                .SingleOrDefault(p => p.TribusId == id);

            if (dbset != null)
            {
                return dbset.SupertribusId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusIdAndHash(int id)
    {
        if (Context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(Context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id &&
                            e.SupertribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.SupertribusId == id &&
            //               e.SupertribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByTribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.TribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByTribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.TribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByTribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.TribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByTribusId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.TribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.TribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subtribus
    public Tbl60Subtribus GetSubtribusSingleBySubtribusIdAndHash(int id)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var single = Context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == id &&
                                                                     a.SubtribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusId(int id)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.SubtribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl63Infratribus> CollInfratribussesBySubtribusIdAndHash(int id)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => e.SubtribusId == id &&
                            e.InfratribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl63Infratribus>(_uow.Tbl63Infratribusses
            //    .Find(e => e.SubtribusId == id &&
            //               e.InfratribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int TribusIdFromSubtribussesCollectionSelect(int id)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var dbset = Context.Tbl60Subtribusses
                .SingleOrDefault(p => p.SubtribusId == id);

            if (dbset != null)
            {
                return dbset.TribusId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl57Tribus> CollTribussesByTribusIdAndHash(int id)
    {
        if (Context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(Context.Tbl57Tribusses
                .Where(e => e.TribusId == id &&
                            e.TribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.TribusId == id &&
            //               e.TribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubtribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubtribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubtribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubtribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubtribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubtribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubtribusId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.SubtribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubtribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infratribus
    public Tbl63Infratribus GetInfratribusSingleByInfratribusIdAndHash(int id)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var single = Context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == id &&
                                                                       a.InfratribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusId(int id)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => e.InfratribusId == id));
            //var collection = new ObservableCollection<Tbl63Infratribus>(_uow.Tbl63Infratribusses
            //    .Find(e => e.InfratribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl66Genus> CollGenussesByInfratribusIdAndHash(int id)
    {
        if (Context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => e.InfratribusId == id &&
                            e.GenusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl66Genus>(_uow.Tbl66Genusses
            //    .Find(e => e.InfratribusId == id &&
            //               e.GenusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubtribusIdFromInfratribussesCollectionSelect(int id)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var dbset = Context.Tbl63Infratribusses
                .SingleOrDefault(p => p.InfratribusId == id);

            if (dbset != null)
            {
                return dbset.SubtribusId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusIdAndHash(int id)
    {
        if (Context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(Context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id &&
                            e.SubtribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.SubtribusId == id &&
            //               e.SubtribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfratribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfratribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfratribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfratribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfratribusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfratribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfratribusId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.InfratribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfratribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Genus
    public Tbl66Genus GetGenusSingleByGenusIdAndHash(int id)
    {
        if (Context.Tbl66Genusses != null)
        {
            var single = Context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == id &&
                                                                 a.GenusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl66Genus> CollGenussesByGenusId(int id)
    {
        if (Context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => e.GenusId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndHash(int id)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => e.GenusId == id &&
                            e.FiSpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                //        .Include(a => a.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.Subspecies == null &&
                            e.FiSpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndHash(int id)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => e.GenusId == id &&
                            e.PlSpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(a => a.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.Subspecies == null &&
                            e.PlSpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    //Function
    public int InfratribusIdFromGenussesCollectionSelect(int id)
    {
        if (Context.Tbl66Genusses != null)
        {
            var dbset = Context.Tbl66Genusses
                .SingleOrDefault(p => p.GenusId == id);

            if (dbset != null)
            {
                return dbset.InfratribusId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusIdAndHash(int id)
    {
        if (Context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(Context.Tbl63Infratribusses
                .Where(e => e.InfratribusId == id &&
                            e.InfratribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByGenusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.GenusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByGenusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.GenusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByGenusId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.GenusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByGenusId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.GenusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
            //    .Where(e => e.GenusId == id));
            return collection;
        }
        return null!;
    }
    #endregion Genus

    #region FiSpecies
    public Tbl69FiSpecies GetFiSpeciesSingleByFiSpeciesIdAndHash(int id)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var single = Context.Tbl69FiSpeciesses.SingleOrDefault(a => a.FiSpeciesId == id &&
                                                                     a.FiSpeciesName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public Tbl69FiSpecies GetFiSpeciesSingleByFiSpeciesId(int id)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = Context.Tbl69FiSpeciesses.FirstOrDefault(a => a.FiSpeciesId == id);
        return single;
    }
    public Tbl69FiSpecies GetFiSpeciesSingleModelByFiSpeciesId(int id)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = Context.Tbl69FiSpeciesses.FirstOrDefault(a => a.FiSpeciesId == id);
        return single;
    }

    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesId(int id)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Where(e => e.FiSpeciesId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciesSubsByFiSpeciesNameandGenusId(string name, int genusId)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == genusId &&
                            e.FiSpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesNameAndNotEmptySubspecies(int id, string name)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.FiSpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }

    //FiSpeciessesCollection = new ObservableCollection<Tbl69FiSpecies>(_context.Tbl69FiSpeciesses
    //.Include(a => a.Tbl66Genusses)
    //.Where(e => e.GenusId == id &&
    //e.Subspecies == null &&
    //e.FiSpeciesName.Contains(Value) == false));

    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndFiSpeciesNameAndEmptySubspecies(int id, string name)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.FiSpeciesName == name &&
                            (e.Subspecies == null || e.Subspecies == string.Empty) &&
                            (e.Divers == null || e.Divers == string.Empty))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesNameAndSubspeciesAndDivers(string fiSpeciesName, string subspecies,
        string divers)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl69FiSpecies>(Context.Tbl69FiSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.FiSpeciesName == fiSpeciesName && e.Subspecies == subspecies && e.Divers == divers)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl78Name> CollNamesByFiSpeciesIdAndHash(int id)
    {
        if (Context.Tbl78Names != null)
        {
            var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
                .Where(e => e.FiSpeciesId == id &&
                            e.NameName.Contains(Value) == false)
                .OrderBy(r => r.NameName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl84Synonym> CollSynonymsByFiSpeciesIdAndHash(int id)
    {
        if (Context.Tbl84Synonyms != null)
        {
            var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
                .Where(e => e.FiSpeciesId == id &&
                            e.SynonymName.Contains(Value) == false)
                .OrderBy(r => r.SynonymName));
            return collection;
        }
        return null!;
    }
    //Function
    public int GenusIdFromFiSpeciessesCollectionSelect(int id)
    {
        if (Context.Tbl69FiSpeciesses != null)
        {
            var dbset = Context.Tbl69FiSpeciesses
                .SingleOrDefault(p => p.FiSpeciesId == id);

            if (dbset != null)
            {
                return dbset.GenusId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl66Genus> CollGenussesByGenusIdAndHash(int id)
    {
        if (Context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(Context.Tbl66Genusses
                .Where(e => e.GenusId == id &&
                            e.GenusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl66Genus>(_uow.Tbl66Genusses
            //    .Find(e => e.GenusId == id &&
            //               e.GenusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByFiSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.FiSpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByFiSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.FiSpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByFiSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.FiSpeciesId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByFiSpeciesId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.FiSpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Images
    public ObservableCollection<Tbl81Image> CollImagesByFiSpeciesId(int id)
    {
        if (Context.Tbl81Images != null)
        {
            var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
                .Where(e => e.FiSpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Geographics
    public ObservableCollection<Tbl87Geographic> CollGeographicsByFiSpeciesId(int id)
    {
        if (Context.Tbl87Geographics != null)
        {
            var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
                .Where(e => e.FiSpeciesId == id));
            return collection;
        }
        return null!;
    }


    #endregion

    #region PlSpecies
    public Tbl72PlSpecies GetPlSpeciesSingleByPlSpeciesIdAndHash(int id)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var single = Context.Tbl72PlSpeciesses.SingleOrDefault(a => a.PlSpeciesId == id &&
                                                                     a.PlSpeciesName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl72PlSpecies GetPlSpeciesSingleByPlSpeciesId(int id)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = Context.Tbl72PlSpeciesses.FirstOrDefault(a => a.PlSpeciesId == id);
        return single;
    }
    public Tbl72PlSpecies GetPlSpeciesSingleModelByPlSpeciesId(int id)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = Context.Tbl72PlSpeciesses.FirstOrDefault(a => a.PlSpeciesId == id);
        return single;
    }

    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesId(int id)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Where(e => e.PlSpeciesId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciesSubsByPlSpeciesNameandGenusId(string name, int genusId)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == genusId &&
                            e.PlSpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndPlSpeciesNameAndEmptySubspecies(int id, string name)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.PlSpeciesName == name &&
                            (e.Subspecies == null || e.Subspecies == string.Empty) &&
                            (e.Divers == null || e.Divers == string.Empty)
                )
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesNameAndNotEmptySubspecies(int id, string name)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.PlSpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }

    public static ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesNameAndNotEmptySubspeciesAndHash(string name)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.PlSpeciesName == name &&
                            e.PlSpeciesName.Contains(Value) == false &&
                            e.Subspecies != null)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesNameAndSubspeciesAndDivers(string fiSpeciesName, string subspecies,
        string divers)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var collection = new ObservableCollection<Tbl72PlSpecies>(Context.Tbl72PlSpeciesses
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.PlSpeciesName == fiSpeciesName && e.Subspecies == subspecies && e.Divers == divers)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.PlSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl78Name> CollNamesByPlSpeciesIdAndHash(int id)
    {
        if (Context.Tbl78Names != null)
        {
            var collection = new ObservableCollection<Tbl78Name>(Context.Tbl78Names
                .Where(e => e.PlSpeciesId == id &&
                            e.NameName.Contains(Value) == false)
                .OrderBy(r => r.NameName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl84Synonym> CollSynonymsByPlSpeciesIdAndHash(int id)
    {
        if (Context.Tbl84Synonyms != null)
        {
            var collection = new ObservableCollection<Tbl84Synonym>(Context.Tbl84Synonyms
                .Where(e => e.PlSpeciesId == id &&
                            e.SynonymName.Contains(Value) == false)
                .OrderBy(r => r.SynonymName));
            return collection;
        }
        return null!;
    }
    //Function
    public int GenusIdFromPlSpeciessesCollectionSelect(int id)
    {
        if (Context.Tbl72PlSpeciesses != null)
        {
            var dbset = Context.Tbl72PlSpeciesses
                .SingleOrDefault(p => p.PlSpeciesId == id);

            if (dbset != null)
            {
                return dbset.GenusId;
            }
        }

        return 0;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByPlSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.PlSpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByPlSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.PlSpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByPlSpeciesId(int id)
    {
        if (Context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(Context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.PlSpeciesId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByPlSpeciesId(int id)
    {
        if (Context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(Context.Tbl93Comments
                .Where(e => e.PlSpeciesId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.PlSpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Images
    public ObservableCollection<Tbl81Image> CollImagesByPlSpeciesId(int id)
    {
        if (Context.Tbl81Images != null)
        {
            var collection = new ObservableCollection<Tbl81Image>(Context.Tbl81Images
                .Where(e => e.PlSpeciesId == id));
            //var collection = new ObservableCollection<Tbl81Image>(_uow.Tbl81Images
            //    .Find(e => e.PlSpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Geographics
    public ObservableCollection<Tbl87Geographic> CollGeographicsByPlSpeciesId(int id)
    {
        if (Context.Tbl87Geographics != null)
        {
            var collection = new ObservableCollection<Tbl87Geographic>(Context.Tbl87Geographics
                .Where(e => e.PlSpeciesId == id));
            //var collection = new ObservableCollection<Tbl87Geographic>(_uow.Tbl87Geographics
            //    .Find(e => e.PlSpeciesId == id));
            return collection;
        }
        return null!;
    }

    #endregion

    #endregion

    #region "Public ObservableCollections"

    public string SearchRegnumName
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl09Division> Tbl09DivisionsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl21Class> Tbl21ClassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl24Subclass> Tbl24SubclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl27Infraclass> Tbl27InfraclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl30Legio> Tbl30LegiosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl33Ordo> Tbl33OrdosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl36Subordo> Tbl36SubordosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl45Family> Tbl45FamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl57Tribus> Tbl57TribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl72PlSpecies> Tbl72PlSpeciessesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl78Name> Tbl78NamesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl81Image> Tbl81ImagesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsList
    {
        get; set;
    } = null!;


    public ObservableCollection<Tbl90Reference> Tbl90ReferencesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl93Comment> Tbl93CommentsList
    {
        get; set;
    } = null!;

    #endregion "Public ObservableCollections"     

    #region "Collections"

    public string FilterText
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> RegnumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl06Phylum> PhylumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl12Subphylum> SubphylumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl09Division> DivisionsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl15Subdivision> SubdivisionsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl18Superclass> SuperclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl21Class> ClassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl24Subclass> SubclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl27Infraclass> InfraclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl30Legio> LegiosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl33Ordo> OrdosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl36Subordo> SubordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl39Infraordo> InfraordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl45Family> FamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl48Subfamily> SubfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl54Supertribus> SupertribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl57Tribus> TribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl60Subtribus> SubtribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl63Infratribus> InfratribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl66Genus> GenussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl69FiSpecies> FiSpeciessesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl72PlSpecies> PlSpeciessesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl78Name> NamesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl84Synonym> SynonymsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> AuthorsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefAuthorsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> SourcesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefSourcesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> ExpertsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefExpertsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl93Comment> CommentsCollection
    {
        get; set;
    } = null!;

    #endregion "Collections"


}
