
namespace ATIS.Views.Search;

public class SearchTemplateSelector : DataTemplateSelector
{
    public DataTemplate RegnumTemplate { get; set; }
    public DataTemplate PhylumTemplate { get; set; }
    public DataTemplate SubphylumTemplate { get; set; }
    public DataTemplate SuperclassTemplate { get; set; }
    public DataTemplate ClassTemplate { get; set; }

    public DataTemplate SpeciesTemplate { get; set; }

    public string SelectedTable { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        return SelectedTable switch
        {
            "Regnum" => RegnumTemplate,
            "Phylum" => PhylumTemplate,
            "Subphylum" => SubphylumTemplate,
            "Superclass" => SuperclassTemplate,
            "Class" => ClassTemplate,
            "Species" => SpeciesTemplate,
            _ => SpeciesTemplate
        };
    }
}
