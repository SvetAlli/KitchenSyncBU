using KitchenSync.Interfaces;
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

    public readonly HotbarSetting[] Hotbars =
    {
        new("_ActionBar", new Setting<bool>(true)),
        new("_ActionBar01", new Setting<bool>(true)),
        new("_ActionBar02", new Setting<bool>(true)),
        new("_ActionBar03", new Setting<bool>(false)),
        new("_ActionBar04", new Setting<bool>(false)),
        new("_ActionBar05", new Setting<bool>(false)),
        new("_ActionBar06", new Setting<bool>(false)),
        new("_ActionBar07", new Setting<bool>(false)),
        new("_ActionBar08", new Setting<bool>(false)),
        new("_ActionBar09", new Setting<bool>(false))
    };
}