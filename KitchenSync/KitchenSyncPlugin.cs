using Dalamud.Plugin;
using KamiLib;
using KitchenSync.Commands;
using KitchenSync.Data;
using KitchenSync.System;
using KitchenSync.Windows;

namespace KitchenSync;

public sealed class KitchenSyncPlugin : IDalamudPlugin
{
    public string Name => "KitchenSync";
    private const string ShorthandCommand = "/ksync";

    public KitchenSyncPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        KamiCommon.Initialize(pluginInterface, Name, () => Service.Configuration.Save());
        
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(pluginInterface);

        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        KamiCommon.CommandManager.AddCommand(new MainHotbarCommands());
        KamiCommon.CommandManager.AddCommand(new CrossHotbarCommands());

        Service.HotbarManager = new HotbarManager();
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        Service.HotbarManager.Dispose();
    }
}