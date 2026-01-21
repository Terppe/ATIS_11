using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Views.UserControls;

public sealed partial class DataGridRowControl : UserControl
{
    public DataGridRowControl()
    {
        InitializeComponent();
        Loaded += OnLoaded; // IMMER registrieren!
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not DataGridRowModel model)
            return;

        if (model.Columns == null || model.Columns.Count == 0)
            return;

        if (model.Data == null)
            return;

        RowGrid.ColumnDefinitions.Clear();
        RowGrid.Children.Clear();

        for (int i = 0; i < model.Columns.Count; i++)
        {
            RowGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = model.Columns[i].Width
            });

            var cell = model.Columns[i].CellTemplate(model.Data);
            Grid.SetColumn(cell, i);
            RowGrid.Children.Add(cell);
        }
    }
}
