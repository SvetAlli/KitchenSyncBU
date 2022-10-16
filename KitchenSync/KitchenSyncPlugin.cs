using Dalamud.Plugin;
using KitchenSync.Data;
using KitchenSync.System;
using Lumina.Excel.GeneratedSheets;

namespace KitchenSync;

public sealed class KitchenSyncPlugin : IDalamudPlugin
{
    public string Name => "KitchenSync";

    public KitchenSyncPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(pluginInterface);

        Service.ActionCache = new LuminaCache<Action>();
        Service.TerritoryCache = new LuminaCache<TerritoryType>();
        Service.IconManager = new IconManager();

        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();
        Service.HotbarManager = new HotbarManager();
    }

    public void Dispose()
    {
        Service.ActionCache.Dispose();
        Service.TerritoryCache.Dispose();
        Service.IconManager.Dispose();

        Service.WindowManager.Dispose();
        Service.CommandSystem.Dispose();
        Service.HotbarManager.Dispose();
    }
}