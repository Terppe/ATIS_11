

using System.Collections;
using ATIS.Services;

namespace ATIS.Views.UserControls;

public sealed partial class AtisDataGrid : UserControl
{
    public AtisDataGrid()
    {
        InitializeComponent();

        // Wichtig: erst NACH InitializeComponent
        Loaded += (_, __) =>
        {
            BuildHeader();
            BuildRows();
        };
    }

    // -----------------------------
    // Dependency Properties
    // -----------------------------

    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(
            nameof(Columns),
            typeof(IList<AtisDataGridColumn>),
            typeof(AtisDataGrid),
            new PropertyMetadata(new List<AtisDataGridColumn>(), OnColumnsChanged));

    public IList<AtisDataGridColumn> Columns
    {
        get => (IList<AtisDataGridColumn>)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (AtisDataGrid)d;
        grid.BuildHeader();
        grid.BuildRows();
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable),
            typeof(AtisDataGrid),
            new PropertyMetadata(null, OnItemsChanged));

    public IEnumerable Items
    {
        get => (IEnumerable)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (AtisDataGrid)d;
        grid.BuildRows();
    }

    // -----------------------------
    // Header Builder
    // -----------------------------

    private void BuildHeader()
    {
        if (Columns == null || Columns.Count == 0)
            return;

        HeaderGrid.ColumnDefinitions.Clear();
        HeaderGrid.Children.Clear();

        for (int i = 0; i < Columns.Count; i++)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = Columns[i].Width
            });

            var header = new TextBlock
            {
                Text = Columns[i].Header,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };

            Grid.SetColumn(header, i);
            HeaderGrid.Children.Add(header);
        }
    }

    // -----------------------------
    // Rows Builder
    // -----------------------------

    private void BuildRows()
    {
        if (Items == null || Columns == null || Columns.Count == 0)
            return;

        var rows = Items
            .Cast<object>()
            .Select(item => new DataGridRowModel
            {
                Data = item,
                Columns = Columns.ToList() // Kopie!
            })
            .ToList(); // Materialisieren!

        RowsRepeater.ItemsSource = rows;
    }

    public void RefreshLocalization()
    {
        if (Columns == null || Columns.Count == 0)
            return;

        foreach (var col in Columns)
        {
            if (!string.IsNullOrEmpty(col.LocalizationKey))
            {
                // Hier deinen Localizer verwenden – Beispiel:
                col.Header = LocalizationManager.Get(col.LocalizationKey);
            }
        }

        // Header neu aufbauen, damit die neuen Texte sichtbar werden
        BuildHeader();
    }

}
