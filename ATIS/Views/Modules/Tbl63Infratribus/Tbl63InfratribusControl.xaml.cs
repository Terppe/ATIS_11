using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusControl : UserControl
{
    public Tbl63InfratribusViewModel ViewModel => DataContext as Tbl63InfratribusViewModel;

    private readonly List<Tbl63InfratribusRowControl> _rows = new();
    private Models.Tbl63Infratribus? _lastSelectedFromVM;

    private double _lastVerticalOffset;
    private double _lastHorizontalOffset;


    public Tbl63InfratribusControl()
    {
        InitializeComponent();
        DataContextChanged += Control_DataContextChanged;

    }

    private void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (args.NewValue is Tbl63InfratribusViewModel vm)
        {
            vm.PropertyChanged += ViewModel_PropertyChanged;

         ////   vm.ReloadCompleted = async () =>
         //   {
         //       await Task.Yield();
         //       FocusManager.TryFocusAsync(FocusSink, FocusState.Programmatic);
         //       BodyScroll.ChangeView(_lastHorizontalOffset, _lastVerticalOffset, null, true);
         //   };

            UpdateRowSelection();
        }
        //if (args.NewValue is Tbl63InfratribusViewModel vm)
        //{
        //    vm.PropertyChanged += ViewModel_PropertyChanged;
        //    UpdateRowSelection();
        //}
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedItem))
            UpdateRowSelection();
    }
    private void RestoreRowFocusToSelected()
    {
        var selected = ViewModel.SelectedItem;
        if (selected is null)
            return;

        var index = ViewModel.Items.IndexOf(selected);
        if (index < 0)
            return;

        var element = InfratribusRepeater.TryGetElement(index);
        if (element is Control c)
            c.Focus(FocusState.Programmatic);
    }


    //private void Row_Tapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (sender is Tbl63InfratribusRowControl row &&
    //        row.DataContext is Models.Tbl63Infratribus item)
    //    {
    //        //ViewModel.SelectedInfratribus = item;
    //        //UpdateRowSelection();

    //        // WICHTIG: NICHT SelectedInfratribus setzen!
    //        ViewModel.SelectItem(item);

    //        // RowControl-Selection aktualisieren
    //        UpdateRowSelection();
    //    }
    //}

    //private void UpdateRowSelection()
    //{
    //    foreach (var row in _rows)
    //        row.IsSelected = ReferenceEquals(row.DataContext, ViewModel.SelectedItem);
    //}

    //private void Row_Loaded(object sender, RoutedEventArgs e)
    //{
    //    if (sender is Tbl63InfratribusRowControl row)
    //        _rows.Add(row);
    //}

    //private void Row_Unloaded(object sender, RoutedEventArgs e)
    //{
    //    if (sender is Tbl63InfratribusRowControl row)
    //        _rows.Remove(row);
    //}


    private bool _syncingHeader;

    private void BodyScroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        _lastVerticalOffset = BodyScroll.VerticalOffset;
        _lastHorizontalOffset = BodyScroll.HorizontalOffset;

        if (_syncingHeader)
            return;

        _syncingHeader = true;
        HeaderScroll.ChangeView(BodyScroll.HorizontalOffset, null, null, disableAnimation: true);
        _syncingHeader = false;
    }

    //private void Row_RowTapped(object sender, Models.Tbl63Infratribus item)
    //{
    //    ViewModel.SelectItem(item);

    //    //  ViewModel.SelectedItem = item;

    //    Debug.WriteLine($"Tapped item={item.InfratribusId}, SelectedItem={ViewModel.SelectedItem?.InfratribusId}");

    //    (ViewModel.SaveCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    //    (ViewModel.DeleteCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();

    //  //  UpdateRowSelection();
    //}

    private void Row_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not Tbl63InfratribusRowControl row)
            return;

        var item = row.Infratribus;

        if (ViewModel.SelectedItem == item)
            return;

        ViewModel.SelectedItem = item;
    }

    //private void Row_Tapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (e.OriginalSource is FrameworkElement fe &&
    //        fe.DataContext is Models.Tbl63Infratribus domain)
    //    {
    //        ViewModel.SelectedItem = domain;
    //    }
    //}

    private void Row_RowTapped(object? sender, Models.Tbl63Infratribus tbl63Infratribus)
    {
        if (sender is not Tbl63InfratribusRowControl row)
            return;

        var item = row.Infratribus;

        // WICHTIG: NICHT erneut setzen, wenn schon selektiert
        if (ViewModel.SelectedItem == item)
            return;

        ViewModel.SelectedItem = item;

    }

    private void UpdateRowSelection()
    {
        System.Diagnostics.Debug.WriteLine(
            $"[UpdateRowSelection] SelectedId={(ViewModel.SelectedItem == null ? "null" : ViewModel.SelectedItem.InfratribusId.ToString())} rows={_rows.Count}");
        Tbl63InfratribusRowControl? selectedRow = null;

        foreach (var row in _rows)
        {
            row.IsSelected = ReferenceEquals(row.Infratribus, ViewModel.SelectedItem);
            if (row.IsSelected)
                selectedRow = row;
        }

        // ItemsRepeater + Clear/Add -> Scrollposition springt oft.
        // Daher nach Selection (auch nach Reload) zum selektierten Datensatz scrollen.
        if (selectedRow != null)
        {
            // sofort versuchen
            selectedRow.StartBringIntoView(new BringIntoViewOptions
            {
                AnimationDesired = false,
                VerticalAlignmentRatio = 0.5
            });

            // und einmal "später" (falls Virtualisierung/Measure noch läuft)
            DispatcherQueue.TryEnqueue(() =>
            {
                var row2 = _rows.FirstOrDefault(r => ReferenceEquals(r.Infratribus, ViewModel.SelectedItem));
                row2?.StartBringIntoView(new BringIntoViewOptions
                {
                    AnimationDesired = false,
                    VerticalAlignmentRatio = 0.5
                });
            });
        }
    }

    //private void UpdateRowSelection()
    //{
    //    foreach (var row in _rows)
    //        row.IsSelected = ReferenceEquals(row.Infratribus, ViewModel.SelectedItem);
    //}
    private void Row_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Tbl63InfratribusRowControl row)
            _rows.Add(row);
    }

    private void Row_Unloaded(object sender, RoutedEventArgs e)
    {
        if (sender is Tbl63InfratribusRowControl row)
            _rows.Remove(row);
    }

}
