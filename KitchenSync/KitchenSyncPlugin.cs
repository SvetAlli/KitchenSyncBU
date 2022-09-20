using Dalamud.Plugin;
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

        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();

        Service.LogicModule = new KitchenSyncLogic();
    }

    public void Dispose()
    {
        Service.WindowManager.Dispose();
        Service.CommandSystem.Dispose();
        Service.LogicModule.Dispose();
    }
}