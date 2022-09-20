using KitchenSync.Interfaces;
using System.Collections.Generic;
using KitchenSync.UserInterface.Components;

namespace KitchenSync.Data;


public record HotbarSetting(string AddonName, Setting<bool> Enabled) : IInfoBoxListConfigurationRow
{
    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner.AddConfigCheckbox(AddonName, Enabled);
    }
}

public class HotbarSettings
{
    public Setting<float> Transparency = new(0.20f);

    public readonly Dictionary<string, HotbarSetting> Hotbars = new Dictionary<string, HotbarSetting>()
    {
        {"_ActionBar", new HotbarSetting("_ActionBar", new Setting<bool>(true))},
        {"_ActionBar01", new HotbarSetting("_ActionBar01", new Setting<bool>(true))},
        {"_ActionBar02", new HotbarSetting("_ActionBar02", new Setting<bool>(true))},
        {"_ActionBar03", new HotbarSetting("_ActionBar03", new Setting<bool>(false))},
        {"_ActionBar04", new HotbarSetting("_ActionBar04", new Setting<bool>(false))},
        {"_ActionBar05", new HotbarSetting("_ActionBar05", new Setting<bool>(false))},
        {"_ActionBar06", new HotbarSetting("_ActionBar06", new Setting<bool>(false))},
        {"_ActionBar07", new HotbarSetting("_ActionBar07", new Setting<bool>(false))},
        {"_ActionBar08", new HotbarSetting("_ActionBar08", new Setting<bool>(false))},
        {"_ActionBar09", new HotbarSetting("_ActionBar09", new Setting<bool>(false))},
    };
}