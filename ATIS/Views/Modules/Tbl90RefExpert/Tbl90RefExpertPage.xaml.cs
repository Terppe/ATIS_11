using System.ComponentModel;


//    Tbl90RefExpertPage Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public sealed partial class Tbl90RefExpertPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl90RefExpertViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefExpertPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefExpertViewModel>();
        DataContext = ViewModel;

        Loaded += RefExpertPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void RefExpertPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RefExpertPage_Loaded;

        await ViewModel.LoadAsync();

        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
    }

    // ---------------------------------------------------------                   
    // QUERY-SUBMITTED
    // ---------------------------------------------------------                   

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }


}
