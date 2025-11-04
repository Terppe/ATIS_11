using ATIS.Models;
using ATIS.Services;
using Microsoft.EntityFrameworkCore;
using static ATIS.Views.Report.PdfModels;

namespace ATIS.Views.Database;

public static class RemoveExtras
{
    private static readonly AtisDbContext Context = new();

    //--------------Food----------------------
    public static async Task RemoveImagesByFoodId(int foodid)
    {
        var images = await Context.Tbl81Images.Where(c => c.FoodId == foodid).ToListAsync();
        if (images.Count != 0)
        {
            Context.Tbl81Images.RemoveRange(images);
            await Context.SaveChangesAsync();
        }
    }
    //--------------Disease----------------------
    public static async Task RemoveImagesByDiseaseId(int diseaseid)
    {
        var images = await Context.Tbl81Images.Where(c => c.DiseaseId == diseaseid).ToListAsync();
        if (images.Count != 0)
        {
            Context.Tbl81Images.RemoveRange(images);
            await Context.SaveChangesAsync();
        }
    }
    //--------------Miscellaneous----------------------
    public static async Task RemoveImagesByMiscellaneousId(int miscellaneousid)
    {
        var images = await Context.Tbl81Images.Where(c => c.MiscellaneousId == miscellaneousid).ToListAsync();
        if (images.Count != 0)
        {
            Context.Tbl81Images.RemoveRange(images);
            await Context.SaveChangesAsync();
        }
    }

    //------------PlSpecies-------------------
    //public static async Task RemoveReferencesByPlSpeciesId(int plspeciesid)
    //{
    //    var references = await Context.Tbl90References.Where(r => r.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (references.Count != 0)
    //    {
    //        Context.Tbl90References.RemoveRange(references);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveCommentsByPlSpeciesId(int plspeciesid)
    //{
    //    var comments = await Context.Tbl93Comments.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (comments.Count != 0)
    //    {
    //        Context.Tbl93Comments.RemoveRange(comments);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveNamesByPlSpeciesId(int plspeciesid)
    //{
    //    var names = await Context.Tbl78Names.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (names.Count != 0)
    //    {
    //        Context.Tbl78Names.RemoveRange(names);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveImagesByPlSpeciesId(int plspeciesid)
    //{
    //    var images = await Context.Tbl81Images.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (images.Count != 0)
    //    {
    //        Context.Tbl81Images.RemoveRange(images);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveSynonymsByPlSpeciesId(int plspeciesid)
    //{
    //    var synonyms = await Context.Tbl84Synonyms.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (synonyms.Count != 0)
    //    {
    //        Context.Tbl84Synonyms.RemoveRange(synonyms);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveGeographicsByPlSpeciesId(int plspeciesid)
    //{
    //    var geographics = await Context.Tbl87Geographics.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    if (geographics.Count != 0)
    //    {
    //        Context.Tbl87Geographics.RemoveRange(geographics);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //------------Species-------------------
    public static async Task RemoveReferencesBySpeciesId(int speciesid)
    {
        var references = await Context.Tbl90References.Where(r => r.SpeciesId == speciesid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveCommentsBySpeciesId(int speciesid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SpeciesId == speciesid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveNamesBySpeciesId(int speciesid)
    {
        var names = await Context.Tbl78Names.Where(c => c.SpeciesId == speciesid).ToListAsync();
        if (names.Count != 0)
        {
            Context.Tbl78Names.RemoveRange(names);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveImagesBySpeciesId(int speciesid)
    {
        var images = await Context.Tbl81Images.Where(c => c.SpeciesId == speciesid).ToListAsync();
        if (images.Count != 0)
        {
            Context.Tbl81Images.RemoveRange(images);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveSynonymsBySpeciesId(int speciesid)
    {
        var synonyms = await Context.Tbl84Synonyms.Where(c => c.SpeciesId == speciesid).ToListAsync();
        if (synonyms.Count != 0)
        {
            Context.Tbl84Synonyms.RemoveRange(synonyms);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveGeographicsBySpeciesId(int speciesid)
    {
        var geographics = await Context.Tbl87Geographics.Where(c => c.SpeciesId == speciesid).ToListAsync();
        if (geographics.Count != 0)
        {
            Context.Tbl87Geographics.RemoveRange(geographics);
            await Context.SaveChangesAsync();
        }
    }

    //------------Genus-------------------
    public static async Task RemoveReferencesByGenusId(int genusid)
    {
        var references = await Context.Tbl90References.Where(r => r.GenusId == genusid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveCommentsByGenusId(int genusid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.GenusId == genusid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Infratribus-------------------
    public static async Task RemoveReferencesByInfratribusId(int infratribusid)
    {
        var references = await Context.Tbl90References.Where(r => r.InfratribusId == infratribusid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveCommentsByInfratribusId(int infratribusid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.InfratribusId == infratribusid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subtribus-------------------
    public static async Task RemoveReferencesBySubtribusId(int subtribusid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubtribusId == subtribusid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubtribusId(int subtribusid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubtribusId == subtribusid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Tribus-------------------
    public static async Task RemoveReferencesByTribusId(int tribusid)
    {
        var references = await Context.Tbl90References.Where(r => r.TribusId == tribusid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByTribusId(int tribusid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.TribusId == tribusid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Supertribus-------------------
    public static async Task RemoveReferencesBySupertribusId(int supertribusid)
    {
        var references = await Context.Tbl90References.Where(r => r.SupertribusId == supertribusid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySupertribusId(int supertribusid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SupertribusId == supertribusid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }
    //------------Infrafamily-------------------
    public static async Task RemoveReferencesByInfrafamilyId(int infrafamilyid)
    {
        var references = await Context.Tbl90References.Where(r => r.InfrafamilyId == infrafamilyid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByInfrafamilyId(int infrafamilyid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.InfrafamilyId == infrafamilyid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subfamily-------------------
    public static async Task RemoveReferencesBySubfamilyId(int subfamilyid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubfamilyId == subfamilyid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubfamilyId(int subfamilyid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubfamilyId == subfamilyid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }
    //------------Family-------------------
    public static async Task RemoveReferencesByFamilyId(int familyid)
    {
        var references = await Context.Tbl90References.Where(r => r.FamilyId == familyid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByFamilyId(int familyid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.FamilyId == familyid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Superfamily-------------------
    public static async Task RemoveReferencesBySuperfamilyId(int superfamilyid)
    {
        var references = await Context.Tbl90References.Where(r => r.SuperfamilyId == superfamilyid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySuperfamilyId(int superfamilyid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SuperfamilyId == superfamilyid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Infraordo-------------------
    public static async Task RemoveReferencesByInfraordoId(int infraordoid)
    {
        var references = await Context.Tbl90References.Where(r => r.InfraordoId == infraordoid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByInfraordoId(int infraordoid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.InfraordoId == infraordoid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subordo-------------------
    public static async Task RemoveReferencesBySubordoId(int subordoid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubordoId == subordoid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubordoId(int subordoid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubordoId == subordoid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Ordo-------------------
    public static async Task RemoveReferencesByOrdoId(int ordoid)
    {
        var references = await Context.Tbl90References.Where(r => r.OrdoId == ordoid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByOrdoId(int ordoid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.OrdoId == ordoid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Legio-------------------
    public static async Task RemoveReferencesByLegioId(int legioid)
    {
        var references = await Context.Tbl90References.Where(r => r.LegioId == legioid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByLegioId(int legioid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.LegioId == legioid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Infraclass-------------------
    public static async Task RemoveReferencesByInfraclassId(int infraclassid)
    {
        var references = await Context.Tbl90References.Where(r => r.InfraclassId == infraclassid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByInfraclassId(int infraclassid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.InfraclassId == infraclassid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subclass-------------------
    public static async Task RemoveReferencesBySubclassId(int subclassid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubclassId == subclassid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubclassId(int subclassid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubclassId == subclassid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Class-------------------
    public static async Task RemoveReferencesByClassId(int classid)
    {
        var references = await Context.Tbl90References.Where(r => r.ClassId == classid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByClassId(int classid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.ClassId == classid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Superclass-------------------
    public static async Task RemoveReferencesBySuperclassId(int superclassid)
    {
        var references = await Context.Tbl90References.Where(r => r.SuperclassId == superclassid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySuperclassId(int superclassid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SuperclassId == superclassid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subdivision-------------------
    public static async Task RemoveReferencesBySubdivisionId(int subdivisionid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubdivisionId == subdivisionid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubdivisionId(int subdivisionid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubdivisionId == subdivisionid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Subphylum-------------------
    public static async Task RemoveReferencesBySubphylumId(int subphylumid)
    {
        var references = await Context.Tbl90References.Where(r => r.SubphylumId == subphylumid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsBySubphylumId(int subphylumid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.SubphylumId == subphylumid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Division-------------------
    public static async Task RemoveReferencesByDivisionId(int divisionid)
    {
        var references = await Context.Tbl90References.Where(r => r.DivisionId == divisionid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByDivisionId(int divisionid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.DivisionId == divisionid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Phylum-------------------
    public static async Task RemoveReferencesByPhylumId(int phylumid)
    {
        var references = await Context.Tbl90References.Where(r => r.PhylumId == phylumid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByPhylumId(int phylumid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.PhylumId == phylumid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //------------Regnum-------------------
    public static async Task RemoveReferencesByRegnumId(int regnumid)
    {
        var references = await Context.Tbl90References.Where(r => r.RegnumId == regnumid).ToListAsync();
        if (references.Count != 0)
        {
            Context.Tbl90References.RemoveRange(references);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task RemoveCommentsByRegnumId(int regnumid)
    {
        var comments = await Context.Tbl93Comments.Where(c => c.RegnumId == regnumid).ToListAsync();
        if (comments.Count != 0)
        {
            Context.Tbl93Comments.RemoveRange(comments);
            await Context.SaveChangesAsync();
        }
    }

    //--------------------------------------------------------
    //---------------SaveToArray wichtig----------------------
    //public static async Task SaveToArrayTbl72PlSpeciesses(int genusid, List<Tbl72PlSpecies> arrPlSpecies)
    //{
    //    var plspeciesses = await Context.Tbl72PlSpeciesses.Where(a => a.GenusId == genusid)
    //        .ToListAsync();
    //    if (plspeciesses.Count != 0)
    //    {
    //        for (var index = 0; index < plspeciesses.Count; index++)
    //        {
    //            var plspecies = plspeciesses[index];
    //            arrPlSpecies.Add(plspecies);
    //        }
    //    }
    //}
    public static async Task SaveToArrayTbl75Speciesses(int genusid, List<Tbl75Species> arrSpecies)
    {
        var speciesses = await Context.Tbl75Speciesses.Where(a => a.GenusId == genusid)
            .ToListAsync();
        if (speciesses.Count != 0)
        {
            for (var index = 0; index < speciesses.Count; index++)
            {
                var species = speciesses[index];
                arrSpecies.Add(species);
            }
        }
    }
    public static async Task SaveToArrayTbl66Genusses(int infratribusid, List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var genusses = await Context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid)
            .ToListAsync();
        if (genusses.Count != 0)
        {
            for (var index = 0; index < genusses.Count; index++)
            {
                var genus = genusses[index];
                var genusid = genus.GenusId;
                arrGenus.Add(genus);
                await SaveToArrayTbl75Speciesses(genusid, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl63Infratribusses(int subtribusid, List<Tbl63Infratribus> arrInfratribus, List<Tbl66Genus> arrGenus,
        List<Tbl75Species> arrSpecies)
    {
        var infratribusses = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid)
            .ToListAsync();
        if (infratribusses.Count != 0)
        {
            for (var index = 0; index < infratribusses.Count; index++)
            {
                var infratribus = infratribusses[index];
                var infratribusid = infratribus.InfratribusId;
                arrInfratribus.Add(infratribus);
                await SaveToArrayTbl66Genusses(infratribusid, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl60Subtribusses(int tribusid, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subtribusses = await Context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
        if (subtribusses.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subtribusses.Count; index++)
            {
                var subtribus = subtribusses[index];
                var subtribusid = subtribus.SubtribusId;
                arrSubtribus.Add(subtribus);
                await SaveToArrayTbl63Infratribusses(subtribusid, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl57Tribusses(int supertribusid, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var tribusses = await Context.Tbl57Tribusses.Where(a => a.SupertribusId == supertribusid).ToListAsync();
        if (tribusses.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < tribusses.Count; index++)
            {
                var tribus = tribusses[index];
                var tribusid = tribus.TribusId;
                arrTribus.Add(tribus);
                await SaveToArrayTbl60Subtribusses(tribusid, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl54Supertribusses(int infrafamilyid, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var supertribusses = await Context.Tbl54Supertribusses.Where(a => a.InfrafamilyId == infrafamilyid).ToListAsync();
        if (supertribusses.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < supertribusses.Count; index++)
            {
                var supertribus = supertribusses[index];
                var supertribusid = supertribus.SupertribusId;
                arrSupertribus.Add(supertribus);
                await SaveToArrayTbl57Tribusses(supertribusid, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl51Infrafamilies(int subfamilyid, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var infrafamilys = await Context.Tbl51Infrafamilies.Where(a => a.SubfamilyId == subfamilyid).ToListAsync();
        if (infrafamilys.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < infrafamilys.Count; index++)
            {
                var infrafamily = infrafamilys[index];
                var infrafamilyid = infrafamily.InfrafamilyId;
                arrInfrafamily.Add(infrafamily);
                await SaveToArrayTbl54Supertribusses(infrafamilyid, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl48Subfamilies(int familyid, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subfamilys = await Context.Tbl48Subfamilies.Where(a => a.FamilyId == familyid).ToListAsync();
        if (subfamilys.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subfamilys.Count; index++)
            {
                var subfamily = subfamilys[index];
                var subfamilyid = subfamily.SubfamilyId;
                arrSubfamily.Add(subfamily);
                await SaveToArrayTbl51Infrafamilies(subfamilyid, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl45Families(int superfamilyid, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var familys = await Context.Tbl45Families.Where(a => a.SuperfamilyId == superfamilyid).ToListAsync();
        if (familys.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < familys.Count; index++)
            {
                var family = familys[index];
                var familyid = family.FamilyId;
                arrFamily.Add(family);
                await SaveToArrayTbl48Subfamilies(familyid, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl42Superfamilies(int infraordoid, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var superfamilys = await Context.Tbl42Superfamilies.Where(a => a.InfraordoId == infraordoid).ToListAsync();
        if (superfamilys.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < superfamilys.Count; index++)
            {
                var superfamily = superfamilys[index];
                var superfamilyid = superfamily.SuperfamilyId;
                arrSuperfamily.Add(superfamily);
                await SaveToArrayTbl45Families(superfamilyid, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl39Infraordos(int subordoid, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var infraordos = await Context.Tbl39Infraordos.Where(a => a.SubordoId == subordoid).ToListAsync();
        if (infraordos.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < infraordos.Count; index++)
            {
                var infraordo = infraordos[index];
                var infraordoid = infraordo.InfraordoId;
                arrInfraordo.Add(infraordo);
                await SaveToArrayTbl42Superfamilies(infraordoid, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl36Subordos(int ordoid, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subordos = await Context.Tbl36Subordos.Where(a => a.OrdoId == ordoid).ToListAsync();
        if (subordos.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subordos.Count; index++)
            {
                var subordo = subordos[index];
                var subordoid = subordo.SubordoId;
                arrSubordo.Add(subordo);
                await SaveToArrayTbl39Infraordos(subordoid, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl33Ordos(int legioid, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var ordos = await Context.Tbl33Ordos.Where(a => a.LegioId == legioid).ToListAsync();
        if (ordos.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < ordos.Count; index++)
            {
                var ordo = ordos[index];
                var ordoid = ordo.OrdoId;
                arrOrdo.Add(ordo);
                await SaveToArrayTbl36Subordos(ordoid, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl30Legios(int infraclassid, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var legios = await Context.Tbl30Legios.Where(a => a.InfraclassId == infraclassid).ToListAsync();
        if (legios.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < legios.Count; index++)
            {
                var legio = legios[index];
                var legioid = legio.LegioId;
                arrLegio.Add(legio);
                await SaveToArrayTbl33Ordos(legioid, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl27Infraclasses(int subclassid, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var infraclasss = await Context.Tbl27Infraclasses.Where(a => a.SubclassId == subclassid).ToListAsync();
        if (infraclasss.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < infraclasss.Count; index++)
            {
                var infraclass = infraclasss[index];
                var infraclassid = infraclass.InfraclassId;
                arrInfraclass.Add(infraclass);
                await SaveToArrayTbl30Legios(infraclassid, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl24Subclasses(int classid, List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subclasss = await Context.Tbl24Subclasses.Where(a => a.ClassId == classid).ToListAsync();
        if (subclasss.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subclasss.Count; index++)
            {
                var subclass = subclasss[index];
                var subclassid = subclass.SubclassId;
                arrSubclass.Add(subclass);
                await SaveToArrayTbl27Infraclasses(subclassid, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl21Classes(int superclassid, List<Tbl21Class> arrClass, List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var classes = await Context.Tbl21Classes.Where(a => a.SuperclassId == superclassid).ToListAsync();
        if (classes.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < classes.Count; index++)
            {
                var classe = classes[index];
                var classid = classe.ClassId;
                arrClass.Add(classe);
                await SaveToArrayTbl24Subclasses(classid, arrSubclass, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl18Superclasses(int subdivisionid, List<Tbl18Superclass> arrSuperclass, List<Tbl21Class> arrClass, List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var superclasses = await Context.Tbl18Superclasses.Where(a => a.SubdivisionId == subdivisionid).ToListAsync();
        if (superclasses.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < superclasses.Count; index++)
            {
                var superclass = superclasses[index];
                var superclassid = superclass.SuperclassId;
                arrSuperclass.Add(superclass);
                await SaveToArrayTbl21Classes(superclassid, arrClass, arrSubclass, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }
    public static async Task SaveToArrayTbl15Subdivisions(int divisionid, List<Tbl15Subdivision> arrSubdivision, List<Tbl18Superclass> arrSuperclass, List<Tbl21Class> arrClass, List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subdivisions = await Context.Tbl15Subdivisions.Where(a => a.DivisionId == divisionid).ToListAsync();
        if (subdivisions.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subdivisions.Count; index++)
            {
                var subdivision = subdivisions[index];
                var subdivisionid = subdivision.SubdivisionId;
                arrSubdivision.Add(subdivision);
                await SaveToArrayTbl18Superclasses(subdivisionid, arrSuperclass, arrClass, arrSubclass, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl12Subphylums(int phylumid, List<Tbl12Subphylum> arrSubphylum, List<Tbl18Superclass> arrSuperclass, List<Tbl21Class> arrClass, List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio, List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo, List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily, List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus, List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var subphylums = await Context.Tbl12Subphylums.Where(a => a.PhylumId == phylumid).ToListAsync();
        if (subphylums.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < subphylums.Count; index++)
            {
                var subphylum = subphylums[index];
                var subphylumid = subphylum.SubphylumId;
                arrSubphylum.Add(subphylum);
                await SaveToArrayTbl18Superclasses(subphylumid, arrSuperclass, arrClass, arrSubclass, arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily, arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl09Divisions(int regnumid, List<Tbl09Division> arrDivision,
        List<Tbl15Subdivision> arrSubdivision, List<Tbl18Superclass> arrSuperclass, List<Tbl21Class> arrClass,
        List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio,
        List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo,
        List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily,
        List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus,
        List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var divisions = await Context.Tbl09Divisions.Where(a => a.RegnumId == regnumid).ToListAsync();
        if (divisions.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < divisions.Count; index++)
            {
                var division = divisions[index];
                var divisionid = division.DivisionId;
                arrDivision.Add(division);
                await SaveToArrayTbl15Subdivisions(divisionid, arrSubdivision, arrSuperclass, arrClass, arrSubclass,
                    arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily,
                    arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    public static async Task SaveToArrayTbl06Phylums(int regnumid, List<Tbl06Phylum> arrPhylum,
        List<Tbl12Subphylum> arrSubphylum, List<Tbl18Superclass> arrSuperclass, List<Tbl21Class> arrClass,
        List<Tbl24Subclass> arrSubclass, List<Tbl27Infraclass> arrInfraclass, List<Tbl30Legio> arrLegio,
        List<Tbl33Ordo> arrOrdo, List<Tbl36Subordo> arrSubordo, List<Tbl39Infraordo> arrInfraordo,
        List<Tbl42Superfamily> arrSuperfamily, List<Tbl45Family> arrFamily, List<Tbl48Subfamily> arrSubfamily,
        List<Tbl51Infrafamily> arrInfrafamily, List<Tbl54Supertribus> arrSupertribus, List<Tbl57Tribus> arrTribus,
        List<Tbl60Subtribus> arrSubtribus, List<Tbl63Infratribus> arrInfratribus,
        List<Tbl66Genus> arrGenus, List<Tbl75Species> arrSpecies)
    {
        var phylums = await Context.Tbl06Phylums.Where(a => a.RegnumId == regnumid).ToListAsync();
        if (phylums.Count != 0)
        {
            //Save  Array
            for (var index = 0; index < phylums.Count; index++)
            {
                var phylum = phylums[index];
                var phylumid = phylum.PhylumId;
                arrPhylum.Add(phylum);
                await SaveToArrayTbl12Subphylums(phylumid, arrSubphylum, arrSuperclass, arrClass, arrSubclass,
                    arrInfraclass, arrLegio, arrOrdo, arrSubordo, arrInfraordo, arrSuperfamily, arrFamily, arrSubfamily,
                    arrInfrafamily, arrSupertribus, arrTribus, arrSubtribus, arrInfratribus, arrGenus, arrSpecies);
            }
        }
    }

    //--------------------Remove Array wichtig-----------------------
    //public static async Task RemoveArrPlSpeciesByPlSpeciesId(List<Tbl72PlSpecies> arrPlSpecies)
    //{
    //    if (arrPlSpecies.Count != 0)
    //    {
    //        for (var index = 0; index < arrPlSpecies.Count; index++)
    //        {
    //            var plspecies = arrPlSpecies[index];
    //            var plspeciesid = plspecies.PlSpeciesId;

    //            await RemoveReferencesByPlSpeciesId(plspeciesid);
    //            await RemoveCommentsByPlSpeciesId(plspeciesid);
    //            await RemoveNamesByPlSpeciesId(plspeciesid);
    //            await RemoveImagesByPlSpeciesId(plspeciesid);
    //            await RemoveSynonymsByPlSpeciesId(plspeciesid);
    //            await RemoveGeographicsByPlSpeciesId(plspeciesid);
    //        }
    //        Context.Tbl72PlSpeciesses.RemoveRange(arrPlSpecies);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    public static async Task RemoveArrSpeciesBySpeciesId(List<Tbl75Species> arrSpecies)
    {
        if (arrSpecies.Count != 0)
        {
            for (var index = 0; index < arrSpecies.Count; index++)
            {
                var species = arrSpecies[index];
                var speciesid = species.SpeciesId;

                await RemoveReferencesBySpeciesId(speciesid);
                await RemoveCommentsBySpeciesId(speciesid);
                await RemoveNamesBySpeciesId(speciesid);
                await RemoveImagesBySpeciesId(speciesid);
                await RemoveSynonymsBySpeciesId(speciesid);
                await RemoveGeographicsBySpeciesId(speciesid);
            }
            Context.Tbl75Speciesses.RemoveRange(arrSpecies);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrGenusByGenusId(List<Tbl66Genus> arrGenus)
    {
        if (arrGenus.Count != 0)
        {
            for (var index = 0; index < arrGenus.Count; index++)
            {
                var genus = arrGenus[index];
                var genusid = genus.GenusId;
                await RemoveReferencesByGenusId(genusid);
                await RemoveCommentsByGenusId(genusid);
            }
            Context.Tbl66Genusses.RemoveRange(arrGenus);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrInfratribusByInfratribusId(List<Tbl63Infratribus> arrInfratribus)
    {
        if (arrInfratribus.Count != 0)
        {
            for (var index = 0; index < arrInfratribus.Count; index++)
            {
                var infratribus = arrInfratribus[index];
                var infratribusid = infratribus.InfratribusId;
                await RemoveReferencesByInfratribusId(infratribusid);
                await RemoveCommentsByInfratribusId(infratribusid);
            }
            Context.Tbl63Infratribusses.RemoveRange(arrInfratribus);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubtribusBySubtribusId(List<Tbl60Subtribus> arrSubtribus)
    {
        if (arrSubtribus.Count != 0)
        {
            for (var index = 0; index < arrSubtribus.Count; index++)
            {
                var subtribus = arrSubtribus[index];
                var subtribusid = subtribus.SubtribusId;
                await RemoveReferencesBySubtribusId(subtribusid);
                await RemoveCommentsBySubtribusId(subtribusid);
            }
            Context.Tbl60Subtribusses.RemoveRange(arrSubtribus);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrTribusByTribusId(List<Tbl57Tribus> arrTribus)
    {
        if (arrTribus.Count != 0)
        {
            for (var index = 0; index < arrTribus.Count; index++)
            {
                var tribus = arrTribus[index];
                var tribusid = tribus.TribusId;
                await RemoveReferencesByTribusId(tribusid);
                await RemoveCommentsByTribusId(tribusid);
            }
            Context.Tbl57Tribusses.RemoveRange(arrTribus);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSupertribusBySupertribusId(List<Tbl54Supertribus> arrSupertribus)
    {
        if (arrSupertribus.Count != 0)
        {
            for (var index = 0; index < arrSupertribus.Count; index++)
            {
                var supertribus = arrSupertribus[index];
                var supertribusid = supertribus.SupertribusId;
                await RemoveReferencesBySupertribusId(supertribusid);
                await RemoveCommentsBySupertribusId(supertribusid);
            }
            Context.Tbl54Supertribusses.RemoveRange(arrSupertribus);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrInfrafamilyByInfrafamilyId(List<Tbl51Infrafamily> arrInfrafamily)
    {
        if (arrInfrafamily.Count != 0)
        {
            for (var index = 0; index < arrInfrafamily.Count; index++)
            {
                var infrafamily = arrInfrafamily[index];
                var infrafamilyid = infrafamily.InfrafamilyId;
                await RemoveReferencesByInfrafamilyId(infrafamilyid);
                await RemoveCommentsByInfrafamilyId(infrafamilyid);
            }
            Context.Tbl51Infrafamilies.RemoveRange(arrInfrafamily);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubfamilyBySubfamilyId(List<Tbl48Subfamily> arrSubfamily)
    {
        if (arrSubfamily.Count != 0)
        {
            for (var index = 0; index < arrSubfamily.Count; index++)
            {
                var subfamily = arrSubfamily[index];
                var subfamilyid = subfamily.SubfamilyId;
                await RemoveReferencesBySubfamilyId(subfamilyid);
                await RemoveCommentsBySubfamilyId(subfamilyid);
            }
            Context.Tbl48Subfamilies.RemoveRange(arrSubfamily);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrFamilyByFamilyId(List<Tbl45Family> arrFamily)
    {
        if (arrFamily.Count != 0)
        {
            for (var index = 0; index < arrFamily.Count; index++)
            {
                var family = arrFamily[index];
                var familyid = family.FamilyId;
                await RemoveReferencesByFamilyId(familyid);
                await RemoveCommentsByFamilyId(familyid);
            }
            Context.Tbl45Families.RemoveRange(arrFamily);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSuperfamilyBySuperfamilyId(List<Tbl42Superfamily> arrSuperfamily)
    {
        if (arrSuperfamily.Count != 0)
        {
            for (var index = 0; index < arrSuperfamily.Count; index++)
            {
                var superfamily = arrSuperfamily[index];
                var superfamilyid = superfamily.SuperfamilyId;
                await RemoveReferencesBySuperfamilyId(superfamilyid);
                await RemoveCommentsBySuperfamilyId(superfamilyid);
            }
            Context.Tbl42Superfamilies.RemoveRange(arrSuperfamily);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrInfraordoByInfraordoId(List<Tbl39Infraordo> arrInfraordo)
    {
        if (arrInfraordo.Count != 0)
        {
            for (var index = 0; index < arrInfraordo.Count; index++)
            {
                var infraordo = arrInfraordo[index];
                var infraordoid = infraordo.InfraordoId;
                await RemoveReferencesByInfraordoId(infraordoid);
                await RemoveCommentsByInfraordoId(infraordoid);
            }
            Context.Tbl39Infraordos.RemoveRange(arrInfraordo);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubordoBySubordoId(List<Tbl36Subordo> arrSubordo)
    {
        if (arrSubordo.Count != 0)
        {
            for (var index = 0; index < arrSubordo.Count; index++)
            {
                var subordo = arrSubordo[index];
                var subordoid = subordo.SubordoId;
                await RemoveReferencesBySubordoId(subordoid);
                await RemoveCommentsBySubordoId(subordoid);
            }
            Context.Tbl36Subordos.RemoveRange(arrSubordo);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrOrdoByOrdoId(List<Tbl33Ordo> arrOrdo)
    {
        if (arrOrdo.Count != 0)
        {
            for (var index = 0; index < arrOrdo.Count; index++)
            {
                var ordo = arrOrdo[index];
                var ordoid = ordo.OrdoId;
                await RemoveReferencesByOrdoId(ordoid);
                await RemoveCommentsByOrdoId(ordoid);
            }
            Context.Tbl33Ordos.RemoveRange(arrOrdo);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrLegioByLegioId(List<Tbl30Legio> arrLegio)
    {
        if (arrLegio.Count != 0)
        {
            for (var index = 0; index < arrLegio.Count; index++)
            {
                var legio = arrLegio[index];
                var legioid = legio.LegioId;
                await RemoveReferencesByLegioId(legioid);
                await RemoveCommentsByLegioId(legioid);
            }
            Context.Tbl30Legios.RemoveRange(arrLegio);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrInfraclassByInfraclassId(List<Tbl27Infraclass> arrInfraclass)
    {
        if (arrInfraclass.Count != 0)
        {
            for (var index = 0; index < arrInfraclass.Count; index++)
            {
                var infraclass = arrInfraclass[index];
                var infraclassid = infraclass.InfraclassId;
                await RemoveReferencesByInfraclassId(infraclassid);
                await RemoveCommentsByInfraclassId(infraclassid);
            }
            Context.Tbl27Infraclasses.RemoveRange(arrInfraclass);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubclassBySubclassId(List<Tbl24Subclass> arrSubclass)
    {
        if (arrSubclass.Count != 0)
        {
            for (var index = 0; index < arrSubclass.Count; index++)
            {
                var subclass = arrSubclass[index];
                var subclassid = subclass.SubclassId;
                await RemoveReferencesBySubclassId(subclassid);
                await RemoveCommentsBySubclassId(subclassid);
            }
            Context.Tbl24Subclasses.RemoveRange(arrSubclass);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrClassByClassId(List<Tbl21Class> arrClass)
    {
        if (arrClass.Count != 0)
        {
            for (var index = 0; index < arrClass.Count; index++)
            {
                var classs = arrClass[index];
                var classid = classs.ClassId;
                await RemoveReferencesByClassId(classid);
                await RemoveCommentsByClassId(classid);
            }
            Context.Tbl21Classes.RemoveRange(arrClass);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSuperclassBySuperclassId(List<Tbl18Superclass> arrSuperclass)
    {
        if (arrSuperclass.Count != 0)
        {
            for (var index = 0; index < arrSuperclass.Count; index++)
            {
                var superclass = arrSuperclass[index];
                var superclassid = superclass.SuperclassId;
                await RemoveReferencesBySuperclassId(superclassid);
                await RemoveCommentsBySuperclassId(superclassid);
            }
            Context.Tbl18Superclasses.RemoveRange(arrSuperclass);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubdivisionBySubdivisionId(List<Tbl15Subdivision> arrSubdivision)
    {
        if (arrSubdivision.Count != 0)
        {
            for (var index = 0; index < arrSubdivision.Count; index++)
            {
                var subdivision = arrSubdivision[index];
                var subdivisionid = subdivision.SubdivisionId;
                await RemoveReferencesBySubdivisionId(subdivisionid);
                await RemoveCommentsBySubdivisionId(subdivisionid);
            }
            Context.Tbl15Subdivisions.RemoveRange(arrSubdivision);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrSubphylumBySubphylumId(List<Tbl12Subphylum> arrSubphylum)
    {
        if (arrSubphylum.Count != 0)
        {
            for (var index = 0; index < arrSubphylum.Count; index++)
            {
                var subphylum = arrSubphylum[index];
                var subphylumid = subphylum.SubphylumId;
                await RemoveReferencesBySubphylumId(subphylumid);
                await RemoveCommentsBySubphylumId(subphylumid);
            }
            Context.Tbl12Subphylums.RemoveRange(arrSubphylum);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrDivisionByDivisionId(List<Tbl09Division> arrDivision)
    {
        if (arrDivision.Count != 0)
        {
            for (var index = 0; index < arrDivision.Count; index++)
            {
                var division = arrDivision[index];
                var divisionid = division.DivisionId;
                await RemoveReferencesByDivisionId(divisionid);
                await RemoveCommentsByDivisionId(divisionid);
            }
            Context.Tbl09Divisions.RemoveRange(arrDivision);
            await Context.SaveChangesAsync();
        }
    }
    public static async Task RemoveArrPhylumByPhylumId(List<Tbl06Phylum> arrPhylum)
    {
        if (arrPhylum.Count != 0)
        {
            for (var index = 0; index < arrPhylum.Count; index++)
            {
                var phylum = arrPhylum[index];
                var phylumid = phylum.PhylumId;
                await RemoveReferencesByPhylumId(phylumid);
                await RemoveCommentsByPhylumId(phylumid);
            }
            Context.Tbl06Phylums.RemoveRange(arrPhylum);
            await Context.SaveChangesAsync();
        }
    }
    //-------------------------------------------













    //-------------------Table-----------------------

    //public static async Task RemoveTbl69FiSpeciessesByGenusId(int genusid)
    //{
    //    var fispeciesses = await Context.Tbl69FiSpeciesses.Where(a => a.GenusId == genusid).ToListAsync();
    //    if (fispeciesses.Count != 0)
    //    {
    //        for (var index1 = 0; index1 < fispeciesses.Count; index1++)
    //        {
    //            var fispecies = fispeciesses[index1];
    //            var fispeciesid = fispecies.FiSpeciesId;

    //            await RemoveReferencesByFiSpeciesId(fispeciesid);
    //            await RemoveCommentsByFiSpeciesId(fispeciesid);
    //            await RemoveNamesByFiSpeciesId(fispeciesid);
    //            await RemoveImagesByFiSpeciesId(fispeciesid);
    //            await RemoveSynonymsByFiSpeciesId(fispeciesid);
    //            await RemoveGeographicsByFiSpeciesId(fispeciesid);
    //        }
    //        Context.Tbl69FiSpeciesses.RemoveRange(fispeciesses);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveTbl72PlSpeciessesByGenusId(int genusid)
    //{
    //    var plspeciesses = await Context.Tbl72PlSpeciesses.Where(a => a.GenusId == genusid).ToListAsync();
    //    if (plspeciesses.Count != 0)
    //    {
    //        for (var index = 0; index < plspeciesses.Count; index++)
    //        {
    //            var plspecies = plspeciesses[index];
    //            var plspeciesid = plspecies.PlSpeciesId;

    //            await RemoveReferencesByPlSpeciesId(plspeciesid);
    //            await RemoveCommentsByPlSpeciesId(plspeciesid);
    //            await RemoveNamesByPlSpeciesId(plspeciesid);
    //            await RemoveImagesByPlSpeciesId(plspeciesid);
    //            await RemoveSynonymsByPlSpeciesId(plspeciesid);
    //            await RemoveGeographicsByPlSpeciesId(plspeciesid);
    //        }
    //        Context.Tbl72PlSpeciesses.RemoveRange(plspeciesses);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    ////-------------------------------------------
    //public static async Task RemoveTbl66GenussesByInfratribusId(int infratribusid)
    //{
    //    var genusses = await Context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid).ToListAsync();
    //    if (genusses.Count != 0)
    //    {
    //        for (var index = 0; index < genusses.Count; index++)
    //        {
    //            var genus = genusses[index];
    //            var genusid = genus.GenusId;
    //            //----------Tbl69FiSpeciesses--------------------
    //            await RemoveTbl69FiSpeciessesByGenusId(genusid);
    //            //----------Tbl72PlSpeciesses--------------------
    //            await RemoveTbl72PlSpeciessesByGenusId(genusid);
    //            //----------references comments--------------------
    //            //references genus
    //            await RemoveReferencesByGenusId(genusid);
    //            //comments genus
    //            await RemoveCommentsByGenusId(genusid);
    //        }
    //        //--------------Tbl66Genusses-------------------------
    //        Context.Tbl66Genusses.RemoveRange(genusses);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    ////-------------------------------------------
    //public static async Task RemoveTbl63InfratribussesBySubtribusId(int subtribusid)
    //{
    //    var infratribuss = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //    if (infratribuss.Count != 0)
    //    {
    //        for (var index = 0; index < infratribuss.Count; index++)
    //        {
    //            var infratribus = infratribuss[index];
    //            var infratribusid = infratribus.InfratribusId;
    //            //----------Tbl66Genusses--------------------
    //            await RemoveTbl66GenussesByInfratribusId(infratribusid);
    //            //----------references comments--------------------
    //            //references infratribus
    //            await RemoveReferencesByInfratribusId(infratribusid);
    //            //comments infratribus
    //            await RemoveCommentsByInfratribusId(infratribusid);
    //        }
    //        //--------------Tbl63Infratribusses-------------------------
    //        Context.Tbl63Infratribusses.RemoveRange(infratribuss);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveTbl63InfratribussesBySubtribusId(int subtribusid)
    //{
    //    var infratribusses = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //    if (infratribusses.Count != 0)
    //    {
    //        for (var index = 0; index < infratribusses.Count; index++)
    //        {
    //            var infratribus = infratribusses[index];
    //            var infratribusid = infratribus.InfratribusId;
    //            //----------Tbl66Genusses--------------------
    //            await RemoveTbl66GenussesByInfratribusId(infratribusid);
    //            //----------references comments--------------------
    //            //references infratribus
    //            await RemoveReferencesByInfratribusId(infratribusid);
    //            //comments infratribus
    //            await RemoveCommentsByInfratribusId(infratribusid);
    //        }
    //        //--------------Tbl63Infratribusses-------------------------
    //        Context.Tbl63Infratribusses.RemoveRange(infratribusses);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //-------------------------------------------
    //---------------------single----------------------


    //public static async Task RemoveTbl60SubtribussesByTribusId(int tribusid)
    //{
    //    var subtribuss = await Context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
    //    if (subtribuss.Count != 0)
    //    {
    //        for (var index = 0; index < subtribuss.Count; index++)
    //        {
    //            var subtribus = subtribuss[index];
    //            var subtribusid = subtribus.SubtribusId;
    //            //----------Tbl63Infratribusses--------------------
    //            await RemoveTbl63InfratribussesBySubtribusId(subtribusid);
    //            //----------references comments--------------------
    //            //references subtribus
    //            await RemoveReferencesBySubtribusId(subtribusid);
    //            //comments subtribus
    //            await RemoveCommentsBySubtribusId(subtribusid);
    //        }
    //        //--------------Tbl60Subtribusses-------------------------
    //        Context.Tbl60Subtribusses.RemoveRange(subtribuss);
    //        await Context.SaveChangesAsync();
    //    }
    //}




    //-------------------Single-----------------------

    //public static async Task RemoveSingleFiSpeciesByFiSpeciesId(int fispeciesid)
    //{
    //    var fispecies = await Context.Tbl69FiSpeciesses.FirstOrDefaultAsync(b => b.FiSpeciesId == fispeciesid);
    //    if (fispecies != null)
    //    {
    //        await RemoveReferencesByFiSpeciesId(fispeciesid);
    //        await RemoveCommentsByFiSpeciesId(fispeciesid);
    //        await RemoveNamesByFiSpeciesId(fispeciesid);
    //        await RemoveImagesByFiSpeciesId(fispeciesid);
    //        await RemoveSynonymsByFiSpeciesId(fispeciesid);
    //        await RemoveGeographicsByFiSpeciesId(fispeciesid);

    //        Context.Tbl69FiSpeciesses.Remove(fispecies);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveSinglePlSpeciesByPlSpeciesId(int plspeciesid)
    //{
    //    var plspecies = await Context.Tbl72PlSpeciesses.FirstOrDefaultAsync(b => b.PlSpeciesId == plspeciesid);
    //    if (plspecies != null)
    //    {
    //        await RemoveReferencesByPlSpeciesId(plspeciesid);
    //        await RemoveCommentsByPlSpeciesId(plspeciesid);
    //        await RemoveNamesByPlSpeciesId(plspeciesid);
    //        await RemoveImagesByPlSpeciesId(plspeciesid);
    //        await RemoveSynonymsByPlSpeciesId(plspeciesid);
    //        await RemoveGeographicsByPlSpeciesId(plspeciesid);

    //        Context.Tbl72PlSpeciesses.Remove(plspecies);
    //        await Context.SaveChangesAsync();
    //    }

    //}

    //public static async Task RemoveSingleGenusByGenusId(int genusid)
    //{
    //    var genus = await Context.Tbl66Genusses.FirstOrDefaultAsync(b => b.GenusId == genusid);
    //    if (genus != null)
    //    {
    //        //----------Tbl69FiSpeciesses--------------------
    //        await RemoveTbl69FiSpeciessesByGenusId(genusid);
    //        //----------Tbl72PlSpeciesses--------------------
    //        await RemoveTbl72PlSpeciessesByGenusId(genusid);
    //        //--------------Tbl66Genusses-------------------------
    //        //references genus
    //        await RemoveReferencesByGenusId(genusid);
    //        //comments genus
    //        await RemoveCommentsByGenusId(genusid);

    //        Context.Tbl66Genusses.Remove(genus);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveSingleInfratribusByInfratribusId(int infratribusid)
    //{
    //    var infratribus = await Context.Tbl63Infratribusses.FirstOrDefaultAsync(b => b.InfratribusId == infratribusid);
    //    if (infratribus != null)
    //    {
    //        //----------Tbl66Genusses--------------------
    //        var genusses = await Context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid).ToListAsync();
    //        if (genusses.Count != 0)
    //        {
    //            for (var index = 0; index < genusses.Count; index++)
    //            {
    //                var genus = genusses[index];
    //                var genusid = genus.GenusId;
    //                //----------Tbl69FiSpeciesses--------------------
    //                await RemoveTbl69FiSpeciessesByGenusId(genusid);
    //                //----------Tbl72PlSpeciesses--------------------
    //                await RemoveTbl72PlSpeciessesByGenusId(genusid);
    //                //----------references comments--------------------
    //                //references genus
    //                await RemoveReferencesByGenusId(genusid);
    //                //comments genus
    //                await RemoveCommentsByGenusId(genusid);
    //            }
    //            //--------------Tbl66Genusses-------------------------
    //            Context.Tbl66Genusses.RemoveRange(genusses);
    //            await Context.SaveChangesAsync();
    //        }
    //        //--------------Tbl63Infratribusses-------------------------
    //        //references infratribus
    //        await RemoveReferencesByInfratribusId(infratribusid);
    //        //comments infratribus
    //        await RemoveCommentsByInfratribusId(infratribusid);

    //        Context.Tbl63Infratribusses.Remove(infratribus);
    //        await Context.SaveChangesAsync();
    //    }
    //}
    //public static async Task RemoveSubtribusBySubtribusId(int subtribusid)
    //{
    //    var subtribus = await Context.Tbl60Subtribusses.FirstOrDefaultAsync(b => b.SubtribusId == subtribusid);
    //    if (subtribus != null)
    //    {
    //        //----------Tbl63Infratribusses--------------------
    //        var infratribuss = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //        if (infratribuss.Count != 0)
    //        {
    //            for (var index = 0; index < infratribuss.Count; index++)
    //            {
    //                var infratribus = infratribuss[index];
    //                var infratribusid = infratribus.InfratribusId;
    //                //----------Tbl66Genusses--------------------
    //                await RemoveTbl66GenussesByInfratribusId(infratribusid);
    //                //----------references comments--------------------
    //                //references infratribus
    //                await RemoveReferencesByInfratribusId(infratribusid);
    //                //comments infratribus
    //                await RemoveCommentsByInfratribusId(infratribusid);
    //            }
    //            //--------------Tbl63Infratribusses-------------------------
    //            Context.Tbl63Infratribusses.RemoveRange(infratribuss);
    //            await Context.SaveChangesAsync();
    //        }
    //        //--------------Tbl60Subtribusses-------------------------
    //        //references subtribus
    //        await RemoveReferencesBySubtribusId(subtribusid);
    //        //comments subtribus
    //        await RemoveCommentsBySubtribusId(subtribusid);

    //        Context.Tbl60Subtribusses.Remove(subtribus);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveSingleSubtribusBySubtribusId(int subtribusid)
    //{
    //    var subtribus = await Context.Tbl60Subtribusses.FirstOrDefaultAsync(b => b.SubtribusId == subtribusid);
    //    if (subtribus != null)
    //    {
    //        //----------Tbl63Infratribusses--------------------
    //        var infratribuss = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //        if (infratribuss.Count != 0)
    //        {
    //            for (var index = 0; index < infratribuss.Count; index++)
    //            {
    //                var infratribus = infratribuss[index];
    //                var infratribusid = infratribus.InfratribusId;
    //                //----------Tbl66Genusses--------------------
    //                await RemoveTbl66GenussesByInfratribusId(infratribusid);
    //                //----------references comments--------------------
    //                //references infratribus
    //                await RemoveReferencesByInfratribusId(infratribusid);
    //                //comments infratribus
    //                await RemoveCommentsByInfratribusId(infratribusid);
    //            }
    //            //--------------Tbl63Infratribusses-------------------------
    //            Context.Tbl63Infratribusses.RemoveRange(infratribuss);
    //            await Context.SaveChangesAsync();
    //        }
    //        //--------------Tbl60Subtribusses-------------------------
    //        //references subtribus
    //        await RemoveReferencesBySubtribusId(subtribusid);
    //        //comments subtribus
    //        await RemoveCommentsBySubtribusId(subtribusid);

    //        Context.Tbl60Subtribusses.Remove(subtribus);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //public static async Task RemoveSubtribusBySubtribusId(int subtribusid)
    //{
    //    var subtribus = await Context.Tbl60Subtribusses.FirstOrDefaultAsync(b => b.SubtribusId == subtribusid);
    //    if (subtribus != null)
    //    {
    //        //----------Tbl63Infratribusses--------------------
    //        var infratribusses = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //        if (infratribusses.Count != 0)
    //        {
    //            for (var index = 0; index < infratribusses.Count; index++)
    //            {
    //                var infratribus = infratribusses[index];
    //                var infratribusid = infratribus.InfratribusId;
    //                //----------Tbl66Genusses--------------------
    //                await RemoveTbl66GenussesByInfratribusId(infratribusid);
    //                //----------references comments--------------------
    //                //references infratribus
    //                await RemoveReferencesByInfratribusId(infratribusid);
    //                //comments infratribus
    //                await RemoveCommentsByInfratribusId(infratribusid);
    //            }
    //            //--------------Tbl63Infratribusses-------------------------
    //            Context.Tbl63Infratribusses.RemoveRange(infratribusses);
    //            await Context.SaveChangesAsync();
    //        }
    //        //--------------Tbl60Subtribusses-------------------------
    //        //references subtribus
    //        await RemoveReferencesBySubtribusId(subtribusid);
    //        //comments subtribus
    //        await RemoveCommentsBySubtribusId(subtribusid);

    //        Context.Tbl60Subtribusses.Remove(subtribus);
    //        await Context.SaveChangesAsync();
    //    }
    //}


    //public static async Task RemoveSingleTribusByTribusId(int tribusid)
    //{
    //    var tribus = await Context.Tbl57Tribusses.FirstOrDefaultAsync(b => b.TribusId == tribusid);
    //    if (tribus != null)
    //    {
    //        //----------Tbl60Subtribusses--------------------
    //        var subtribuss = await Context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
    //        if (subtribuss.Count != 0)
    //        {
    //            for (var index = 0; index < subtribuss.Count; index++)
    //            {
    //                var subtribus = subtribuss[index];
    //                var subtribusid = subtribus.SubtribusId;
    //                //----------Tbl63Infratribusses--------------------
    //                await RemoveTbl63InfratribussesBySubtribusId(subtribusid);
    //                //----------references comments--------------------
    //                //references subtribus
    //                await RemoveReferencesBySubtribusId(subtribusid);
    //                //comments subtribus
    //                await RemoveCommentsBySubtribusId(subtribusid);
    //            }
    //            //--------------Tbl60Subtribusses-------------------------
    //            Context.Tbl60Subtribusses.RemoveRange(subtribuss);
    //            await Context.SaveChangesAsync();
    //        }
    //        //--------------Tbl57Tribusses-------------------------
    //        //references tribus
    //        await RemoveReferencesByTribusId(tribusid);
    //        //comments tribus
    //        await RemoveCommentsByTribusId(tribusid);

    //        Context.Tbl57Tribusses.Remove(tribus);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    //-------------------------------------------
    //-------------------------------------------

    //public static async Task RemoveReferenceAndComment(int regnumid)
    //{
    //    var references = await Context.Tbl90References.Where(r => r.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    // //   await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    // //   await Context.SaveChangesAsync();
    //}

    //public static async Task<int> RemoveTbl06Phylums(int regnumid)
    //{
    //    var phylumid = 0;
    //    var phylums = await Context.Tbl06Phylums.Where(a => a.RegnumId == regnumid).ToListAsync();
    //    foreach (var phylum in phylums)
    //    {
    //        for (var i = 0; i <= phylums.Count; i++)
    //        {
    //            phylumid = phylum.PhylumId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.PhylumId == phylumid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.PhylumId == phylumid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl06Phylums.RemoveRange(phylums);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen regnums löschen 
    //    var references = await Context.Tbl90References.Where(r => r.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return phylumid;
    //}
    //public static async Task<int> RemoveTbl09Divisions(int regnumid)
    //{
    //    var divisionid = 0;
    //    var divisions = await Context.Tbl09Divisions.Where(a => a.RegnumId == regnumid).ToListAsync();
    //    foreach (var division in divisions)
    //    {
    //        for (var i = 0; i <= divisions.Count; i++)
    //        {
    //            divisionid = division.DivisionId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.DivisionId == divisionid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.DivisionId == divisionid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }

    //    Context.Tbl09Divisions.RemoveRange(divisions);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen regnums löschen 
    //    var references = await Context.Tbl90References.Where(r => r.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.RegnumId == regnumid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return divisionid;
    //}
    //public static async Task<int> RemoveTbl12Subphylums(int phylumid)
    //{
    //    var subphylumid = 0;
    //    var subphylums = await Context.Tbl12Subphylums.Where(a => a.PhylumId == phylumid).ToListAsync();
    //    foreach (var subphylum in subphylums)
    //    {
    //        for (var i = 0; i <= subphylums.Count; i++)
    //        {
    //            subphylumid = subphylum.SubphylumId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubphylumId == subphylumid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }
    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubphylumId == subphylumid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl12Subphylums.RemoveRange(subphylums);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen phylums löschen 
    //    var references = await Context.Tbl90References.Where(r => r.PhylumId == phylumid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.PhylumId == phylumid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subphylumid;
    //}

    //public static async Task<int> RemoveTbl15Subdivisions(int divisionid)
    //{
    //    var subdivisionid = 0;
    //    var subdivisions = await Context.Tbl15Subdivisions.Where(a => a.DivisionId == divisionid).ToListAsync();
    //    foreach (var subdivision in subdivisions)
    //    {
    //        for (var i = 0; i <= subdivisions.Count; i++)
    //        {
    //            subdivisionid = subdivision.SubdivisionId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubdivisionId == subdivisionid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubdivisionId == subdivisionid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl15Subdivisions.RemoveRange(subdivisions);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen divisions löschen 
    //    var references = await Context.Tbl90References.Where(r => r.DivisionId == divisionid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.DivisionId == divisionid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subdivisionid;
    //}


    //public static async Task<int> RemoveTbl18Superclasses(int subphylumid,int subdivisionid)
    //{
    //    //hier noch überarbeiten
    //    var superclassid = 0;
    //    var superclasss = await Context.Tbl18Superclasses.Where(a => a.SubdivisionId == subdivisionid).ToListAsync();
    //    foreach (var superclass in superclasss)
    //    {
    //        for (var i = 0; i <= superclasss.Count; i++)
    //        {
    //            superclassid = superclass.SuperclassId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SuperclassId == superclassid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SuperclassId == superclassid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl18Superclasses.RemoveRange(superclasss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen subdivisions löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SubdivisionId == subdivisionid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SubdivisionId == subdivisionid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return superclassid;
    //}

    //public static async Task<int> RemoveTbl21Classes(int superclassid)
    //{
    //    var classeid = 0;
    //    var classes = await Context.Tbl21Classes.Where(a => a.SuperclassId == superclassid).ToListAsync();
    //    foreach (var classe in classes)
    //    {
    //        for (var i = 0; i <= classes.Count; i++)
    //        {
    //            classeid = classe.ClassId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.ClassId == classeid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.ClassId == classeid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl21Classes.RemoveRange(classes);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen superclasss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SuperclassId == superclassid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SuperclassId == superclassid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return classeid;
    //}


    //public static async Task<int> RemoveTbl24Subclasses(int classid)
    //{
    //    var subclassid = 0;
    //    var subclasss = await Context.Tbl24Subclasses.Where(a => a.ClassId == classid).ToListAsync();
    //    foreach (var subclass in subclasss)
    //    {
    //        for (var i = 0; i <= subclasss.Count; i++)
    //        {
    //            subclassid = subclass.SubclassId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubclassId == subclassid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubclassId == subclassid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl24Subclasses.RemoveRange(subclasss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen classs löschen 
    //    var references = await Context.Tbl90References.Where(r => r.ClassId == classid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.ClassId == classid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subclassid;
    //}


    //public static async Task<int> RemoveTbl27Infraclasses(int subclassid)
    //{
    //    var infraclassid = 0;
    //    var infraclasss = await Context.Tbl27Infraclasses.Where(a => a.SubclassId == subclassid).ToListAsync();
    //    foreach (var infraclass in infraclasss)
    //    {
    //        for (var i = 0; i <= infraclasss.Count; i++)
    //        {
    //            infraclassid = infraclass.InfraclassId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfraclassId == infraclassid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfraclassId == infraclassid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl27Infraclasses.RemoveRange(infraclasss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen subclasss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SubclassId == subclassid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SubclassId == subclassid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return infraclassid;
    //}


    //public static async Task<int> RemoveTbl30Legios(int infraclassid)
    //{
    //    var legioid = 0;
    //    var legios = await Context.Tbl30Legios.Where(a => a.InfraclassId == infraclassid).ToListAsync();
    //    foreach (var legio in legios)
    //    {
    //        for (var i = 0; i <= legios.Count; i++)
    //        {
    //            legioid = legio.LegioId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.LegioId == legioid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.LegioId == legioid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl30Legios.RemoveRange(legios);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen infraclasss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.InfraclassId == infraclassid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.InfraclassId == infraclassid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return legioid;
    //}

    //public static async Task<int> RemoveTbl33Ordos(int legioid)
    //{
    //    var ordoid = 0;
    //    var ordos = await Context.Tbl33Ordos.Where(a => a.LegioId == legioid).ToListAsync();
    //    foreach (var ordo in ordos)
    //    {
    //        for (var i = 0; i <= ordos.Count; i++)
    //        {
    //            ordoid = ordo.OrdoId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.OrdoId == ordoid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.OrdoId == ordoid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl33Ordos.RemoveRange(ordos);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen legios löschen 
    //    var references = await Context.Tbl90References.Where(r => r.LegioId == legioid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.LegioId == legioid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return ordoid;
    //}

    //public static async Task RemoveOrdo(int ordoid)
    //{
    //    var ordosingle = Context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == ordoid);
    //    if (ordosingle == null)
    //    {
    //        return;
    //    }
    //    //delete  References  
    //    var references = await Context.Tbl90References.Where(r => r.OrdoId == ordoid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();
    //    //delete  comments
    //    var comments = await Context.Tbl93Comments.Where(c => c.OrdoId == ordoid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();
    //    //delete  Ordo  
    //    Context.Tbl33Ordos.Remove(ordosingle);
    //    await Context.SaveChangesAsync();
    //}

    //public static async Task<int> RemoveTbl36Subordos(int ordoid)
    //{
    //    var subordoid = 0;
    //    var subordos = await Context.Tbl36Subordos.Where(a => a.OrdoId == ordoid).ToListAsync();
    //    foreach (var subordo in subordos)
    //    {
    //        for (var i = 0; i <= subordos.Count; i++)
    //        {
    //            subordoid = subordo.SubordoId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubordoId == subordoid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubordoId == subordoid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl36Subordos.RemoveRange(subordos);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen ordos löschen 
    //    var references = await Context.Tbl90References.Where(r => r.OrdoId == ordoid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.OrdoId == ordoid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subordoid;
    //}
    //public static async Task RemoveSubordo(int subordoid)
    //{
    //    var subordosingle = Context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == subordoid);
    //    if (subordosingle == null)
    //    {
    //        return;
    //    }
    //    //delete  References  
    //    var references = await Context.Tbl90References.Where(r => r.SubordoId == subordoid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();
    //    //delete  comments
    //    var comments = await Context.Tbl93Comments.Where(c => c.SubordoId == subordoid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();
    //    //delete  Subordo  
    //    Context.Tbl36Subordos.Remove(subordosingle);
    //    await Context.SaveChangesAsync();
    //}

    //public static async Task<int> RemoveTbl39Infraordos(int subordoid)
    //{
    //    var infraordoid = 0;
    //    var infraordos = await Context.Tbl39Infraordos.Where(a => a.SubordoId == subordoid).ToListAsync();
    //    foreach (var infraordo in infraordos)
    //    {
    //        for (var i = 0; i <= infraordos.Count; i++)
    //        {
    //            infraordoid = infraordo.InfraordoId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfraordoId == infraordoid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfraordoId == infraordoid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl39Infraordos.RemoveRange(infraordos);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen subordos löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SubordoId == subordoid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SubordoId == subordoid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return infraordoid;
    //}

    //public static async Task<int> RemoveTbl42Superfamilies(int infraordoid)
    //{
    //    var superfamilyid = 0;
    //    var superfamilys = await Context.Tbl42Superfamilies.Where(a => a.InfraordoId == infraordoid).ToListAsync();
    //    foreach (var superfamily in superfamilys)
    //    {
    //        for (var i = 0; i <= superfamilys.Count; i++)
    //        {
    //            superfamilyid = superfamily.SuperfamilyId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl42Superfamilies.RemoveRange(superfamilys);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen infraordos löschen 
    //    var references = await Context.Tbl90References.Where(r => r.InfraordoId == infraordoid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.InfraordoId == infraordoid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return superfamilyid;
    //}

    //public static async Task<int> RemoveTbl45Families(int superfamilyid)
    //{
    //    var familyid = 0;
    //    var familys = await Context.Tbl45Families.Where(a => a.SuperfamilyId == superfamilyid).ToListAsync();
    //    foreach (var family in familys)
    //    {
    //        for (var i = 0; i <= familys.Count; i++)
    //        {
    //            familyid = family.FamilyId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.FamilyId == familyid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.FamilyId == familyid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl45Families.RemoveRange(familys);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen superfamilys löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SuperfamilyId == superfamilyid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SuperfamilyId == superfamilyid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return familyid;
    //}


    //public static async Task<int> RemoveTbl48Subfamilies(int familyid)
    //{
    //    var subfamilyid = 0;
    //    var subfamilys = await Context.Tbl48Subfamilies.Where(a => a.FamilyId == familyid).ToListAsync();
    //    foreach (var subfamily in subfamilys)
    //    {
    //        for (var i = 0; i <= subfamilys.Count; i++)
    //        {
    //            subfamilyid = subfamily.SubfamilyId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl48Subfamilies.RemoveRange(subfamilys);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen familys löschen 
    //    var references = await Context.Tbl90References.Where(r => r.FamilyId == familyid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.FamilyId == familyid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subfamilyid;
    //}

    //public static async Task<int> RemoveTbl51Infrafamilies(int subfamilyid)
    //{
    //    var infrafamilyid = 0;
    //    var infrafamilys = await Context.Tbl51Infrafamilies.Where(a => a.SubfamilyId == subfamilyid).ToListAsync();
    //    foreach (var infrafamily in infrafamilys)
    //    {
    //        for (var i = 0; i <= infrafamilys.Count; i++)
    //        {
    //            infrafamilyid = infrafamily.InfrafamilyId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl51Infrafamilies.RemoveRange(infrafamilys);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen subfamilys löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SubfamilyId == subfamilyid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SubfamilyId == subfamilyid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return infrafamilyid;
    //}


    //public static async Task<int> RemoveTbl54Supertribusses(int infrafamilyid)
    //{
    //    var supertribusid = 0;
    //    var supertribuss = await Context.Tbl54Supertribusses.Where(a => a.InfrafamilyId == infrafamilyid).ToListAsync();
    //    foreach (var supertribus in supertribuss)
    //    {
    //        for (var i = 0; i <= supertribuss.Count; i++)
    //        {
    //            supertribusid = supertribus.SupertribusId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SupertribusId == supertribusid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SupertribusId == supertribusid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl54Supertribusses.RemoveRange(supertribuss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen infrafamilys löschen 
    //    var references = await Context.Tbl90References.Where(r => r.InfrafamilyId == infrafamilyid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.InfrafamilyId == infrafamilyid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return supertribusid;
    //}


    //public static async Task<int> RemoveTbl57Tribusses(int supertribusid)
    //{
    //    var tribusid = 0;
    //    var tribuss = await Context.Tbl57Tribusses.Where(a => a.SupertribusId == supertribusid).ToListAsync();
    //    foreach (var tribus in tribuss)
    //    {
    //        for (var i = 0; i <= tribuss.Count; i++)
    //        {
    //            tribusid = tribus.TribusId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.TribusId == tribusid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.TribusId == tribusid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl57Tribusses.RemoveRange(tribuss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen supertribuss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SupertribusId == supertribusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SupertribusId == supertribusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return tribusid;
    //}


    //public static async Task<int> RemoveTbl60Subtribusses(int tribusid)
    //{
    //    var subtribusid = 0;
    //    var subtribuss = await Context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
    //    foreach (var subtribus in subtribuss)
    //    {
    //        for (var i = 0; i <= subtribuss.Count; i++)
    //        {
    //            subtribusid = subtribus.SubtribusId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.SubtribusId == subtribusid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.SubtribusId == subtribusid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl60Subtribusses.RemoveRange(subtribuss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen tribuss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.TribusId == tribusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.TribusId == tribusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return subtribusid;
    //}

    //public static async Task<int> RemoveTbl63Infratribusses(int subtribusid)
    //{
    //    var infratribusid = 0;
    //    var infratribuss = await Context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
    //    foreach (var infratribus in infratribuss)
    //    {
    //        for (var i = 0; i <= infratribuss.Count; i++)
    //        {
    //            infratribusid = infratribus.InfratribusId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.InfratribusId == infratribusid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.InfratribusId == infratribusid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl63Infratribusses.RemoveRange(infratribuss);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen subtribuss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.SubtribusId == subtribusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.SubtribusId == subtribusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return infratribusid;
    //}


    //public static async Task<int> RemoveTbl66Genusses(int infratribusid)
    //{
    //    var genusid = 0;
    //    var genusses = await Context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid).ToListAsync();
    //    foreach (var genus in genusses)
    //    {
    //        for (var i = 0; i <= genusses.Count; i++)
    //        {
    //            genusid = genus.GenusId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.GenusId == genusid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.GenusId == genusid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl66Genusses.RemoveRange(genusses);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen infratribuss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.InfratribusId == infratribusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.InfratribusId == infratribusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return genusid;
    //}


    //public static async Task<int> RemoveTbl69FiSpeciesses(int genusid)
    //{
    //    var fispeciesid = 0;
    //    var fispeciesses = await Context.Tbl69FiSpeciesses.Where(a => a.GenusId == genusid).ToListAsync();
    //    foreach (var fispecies in fispeciesses)
    //    {
    //        for (var i = 0; i <= fispeciesses.Count; i++)
    //        {
    //            fispeciesid = fispecies.FiSpeciesId;
    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl69FiSpeciesses.RemoveRange(fispeciesses);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen genus löschen 
    //    var references = await Context.Tbl90References.Where(r => r.GenusId == genusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.GenusId == genusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return fispeciesid;
    //}


    //public static async Task<int> RemoveTbl72PlSpeciesses(int genusid)
    //{
    //    var plspeciesid = 0;
    //    var plspeciess = await Context.Tbl72PlSpeciesses.Where(a => a.GenusId == genusid).ToListAsync();
    //    foreach (var plspecies in plspeciess)
    //    {
    //        for (var i = 0; i <= plspeciess.Count; i++)
    //        {
    //            plspeciesid = plspecies.PlSpeciesId;

    //            var singleref = Context.Tbl90References.SingleOrDefault(a => a.PlSpeciesId == plspeciesid);
    //            if (singleref != null)
    //            {
    //                Context.Tbl90References.Remove(singleref);
    //                await Context.SaveChangesAsync();
    //            }

    //            var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.PlSpeciesId == plspeciesid);
    //            if (singlecom != null)
    //            {
    //                Context.Tbl93Comments.Remove(singlecom);
    //                await Context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //    Context.Tbl72PlSpeciesses.RemoveRange(plspeciess);
    //    await Context.SaveChangesAsync();

    //    //References und Comments von allen genuss löschen 
    //    var references = await Context.Tbl90References.Where(r => r.GenusId == genusid).ToListAsync();
    //    Context.Tbl90References.RemoveRange(references);
    //    await Context.SaveChangesAsync();

    //    var comments = await Context.Tbl93Comments.Where(c => c.GenusId == genusid).ToListAsync();
    //    Context.Tbl93Comments.RemoveRange(comments);
    //    await Context.SaveChangesAsync();

    //    return plspeciesid;
    //}

    //public static async Task<int> RemoveTbl78Names(int plspeciesid)
    //{
    //    var nameid = 0;
    //    var names = await Context.Tbl78Names.Where(a => a.PlSpeciesId == plspeciesid).ToListAsync();
    //    //foreach (var name in names)
    //    //{
    //    //    for (var i = 0; i <= names.Count; i++)
    //    //    {
    //    //        nameid = name.NameId;
    //    //        var singleref = Context.Tbl90References.SingleOrDefault(a => a.NameId == nameid);
    //    //        if (singleref != null)
    //    //        {
    //    //            Context.Tbl90References.Remove(singleref);
    //    //            await Context.SaveChangesAsync();
    //    //        }

    //    //        var singlecom = Context.Tbl93Comments.SingleOrDefault(a => a.NameId == nameid);
    //    //        if (singlecom != null)
    //    //        {
    //    //            Context.Tbl93Comments.Remove(singlecom);
    //    //            await Context.SaveChangesAsync();
    //    //        }
    //    //    }
    //    //}
    //    Context.Tbl78Names.RemoveRange(names);
    //    await Context.SaveChangesAsync();

    //    ////References und Comments von allen plspeciess löschen 
    //    //var references = await Context.Tbl90References.Where(r => r.PlSpeciesId == plspeciesid).ToListAsync();
    //    //Context.Tbl90References.RemoveRange(references);
    //    //await Context.SaveChangesAsync();

    //    //var comments = await Context.Tbl93Comments.Where(c => c.PlSpeciesId == plspeciesid).ToListAsync();
    //    //Context.Tbl93Comments.RemoveRange(comments);
    //    //await Context.SaveChangesAsync();

    //    return nameid;
    //}


}
