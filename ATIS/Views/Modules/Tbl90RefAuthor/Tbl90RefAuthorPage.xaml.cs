using System.ComponentModel;


//    Tbl90RefAuthorPage Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public sealed partial class Tbl90RefAuthorPage : Page
{
    private object? _commentsHeaderText;
    private object? _referencesHeaderText;
    public Tbl90RefAuthorViewModel ViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefAuthorPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefAuthorViewModel>();
        DataContext = ViewModel;

        Loaded += RefAuthorPage_Loaded;
    }

    // ---------------------------------------------------------
    // PAGE LOADED
    // ---------------------------------------------------------

    private async void RefAuthorPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= RefAuthorPage_Loaded;

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

    // ------------------------------------------------------------------------------------------------------------------------
    // COMMENTS + REFERENCES PIVOTS VON Species VERWALTEN
    // ------------------------------------------------------------------------------------------------------------------------





}
