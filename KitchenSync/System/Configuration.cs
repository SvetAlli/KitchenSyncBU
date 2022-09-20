using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace KitchenSync.System;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;




    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;
    public void Initialize(DalamudPluginInterface @interface) => pluginInterface = @interface;
    public void Save() => pluginInterface!.SavePluginConfig(this);
}