using QuestPDF.Infrastructure;

namespace ATIS.Interfaces;

public interface IPdfExportService
{
    Task ExportAsync(Func<Task<IDocument?>> createDocumentAsync, string suggestedFileName);
}
