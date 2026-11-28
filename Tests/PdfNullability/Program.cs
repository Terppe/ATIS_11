using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report;
using ATIS.Views.Search.Pdf.Tbl63Infratribusses;

string? missing = null;
var regnum = new PdfModels.Regnum { RegnumName = missing, Author = missing, ValidYear = missing };
var miscellaneous = new PdfModels.Miscellaneous { MiscellaneousName = missing, Info = missing };
if (regnum.RegnumName != null || regnum.Author != null || regnum.ValidYear != null ||
    miscellaneous.MiscellaneousName != null || miscellaneous.Info != null)
    throw new Exception("Missing PDF fields were not preserved.");

var model = Tbl63InfratribussesListDocumentDataSource.GetInfratribusListDetails(
    new ObservableCollection<Tbl63Infratribus>());
if (model.Valid != null)
    throw new Exception("Missing validity must remain null.");

Console.WriteLine("PDF nullability checks passed.");
