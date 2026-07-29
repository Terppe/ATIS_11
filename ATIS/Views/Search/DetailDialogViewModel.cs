
using System.Collections.ObjectModel;
using System.Reflection;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Search;

public sealed class DetailDialogViewModel : ObservableObject
{
    public string Title { get; }
    public ObservableCollection<DetailField> Fields { get; } = new();

    public IRelayCommand ExportPdfCommand { get; }

    public DetailDialogViewModel(object entity)
    {
        Title = entity.GetType().Name;

        // ⭐ Felder extrahieren
        foreach (var prop in entity.GetType().GetProperties())
        {
            // Navigation Properties überspringen
            if (!IsPrimitive(prop.PropertyType))
                continue;

            var value = prop.GetValue(entity)?.ToString() ?? "";
            Fields.Add(new DetailField(prop.Name, value));
        }

        ExportPdfCommand = new RelayCommand(ExportPdf);
    }

    private bool IsPrimitive(Type t)
    {
        return t.IsPrimitive ||
               t == typeof(string) ||
               t == typeof(DateTime) ||
               t == typeof(decimal) ||
               t == typeof(int) ||
               t == typeof(double);
    }

    private void ExportPdf()
    {
        // ⭐ Hier kannst du später PDF‑Erzeugung einbauen
        // z.B. Syncfusion, QuestPDF, iText, etc.
    }
}

public sealed class DetailField
{
    public string Label { get; }
    public string Value { get; }

    public DetailField(string label, string value)
    {
        Label = label;
        Value = value;
    }
}

public sealed class FieldEntry
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public FieldKind Kind { get; set; } = FieldKind.Text;
}

public enum FieldKind
{
    Text,
    DateTime,
    Boolean,
    Enum,
    List,
    Object
}

