using Dalamud.Plugin;
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
        
        KamiLib.KamiLib.Initialize(pluginInterface, Name, () => Service.Configuration.Save());
        
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(pluginInterface);

        KamiLib.KamiLib.WindowManager.AddWindow(new ConfigurationWindow());
        KamiLib.KamiLib.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        KamiLib.KamiLib.CommandManager.AddCommand(new MainHotbarCommands());
        KamiLib.KamiLib.CommandManager.AddCommand(new CrossHotbarCommands());

        Service.HotbarManager = new HotbarManager();
    }

    public void Dispose()
    {
        KamiLib.KamiLib.Dispose();
        
        Service.HotbarManager.Dispose();
    }
}