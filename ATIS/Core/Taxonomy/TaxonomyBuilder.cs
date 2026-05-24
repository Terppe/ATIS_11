using ATIS.Core.Taxonomy;
using ATIS.Views.Modules.Tbl03Regnum;
using ATIS.Views.Modules.Tbl06Phylum;
using ATIS.Views.Modules.Tbl12Subphylum;
using ATIS.Views.Modules.Tbl18Superclass;
using ATIS.Views.Modules.Tbl21Class;
using ATIS.Views.Modules.Tbl24Subclass;
using ATIS.Views.Modules.Tbl27Infraclass;
using ATIS.Views.Modules.Tbl30Legio;
using ATIS.Views.Modules.Tbl33Ordo;
using ATIS.Views.Modules.Tbl36Subordo;
using ATIS.Views.Modules.Tbl39Infraordo;
using ATIS.Views.Modules.Tbl42Superfamily;
using ATIS.Views.Modules.Tbl45Family;
using ATIS.Views.Modules.Tbl48Subfamily;
using ATIS.Views.Modules.Tbl51Infrafamily;
using ATIS.Views.Modules.Tbl54Supertribus;
using ATIS.Views.Modules.Tbl57Tribus;
using ATIS.Views.Modules.Tbl60Subtribus;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl75Species;

namespace ATIS.Core.Taxonomy;

public static class TaxonomyBuilder
{
    public static void Build(
        //Tbl03RegnumViewModel regnumVM,
        //Tbl06PhylumViewModel phylumVM,
        //Tbl12SubphylumViewModel subphylumVM,
        //Tbl18SuperclassViewModel superclassVM,
        //Tbl21ClassViewModel classVM,
        //Tbl24SubclassViewModel subclassVM,
        //Tbl27InfraclassViewModel infraclassVM,
        //Tbl30LegioViewModel legioVM,
        //Tbl33OrdoViewModel ordoVM,
        //Tbl36SubordoViewModel subordoVM,
        //Tbl39InfraordoViewModel infraordoVM,
        //Tbl42SuperfamilyViewModel superfamilyVM,
        //Tbl45FamilyViewModel familyVM,
        //Tbl48SubfamilyViewModel subfamilyVM,
        //Tbl51InfrafamilyViewModel infrafamilyVM,
        Tbl54SupertribusViewModel supertribusVM,
        Tbl57TribusViewModel tribusVM,
        Tbl60SubtribusViewModel subtribusVM,
        Tbl63InfratribusViewModel infratribusVM,
        Tbl66GenusViewModel genusVM
        //Tbl75SpeciesViewModel speciesVM
    )
    {
        TaxonomyDefinition.Levels.Clear();

        TaxonomyDefinition.Levels.AddRange(new[]
        {
            //new TaxonLevel("Regnum",
            //    (Func<int, Task>)(id => regnumVM.LoadForRegnumAsync(id)),
            //    (Action)regnumVM.ResetBelow),

            //new TaxonLevel("Phylum",
            //    (Func<int, Task>)(id => phylumVM.LoadForRegnumAsync(id)),
            //    (Action)phylumVM.ResetBelow),

            //new TaxonLevel("Subphylum",
            //    (Func<int, Task>)(id => subphylumVM.LoadForPhylumAsync(id)),
            //    (Action)subphylumVM.ResetBelow),

            //new TaxonLevel("Superclass",
            //    (Func<int, Task>)(id => superclassVM.LoadForSubphylumAsync(id)),
            //    (Action)superclassVM.ResetBelow),

            //new TaxonLevel("Class",
            //    (Func<int, Task>)(id => classVM.LoadForSuperclassAsync(id)),
            //    (Action)classVM.ResetBelow),

            //new TaxonLevel("Subclass",
            //    (Func<int, Task>)(id => subclassVM.LoadForClassAsync(id)),
            //    (Action)subclassVM.ResetBelow),

            //new TaxonLevel("Infraclass",
            //    (Func<int, Task>)(id => infraclassVM.LoadForSubclassAsync(id)),
            //    (Action)infraclassVM.ResetBelow),

            //new TaxonLevel("Legio",
            //    (Func<int, Task>)(id => legioVM.LoadForInfraclassAsync(id)),
            //    (Action)legioVM.ResetBelow),

            //new TaxonLevel("Ordo",
            //    (Func<int, Task>)(id => ordoVM.LoadForLegioAsync(id)),
            //    (Action)ordoVM.ResetBelow),

            //new TaxonLevel("Subordo",
            //    (Func<int, Task>)(id => subordoVM.LoadForOrdoAsync(id)),
            //    (Action)subordoVM.ResetBelow),

            //new TaxonLevel("Infraordo",
            //    (Func<int, Task>)(id => infraordoVM.LoadForSubordoAsync(id)),
            //    (Action)infraordoVM.ResetBelow),

            //new TaxonLevel("Superfamily",
            //    (Func<int, Task>)(id => superfamilyVM.LoadForInfraordoAsync(id)),
            //    (Action)superfamilyVM.ResetBelow),

            //new TaxonLevel("Family",
            //    (Func<int, Task>)(id => familyVM.LoadForSuperfamilyAsync(id)),
            //    (Action)familyVM.ResetBelow),

            //new TaxonLevel("Subfamily",
            //    (Func<int, Task>)(id => subfamilyVM.LoadForFamilyAsync(id)),
            //    (Action)subfamilyVM.ResetBelow),

            //new TaxonLevel("Infrafamily",
            //    (Func<int, Task>)(id => infrafamilyVM.LoadForSubfamilyAsync(id)),
            //    (Action)infrafamilyVM.ResetBelow),

            //new TaxonLevel("Supertribus",
            //    (Func<int, Task>)(id => supertribusVM.LoadForInfrafamilyAsync(id)),
            //    (Action)supertribusVM.ResetBelow),

            new TaxonLevel("Tribus",
                (Func<int, Task>)(id => tribusVM.LoadForSupertribusAsync(id)),
                (Action)tribusVM.ResetBelow),

            new TaxonLevel("Subtribus",
                (Func<int, Task>)(id => subtribusVM.LoadForTribusAsync(id)),
                (Action)subtribusVM.ResetBelow),

            new TaxonLevel("Infratribus",
                (Func<int, Task>)(id => infratribusVM.LoadForSubtribusAsync(id)),
                (Action)infratribusVM.ResetBelow),

            new TaxonLevel("Genus",
                (Func<int, Task>)(id => genusVM.LoadForInfratribusAsync(id)),
                (Action)genusVM.ResetBelow),

            //new TaxonLevel("Species",
            //    (Func<int, Task>)(id => speciesVM.LoadForGenusAsync(id)),
            //    (Action)speciesVM.ResetBelow)
        });
    }
}
