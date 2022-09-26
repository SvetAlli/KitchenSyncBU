using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace KitchenSync.Data;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public HotbarSettings HotbarSettings = new();
    
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;
    public void Initialize(DalamudPluginInterface @interface) => pluginInterface = @interface;
    public void Save()
    {
        Service.HotbarManager.Refresh();
        pluginInterface!.SavePluginConfig(this);
    }
}