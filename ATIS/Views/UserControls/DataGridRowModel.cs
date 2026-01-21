namespace ATIS.Views.UserControls;

public class DataGridRowModel
{
    public object Data { get; set; }
    public IList<AtisDataGridColumn> Columns { get; set; }
}
