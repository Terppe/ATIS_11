using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl75Species;

public sealed class Tbl75SpeciesTabController
{
    public const int TabEdit = 0;
    public const int TabComments = 1;
    public const int TabReferences = 2;
    public const int TabName = 3;
    public const int TabImage = 4;
    public const int TabSynonym = 5;
    public const int TabGeographic = 6;

    private readonly Tbl75SpeciesPage _page;
    private readonly Tbl75SpeciesViewModel _vm;

    public Tbl75SpeciesTabController(Tbl75SpeciesPage page, Tbl75SpeciesViewModel vm)
    {
        _page = page;
        _vm = vm;
    }

    public void HandleTabClick(object sender)
    {
        if (sender is Button button &&
            button.Tag is string tagText &&
            int.TryParse(tagText, out int index))
        {
            _vm.SelectedDetailTabIndex = index;

            PrepareCurrentTabState();
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }

    public void PrepareCurrentTabState()
    {
        switch (_vm.SelectedDetailTabIndex)
        {
            case TabComments:
                _vm.SpeciesCommentViewModel.PrepareViewState();
                break;

            case TabReferences:
                _vm.SpeciesReferenceViewModel.PrepareViewState();
                break;

            case TabName:
                _vm.NameViewModel?.PrepareViewState();
                break;

            case TabImage:
                 _vm.ImageViewModel?.PrepareViewState();
                break;

            case TabSynonym:
                 _vm.SynonymViewModel?.PrepareViewState();
                break;

            case TabGeographic:
                 _vm.GeographicViewModel?.PrepareViewState();
                break;
        }
    }

    public void UpdateNameTabs(bool hasSelection)
    {
        _page.NameTab.Visibility = Visibility.Visible;
        _page.NameTab.IsEnabled = true;
        _page.ImageTab.Visibility = Visibility.Visible;
        _page.ImageTab.IsEnabled = true;
        _page.SynonymTab.Visibility = Visibility.Visible;
        _page.SynonymTab.IsEnabled = true;
        _page.GeographicTab.Visibility = Visibility.Visible;
        _page.GeographicTab.IsEnabled = true;

        //    _page.ImageTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        //_page.ImageTab.Visibility = Visibility.Visible;
        //_page.SynonymTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;
        //_page.GeographicTab.Visibility = hasSelection ? Visibility.Visible : Visibility.Collapsed;

        //_page.ImageTab.IsEnabled = hasSelection;
        //_page.SynonymTab.IsEnabled = hasSelection;
        //_page.GeographicTab.IsEnabled = hasSelection;

        //if (!hasSelection && _vm.SelectedDetailTabIndex >= TabImage)
        //    _vm.SelectedDetailTabIndex = TabName;

        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    public void UpdateTabButtonStates()
    {
        SetTabButtonStyle(_page.EditSpeciesTab, _page.EditSpeciesTabUnderline, _vm.SelectedDetailTabIndex == TabEdit);
        SetTabButtonStyle(_page.CommentsSpeciesTab, _page.CommentsSpeciesTabUnderline, _vm.SelectedDetailTabIndex == TabComments);
        SetTabButtonStyle(_page.ReferencesSpeciesTab, _page.ReferencesSpeciesTabUnderline, _vm.SelectedDetailTabIndex == TabReferences);
        SetTabButtonStyle(_page.NameTab, _page.NameTabUnderline, _vm.SelectedDetailTabIndex == TabName);
        SetTabButtonStyle(_page.ImageTab, _page.ImageTabUnderline, _vm.SelectedDetailTabIndex == TabImage);
        SetTabButtonStyle(_page.SynonymTab, _page.SynonymTabUnderline, _vm.SelectedDetailTabIndex == TabSynonym);
        SetTabButtonStyle(_page.GeographicTab, _page.GeographicTabUnderline, _vm.SelectedDetailTabIndex == TabGeographic);
    }

    public void UpdateDetailContentVisibility()
    {
        _page.EditSpeciesContent.Visibility = _vm.SelectedDetailTabIndex == TabEdit ? Visibility.Visible : Visibility.Collapsed;
        _page.CommentsSpeciesContent.Visibility = _vm.SelectedDetailTabIndex == TabComments ? Visibility.Visible : Visibility.Collapsed;
        _page.ReferencesSpeciesContent.Visibility = _vm.SelectedDetailTabIndex == TabReferences ? Visibility.Visible : Visibility.Collapsed;
        _page.NameContent.Visibility = _vm.SelectedDetailTabIndex == TabName ? Visibility.Visible : Visibility.Collapsed;

        // falls später aktiv:
        _page.ImageContent.Visibility = _vm.SelectedDetailTabIndex == TabImage ? Visibility.Visible : Visibility.Collapsed;
        _page.SynonymContent.Visibility = _vm.SelectedDetailTabIndex == TabSynonym ? Visibility.Visible : Visibility.Collapsed;
        _page.GeographicContent.Visibility = _vm.SelectedDetailTabIndex == TabGeographic ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }
}
