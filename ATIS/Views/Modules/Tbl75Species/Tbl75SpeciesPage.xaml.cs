using System.ComponentModel;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl81Image;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    Tbl75SpeciesPage Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesPage : Page
{
    public Tbl75SpeciesViewModel ViewModel { get; }
    private readonly Tbl75SpeciesTabController _tabs;
    public Tbl75SpeciesPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl75SpeciesViewModel>();
        DataContext = ViewModel;
        _tabs = new Tbl75SpeciesTabController(this, ViewModel);

        Loaded += SpeciesPage_Loaded;
    }

    private Tbl75SpeciesViewModel Vm => (Tbl75SpeciesViewModel)DataContext;

    private async void SpeciesPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SpeciesPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);

        Vm.NameViewModel.PropertyChanged += NameViewModel_PropertyChanged;

        _tabs.UpdateNameTabs(Vm.NameViewModel.SelectedItem != null);
        _tabs.PrepareCurrentTabState();
        _tabs.UpdateTabButtonStates();
        _tabs.UpdateDetailContentVisibility();
    }

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ---------------------------------------------------------
    // TAB CLICK / STYLE
    // ---------------------------------------------------------
    private void DetailTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.Tag is string tagText &&
            int.TryParse(tagText, out int index))
        {
            Vm.SelectedDetailTabIndex = index;

            _tabs.HandleTabClick(sender);
        }
    }


    // ---------------------------------------------------------
    // TAB MATRIX 0..6
    // 0 EditSpecies
    // 1 CommentsSpecies
    // 2 ReferencesSpecies
    // 3 Name
    // 4 Image
    // 5 Synonym
    // 6 Geographic
    // ---------------------------------------------------------



    private void NameViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tbl78NameViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _tabs.UpdateNameTabs(Vm.NameViewModel.SelectedItem != null);
            });
        }
        if (e.PropertyName == nameof(Tbl81ImageViewModel.SelectedItem))
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _tabs.UpdateNameTabs(Vm.ImageViewModel.SelectedItem != null);
            });
        }

    }


    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (DataContext is Tbl75SpeciesViewModel vm)
        {
            vm.NameViewModel.PropertyChanged -= NameViewModel_PropertyChanged;
        }   
        base.OnNavigatedFrom(e);
    }
}
