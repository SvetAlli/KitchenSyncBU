using Dalamud.Plugin;
using KitchenSync.Data;
using KitchenSync.System;

namespace KitchenSync;

public sealed class KitchenSyncPlugin : IDalamudPlugin
{
    public string Name => "KitchenSync";

    public KitchenSyncPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(pluginInterface);

        Service.IconManager = new IconManager();
        Service.PlayerEventManager = new PlayerEventManager();

        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();
        Service.HotbarManager = new HotbarManager();
    }

    public void Dispose()
    {
        Service.IconManager.Dispose();
        Service.PlayerEventManager.Dispose();

        Service.WindowManager.Dispose();
        Service.CommandSystem.Dispose();
        Service.HotbarManager.Dispose();
    }
}