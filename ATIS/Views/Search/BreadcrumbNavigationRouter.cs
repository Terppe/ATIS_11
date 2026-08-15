using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Views.Search.DetailPages;

namespace ATIS.Views.Search;

public static class BreadcrumbNavigationRouter
{
    public static async Task NavigateAsync(BreadcrumbItem crumb)
    {
        var content = App.MainPageInstance?.NavFrame?.Content;

        if (content is Tbl03RegnumDetailPage regnumPage && regnumPage.ViewModel != null)
        {
            await regnumPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl06PhylumDetailPage phylumPage && phylumPage.ViewModel != null)
        {
            await phylumPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl12SubphylumDetailPage subphylumPage && subphylumPage.ViewModel != null)
        {
            await subphylumPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl18SuperclassDetailPage superclassPage && superclassPage.ViewModel != null)
        {
            await superclassPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl21ClassDetailPage classPage && classPage.ViewModel != null)
        {
            await classPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl24SubclassDetailPage subclassPage && subclassPage.ViewModel != null)
        {
            await subclassPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl27InfraclassDetailPage infraclassPage && infraclassPage.ViewModel != null)
        {
            await infraclassPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl30LegioDetailPage legioPage && legioPage.ViewModel != null)
        {
            await legioPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl33OrdoDetailPage ordoPage && ordoPage.ViewModel != null)
        {
            await ordoPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        //if (content is Tbl66GenusDetailPage genusPage && genusPage.ViewModel != null)
        //{
        //    await genusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
        //    return;
        //}

        // ...weitere Seiten hier zentral ergänzen
    }
}
