using System;
using System.Collections.Generic;
using System.Text;
using ATIS.AtisDesignsystem.Enums;

namespace ATIS.AtisDesignsystem.Services;

public static class AtisIconService
{
    public static IconElement GetIcon(AtisIconKind kind)
    {
        return kind switch
        {
            AtisIconKind.Home => new SymbolIcon(Symbol.Home),

            AtisIconKind.Database => new BitmapIcon
            {
                UriSource = new Uri("ms-appx:///Assets/Icons/database.png"),
                ShowAsMonochrome = false
            },

            AtisIconKind.Folder => new SymbolIcon(Symbol.Folder),

            AtisIconKind.Species => new BitmapIcon
            {
                UriSource = new Uri("ms-appx:///Assets/Icons/species.png"),
                ShowAsMonochrome = false
            },

            AtisIconKind.Genus => new BitmapIcon
            {
                UriSource = new Uri("ms-appx:///Assets/Icons/genus.png"),
                ShowAsMonochrome = false
            },

            AtisIconKind.Settings => new SymbolIcon(Symbol.Setting),

            _ => new SymbolIcon(Symbol.Help)
        };
    }
}
