using ATIS.Models;

namespace ATIS.Views.Report;

public abstract class GenrateModels
{
    public abstract class GenRegnums
    {
        public static PdfModels.Regnum GenerateRegnums(Tbl03Regnum single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Regnum
            {
                RegnumName = single.RegnumName,
                Subregnum = single.Subregnum,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public abstract class GenPhylums
    {
        public static PdfModels.Phylum GeneratePhylums(Tbl06Phylum single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Phylum
            {
                PhylumName = single.PhylumName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public abstract class GenSubphylums
    {
        public static PdfModels.Subphylum GenerateSubphylums(Tbl12Subphylum single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Subphylum
            {
                SubphylumName = single.SubphylumName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    //public abstract class GenDivisions
    //{
    //    public static PdfModels.Division GenerateDivisions(Tbl09Division single)
    //    {
    //        if (single == null)
    //        {
    //            return null!;
    //        }

    //        return new PdfModels.Division
    //        {
    //            DivisionName = single.DivisionName,
    //            Author = single.Author,
    //            AuthorYear = single.AuthorYear,
    //            GerName = single.GerName,
    //            EngName = single.EngName,
    //            FraName = single.FraName,
    //            PorName = single.PorName,
    //            Valid = single.Valid,
    //            ValidYear = single.ValidYear,
    //            Info = single.Info,
    //            Memo = single.Memo
    //        };
    //    }
    //}
    //public abstract class GenSubdivisions
    //{
    //    public static PdfModels.Subdivision GenerateSubdivisions(Tbl15Subdivision single)
    //    {
    //        if (single == null)
    //        {
    //            return null!;
    //        }

    //        return new PdfModels.Subdivision
    //        {
    //            SubdivisionName = single.SubdivisionName,
    //            Author = single.Author,
    //            AuthorYear = single.AuthorYear,
    //            GerName = single.GerName,
    //            EngName = single.EngName,
    //            FraName = single.FraName,
    //            PorName = single.PorName,
    //            Valid = single.Valid,
    //            ValidYear = single.ValidYear,
    //            Info = single.Info,
    //            Memo = single.Memo
    //        };
    //    }
    //}
    public abstract class GenSuperclasses
    {
        public static PdfModels.Superclass GenerateSuperclasses(Tbl18Superclass single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Superclass
            {
                SuperclassName = single.SuperclassName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public abstract class GenClasses
    {
        public static PdfModels.Class GenerateClasses(Tbl21Class single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Class
            {
                ClassName = single.ClassName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public abstract class GenSubclasses
    {
        public static PdfModels.Subclass GenerateSubclasses(Tbl24Subclass single)
        {
            if (single == null)
            {
                return null!;
            }
            return new PdfModels.Subclass
            {
                SubclassName = single.SubclassName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public abstract class GenInfraclasses
    {
        public static PdfModels.Infraclass GenerateInfraclasses(Tbl27Infraclass single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Infraclass
            {
                InfraclassName = single.InfraclassName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenLegios
    {
        public static PdfModels.Legio GenerateLegios(Tbl30Legio single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Legio
            {
                LegioName = single.LegioName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenOrdos
    {
        public static PdfModels.Ordo GenerateOrdos(Tbl33Ordo single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Ordo
            {
                OrdoName = single.OrdoName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSubordos
    {
        public static PdfModels.Subordo GenerateSubordos(Tbl36Subordo single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Subordo
            {
                SubordoName = single.SubordoName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenInfraordos
    {
        public static PdfModels.Infraordo GenerateInfraordos(Tbl39Infraordo single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Infraordo
            {
                InfraordoName = single.InfraordoName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSuperfamilies
    {
        public static PdfModels.Superfamily GenerateSuperfamilies(Tbl42Superfamily single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Superfamily
            {
                SuperfamilyName = single.SuperfamilyName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenFamilies
    {
        public static PdfModels.Family GenerateFamilies(Tbl45Family single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Family
            {
                FamilyName = single.FamilyName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSubfamilies
    {
        public static PdfModels.Subfamily GenerateSubfamilies(Tbl48Subfamily single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Subfamily
            {
                SubfamilyName = single.SubfamilyName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenInfrafamilies
    {
        public static PdfModels.Infrafamily GenerateInfrafamilies(Tbl51Infrafamily single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Infrafamily
            {
                InfrafamilyName = single.InfrafamilyName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSupertribusses
    {
        public static PdfModels.Supertribus GenerateSupertribusses(Tbl54Supertribus single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Supertribus
            {
                SupertribusName = single.SupertribusName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenTribusses
    {
        public static PdfModels.Tribus GenerateTribusses(Tbl57Tribus single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Tribus
            {
                TribusName = single.TribusName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSubtribusses
    {
        public static PdfModels.Subtribus GenerateSubtribusses(Tbl60Subtribus single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Subtribus
            {
                SubtribusName = single.SubtribusName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenInfratribusses
    {
        public static PdfModels.Infratribus GenerateInfratribusses(Tbl63Infratribus single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Infratribus
            {
                InfratribusName = single.InfratribusName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = (bool)single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenGenusses
    {
        public static PdfModels.Genus GenerateGenusses(Tbl66Genus single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Genus
            {
                GenusName = single.GenusName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                GerName = single.GerName,
                EngName = single.EngName,
                FraName = single.FraName,
                PorName = single.PorName,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenSpeciesses
    {
        public static PdfModels.Species GenerateSpeciesses(Tbl75Species single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Species
            {
                Tbl66Genusses = single.Tbl66Genusses,
                SpeciesName = single.SpeciesName,
                Subspecies = single.Subspecies,
                Divers = single.Divers,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                MemoSpecies = single.MemoSpecies,
                TradeName = single.TradeName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                Importer = single.Importer,
                ImportingYear = single.ImportingYear,
                TypeSpecies = single.TypeSpecies,
                LNumber = single.LNumber,
                LOrigin = single.LOrigin,
                LdaOrigin = single.LdaOrigin,
                LdaNumber = single.LdaNumber,
                BasinLength = single.BasinLength,
                FishLength = single.FishLength,
                Karnivore = single.Karnivore,
                Herbivore = single.Herbivore,
                Limnivore = single.Limnivore,
                Omnivore = single.Omnivore,
                MemoFoods = single.MemoFoods,
                Difficult1 = single.Difficult1,
                Difficult2 = single.Difficult2,
                Difficult3 = single.Difficult3,
                Difficult4 = single.Difficult4,
                RegionTop = single.RegionTop,
                RegionMiddle = single.RegionMiddle,
                RegionBottom = single.RegionBottom,
                MemoRegion = single.MemoRegion,
                MemoTech = single.MemoTech,
                Ph1 = single.Ph1,
                Ph2 = single.Ph2,
                Temp1 = single.Temp1,
                Temp2 = single.Temp2,
                Hardness1 = single.Hardness1,
                Hardness2 = single.Hardness2,
                CarboHardness1 = single.CarboHardness1,
                CarboHardness2 = single.CarboHardness2,
                MemoHusbandry = single.MemoHusbandry,
                MemoBuilt = single.MemoBuilt,
                MemoColor = single.MemoColor,
                MemoSozial = single.MemoSozial,
                MemoDomorphism = single.MemoDomorphism,
                MemoSpecial = single.MemoSpecial,
                MemoBreeding = single.MemoBreeding,
                //Plant
                BasinHeight = single.BasinHeight,
                PlantLength = single.PlantLength,
                MemoReproduction = single.MemoReproduction,
                MemoCulture = single.MemoCulture,
                MemoGlobal = single.MemoGlobal,
                Creature = single.Creature
            };
        }
    }
    //public class GenPlSpeciesses
    //{
    //    public static PdfModels.PlSpecies GeneratePlSpeciesses(Tbl75Species single)
    //    {
    //        if (single == null)
    //        {
    //            return null!;
    //        }

    //        return new PdfModels.PlSpecies
    //        {
    //            Tbl66Genusses = single.Tbl66Genusses,
    //            PlSpeciesName = single.SpeciesName,
    //            Subspecies = single.Subspecies,
    //            Divers = single.Divers,
    //            GenusId = single.GenusId,
    //            SpeciesgroupId = single.SpeciesgroupId,
    //            Valid = single.Valid,
    //            ValidYear = single.ValidYear,
    //            MemoSpecies = single.MemoSpecies,
    //            TradeName = single.TradeName,
    //            Author = single.Author,
    //            AuthorYear = single.AuthorYear,
    //            Importer = single.Importer,
    //            ImportingYear = single.ImportingYear,
    //            BasinHeight = single.BasinHeight,
    //            PlantLength = single.PlantLength,
    //            Difficult1 = single.Difficult1,
    //            Difficult2 = single.Difficult2,
    //            Difficult3 = single.Difficult3,
    //            Difficult4 = single.Difficult4,
    //            MemoTech = single.MemoTech,
    //            Ph1 = single.Ph1,
    //            Ph2 = single.Ph2,
    //            Temp1 = single.Temp1,
    //            Temp2 = single.Temp2,
    //            Hardness1 = single.Hardness1,
    //            Hardness2 = single.Hardness2,
    //            CarboHardness1 = single.CarboHardness1,
    //            CarboHardness2 = single.CarboHardness2,
    //            MemoBuilt = single.MemoBuilt,
    //            MemoColor = single.MemoColor,
    //            MemoReproduction = single.MemoReproduction,
    //            MemoCulture = single.MemoCulture,
    //            MemoGlobal = single.MemoGlobal
    //        };
    //    }
    //}

    public class GenSynonyms
    {
        public static PdfModels.Synonym GenerateSynonyms(Tbl84Synonym single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Synonym
            {
                SynonymName = single.SynonymName,
                Author = single.Author,
                AuthorYear = single.AuthorYear,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }
    public class GenNames
    {
        public static PdfModels.Name GenerateNames(Tbl78Name single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Name
            {
                NameName = single.NameName,
                Language = single.Language,
                Valid = single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }

    public class GenDiseases
    {
        public static PdfModels.Disease GenerateDiseases(TblDisease single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Disease
            {
                DiseaseName = single.DiseaseName,
                ShortInfo = single.ShortInfo,
                Symptom = single.Symptom,
                Reason = single.Reason,
                Therapie = single.Therapie,
                Valid = (bool)single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo
            };
        }
    }

    public class GenFoods
    {
        public static PdfModels.Food GenerateFoods(TblFood single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Food
            {
                FoodName = single.FoodName,
                FoodVolume = single.FoodVolume,
                ShortInfo = single.ShortInfo,
                Valid = (bool)single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo,
                Symptom = single.Symptom
            };
        }
    }
    public class GenMiscellaneousses
    {
        public static PdfModels.Miscellaneous GenerateMiscellaneoussess(TblMiscellaneous single)
        {
            if (single == null)
            {
                return null!;
            }

            return new PdfModels.Miscellaneous
            {
                MiscellaneousName = single.MiscellaneousName,
                ShortInfo = single.ShortInfo,
                Thema1 = single.Thema1,
                Thema2 = single.Thema2,
                Valid = (bool)single.Valid,
                ValidYear = single.ValidYear,
                Info = single.Info,
                Memo = single.Memo,
            };
        }
    }

}
