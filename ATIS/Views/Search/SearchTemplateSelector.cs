using ATIS.Models;

namespace ATIS.Views.Search;

public class SearchTemplateSelector : DataTemplateSelector
{
    public DataTemplate RegnumTemplate { get; set; }
    public DataTemplate PhylumTemplate { get; set; }
    public DataTemplate SubphylumTemplate { get; set; }
    public DataTemplate SuperclassTemplate { get; set; }
    public DataTemplate ClassTemplate { get; set; }
    public DataTemplate SubclassTemplate { get; set; }
    public DataTemplate InfraclassTemplate { get; set; }
    public DataTemplate LegioTemplate { get; set; }
    public DataTemplate OrdoTemplate { get; set; }
    public DataTemplate SubordoTemplate { get; set; }
    public DataTemplate InfraordoTemplate { get; set; }
    public DataTemplate SuperfamilyTemplate { get; set; }
    public DataTemplate FamilyTemplate { get; set; }
    public DataTemplate SubfamilyTemplate { get; set; }
    public DataTemplate InfrafamilyTemplate { get; set; }
    public DataTemplate SupertribusTemplate { get; set; }
    public DataTemplate TribusTemplate { get; set; }
    public DataTemplate SubtribusTemplate { get; set; }
    public DataTemplate InfratribusTemplate { get; set; }
    public DataTemplate GenusTemplate { get; set; }
    public DataTemplate SpeciesTemplate { get; set; }


    protected override DataTemplate SelectTemplateCore(object item)
    {
        return item switch
        {
            Tbl03Regnum => RegnumTemplate,
            Tbl06Phylum => PhylumTemplate,
            Tbl12Subphylum => SubphylumTemplate,
            Tbl18Superclass => SuperclassTemplate,
            Tbl21Class => ClassTemplate,
            Tbl24Subclass => SubclassTemplate,
            Tbl27Infraclass => InfraclassTemplate,
            Tbl30Legio => LegioTemplate,
            Tbl33Ordo => OrdoTemplate,
            Tbl36Subordo => SubordoTemplate,
            Tbl39Infraordo => SubordoTemplate,
            Tbl42Superfamily => SubordoTemplate,
            Tbl45Family => SubordoTemplate,
            Tbl48Subfamily => SubordoTemplate,
            Tbl51Infrafamily => SubordoTemplate,
            Tbl54Supertribus => SubordoTemplate,
            Tbl57Tribus => SubordoTemplate,
            Tbl60Subtribus => SubordoTemplate,
            Tbl63Infratribus => SubordoTemplate,
            Tbl66Genus => SubordoTemplate,
            Tbl75Species => SpeciesTemplate,

            _ => base.SelectTemplateCore(item)
        };
    }
}
