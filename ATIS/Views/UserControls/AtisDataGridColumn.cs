
namespace ATIS.Views.UserControls;


public class AtisDataGridColumn
{
    public string Header { get; set; }
    public string LocalizationKey { get; set; }
    public GridLength Width { get; set; } = new GridLength(1, GridUnitType.Star);
    public Func<object, FrameworkElement> CellTemplate { get; set; }
}
