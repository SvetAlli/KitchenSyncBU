using System.Collections.Generic;
using KamiLib.Configuration;
using KitchenSync.Utilities;

namespace KitchenSync.Data;

public class HotbarSettings
{
    public Setting<float> Transparency = new(0.20f);
    public Setting<bool> IncludeMacros = new(true);
    public Setting<bool> IncludeNotUnlocked = new(true);
    public Setting<bool> DisableInSanctuaries = new(true);
    
    public Dictionary<HotbarName, Setting<bool>> Hotbars = new()
    {
        {HotbarName.Hotbar1, new Setting<bool>(true)},
        {HotbarName.Hotbar2, new Setting<bool>(true)},
        {HotbarName.Hotbar3, new Setting<bool>(true)},
        {HotbarName.Hotbar4, new Setting<bool>(false)},
        {HotbarName.Hotbar5, new Setting<bool>(false)},
        {HotbarName.Hotbar6, new Setting<bool>(false)},
        {HotbarName.Hotbar7, new Setting<bool>(false)},
        {HotbarName.Hotbar8, new Setting<bool>(false)},
        {HotbarName.Hotbar9, new Setting<bool>(false)},
        {HotbarName.Hotbar10, new Setting<bool>(false)},
        {HotbarName.CrossHotbar, new Setting<bool>(false)},
        {HotbarName.DoubleCrossR, new Setting<bool>(false)},
        {HotbarName.DoubleCrossL, new Setting<bool>(false)},
    };
}
