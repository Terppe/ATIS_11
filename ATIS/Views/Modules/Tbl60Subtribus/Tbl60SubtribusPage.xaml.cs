using System.ComponentModel;
using System.Diagnostics;
using ATIS.Services;
using log4net;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusPage : Page
{
    public Tbl60SubtribusViewModel ViewModel { get; }

    public Tbl60SubtribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl60SubtribusViewModel>();
        DataContext = ViewModel;

        Loaded += SubtribusPage_Loaded;
    }

    private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SubtribusPage_Loaded;

        // Falls Lookup: Tribus benötigt wird
        await ViewModel.LoadTribusLookupAsync();

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
    }
    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }
    private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            await ViewModel.ApplyFilterAsync(sender.Text);
    }
    //private void DetailPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (DetailPivot.SelectedItem == CommentsSubtribusPivot)
    //    {
    //        CommentParentControl.DataContext = ViewModel.CommentViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == ReferencesSubtribusPivot)
    //    {
    //        ReferenceParentControl.DataContext = ViewModel.ReferenceViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == InfratribusSubtribusPivot)
    //    {
    //        InfratribusParentControl.DataContext = ViewModel.InfratribusViewModel;
    //    }
    //}

    //private async void DetailPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (DetailPivot.SelectedItem is PivotItem pivotItem)
    //    {
    //        if (pivotItem.Header is string header && header == "Comments")
    //        {
    //            if (DataContext is Tbl60SubtribusViewModel vm &&
    //                vm.SelectedItem != null)
    //            {
    //                await vm.CommentViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //            }
    //        }
    //        // später: References, Infratribus genauso
    //    }
    //}

    private bool _commentsLoaded;
    private bool _referencesLoaded;
    private bool _infratribussesLoaded;

    //private async void DetailPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (DataContext is not Tbl60SubtribusViewModel vm)
    //        return;

    //    if (vm.SelectedItem == null)
    //        return;

    //    // COMMENTS (Subtribus + Infratribus)
    //    if (DetailPivot.SelectedItem == CommentsSubtribusPivot ||
    //        DetailPivot.SelectedItem == CommentsInfratribusPivot)
    //    {
    //        if (!_commentsLoaded)
    //        {
    //            _commentsLoaded = true;
    //            await vm.CommentViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }

    //    // REFERENCES (Subtribus + Infratribus)
    //    else if (DetailPivot.SelectedItem == ReferencesSubtribusPivot ||
    //             DetailPivot.SelectedItem == ReferencesInfratribusPivot)
    //    {
    //        if (!_referencesLoaded)
    //        {
    //            _referencesLoaded = true;
    //            await vm.ReferenceViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }

    //    // INFRATRIBUS
    //    else if (DetailPivot.SelectedItem == InfratribusSubtribusPivot)
    //    {
    //        if (!_infratribussesLoaded)
    //        {
    //            _infratribussesLoaded = true;
    //            await vm.InfratribusViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //}


    //private async void DetailPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (DetailPivot.SelectedItem == CommentsSubtribusPivot)
    //    {
    //        if (DataContext is Tbl60SubtribusViewModel vm &&
    //            vm.SelectedItem != null)
    //        {
    //            await vm.CommentViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //    else if (DetailPivot.SelectedItem == ReferencesSubtribusPivot)
    //    {
    //        if (DataContext is Tbl60SubtribusViewModel vm &&
    //            vm.SelectedItem != null)
    //        {
    //            await vm.ReferenceViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //    //weiteres Pivot-Item hinzufügen, z.B. Infratribus
    //    else if (DetailPivot.SelectedItem == InfratribusSubtribusPivot)
    //    {
    //        if (DataContext is Tbl60SubtribusViewModel vm &&
    //            vm.SelectedItem != null)
    //        {
    //            await vm.InfratribusViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //    else if (DetailPivot.SelectedItem == CommentsInfratribusPivot)
    //    {
    //        if (DataContext is Tbl60SubtribusViewModel vm &&
    //            vm.SelectedItem != null)
    //        {
    //            await vm.CommentViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //    else if (DetailPivot.SelectedItem == ReferencesInfratribusPivot)
    //    {
    //        if (DataContext is Tbl60SubtribusViewModel vm &&
    //            vm.SelectedItem != null)
    //        {
    //            await vm.ReferenceViewModel.LoadParentAsync(vm.SelectedItem.SubtribusId);
    //        }
    //    }
    //}

}
