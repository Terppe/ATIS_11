using System.ComponentModel;
using System.Diagnostics;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPage : Page
{
    public Tbl63InfratribusViewModel ViewModel { get; }

    public Tbl63InfratribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl63InfratribusViewModel>();
        DataContext = ViewModel;

        Loaded += InfratribusPage_Loaded;

        CommentsInfratribusPivot.DataContext = ViewModel.InfratribusCommentViewModel;

        Debug.WriteLine("ParentControl DC = " + this.DataContext);

    }
    private async void InfratribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InfratribusPage_Loaded;

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
    //    if (DetailPivot.SelectedItem == CommentsInfratribusPivot)
    //    {
    //        Tbl93CommentParentControl.DataContext = ViewModel.CommentViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == ReferencesInfratribusPivot) 
    //    {
    //        Tbl90ReferenceParentControl.DataContext = ViewModel.ReferenceViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == GenusInfratribusPivot)
    //    {
    //        Tbl66GenusParentControl.DataContext = ViewModel.GenusViewModel;
    //    }
    //}

    //private void DetailPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (DetailPivot.SelectedItem == CommentsInfratribusPivot)
    //    {
    //        Tbl93CommentParentControl.DataContext = ViewModel.CommentViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == ReferencesInfratribusPivot)
    //    {
    //        Tbl90ReferenceParentControl.DataContext = ViewModel.ReferenceViewModel;
    //    }

    //    if (DetailPivot.SelectedItem == GenusInfratribusPivot)
    //    {
    //        Tbl66GenusParentControl.DataContext = ViewModel.GenusViewModel;
    //    }
    //}
}
