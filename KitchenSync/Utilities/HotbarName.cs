using System;

namespace KitchenSync.Utilities;

public enum HotbarName
{
    Hotbar1, 
    Hotbar2, 
    Hotbar3,
    Hotbar4, 
    Hotbar5, 
    Hotbar6,
    Hotbar7, 
    Hotbar8, 
    Hotbar9, 
    Hotbar10,
    CrossHotbar,
    DoubleCrossR,
    DoubleCrossL,
}

internal static class HotbarNameExtensions
{
    public static string GetAddonName(this HotbarName name)
    {
        return name switch
        {
            HotbarName.Hotbar1 => "_ActionBar",
            HotbarName.Hotbar2 => "_ActionBar01",
            HotbarName.Hotbar3 => "_ActionBar02",
            HotbarName.Hotbar4 => "_ActionBar03",
            HotbarName.Hotbar5 => "_ActionBar04",
            HotbarName.Hotbar6 => "_ActionBar05",
            HotbarName.Hotbar7 => "_ActionBar06",
            HotbarName.Hotbar8 => "_ActionBar07",
            HotbarName.Hotbar9 => "_ActionBar08",
            HotbarName.Hotbar10 => "_ActionBar09",
            HotbarName.CrossHotbar => "_ActionCross",
            HotbarName.DoubleCrossR => "_ActionDoubleCrossR",
            HotbarName.DoubleCrossL => "_ActionDoubleCrossL",
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        };
    }
}
