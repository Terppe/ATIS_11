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

        if (content is Tbl36SubordoDetailPage subordoPage && subordoPage.ViewModel != null)
        {
            await subordoPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl39InfraordoDetailPage infraordoPage && infraordoPage.ViewModel != null)
        {
            await infraordoPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl42SuperfamilyDetailPage superfamilyPage && superfamilyPage.ViewModel != null)
        {
            await superfamilyPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl45FamilyDetailPage familyPage && familyPage.ViewModel != null)
        {
            await familyPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl48SubfamilyDetailPage subfamilyPage && subfamilyPage.ViewModel != null)
        {
            await subfamilyPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl51InfrafamilyDetailPage infrafamilyPage && infrafamilyPage.ViewModel != null)
        {
            await infrafamilyPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl54SupertribusDetailPage supertribusPage && supertribusPage.ViewModel != null)
        {
            await supertribusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl57TribusDetailPage tribusPage && tribusPage.ViewModel != null)
        {
            await tribusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl60SubtribusDetailPage subtribusPage && subtribusPage.ViewModel != null)
        {
            await subtribusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl63InfratribusDetailPage infratribusPage && infratribusPage.ViewModel != null)
        {
            await infratribusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl66GenusDetailPage genusPage && genusPage.ViewModel != null)
        {
            await genusPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        if (content is Tbl75SpeciesDetailPage speciesPage && speciesPage.ViewModel != null)
        {
            await speciesPage.ViewModel.NavigateBreadcrumbAsync(crumb);
            return;
        }

        // ...weitere Seiten hier zentral ergänzen
    }
}
