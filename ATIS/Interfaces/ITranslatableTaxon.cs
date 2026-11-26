
namespace ATIS.Interfaces;

public interface ITranslatableTaxon
{
    string? Info { get; }
    string? Memo { get; }
    string? TranslatedInfo { get; set; }
    string? TranslatedMemo { get; set; }
}
