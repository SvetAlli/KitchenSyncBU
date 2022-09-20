using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using CommandManager = Dalamud.Game.Command.CommandManager;

namespace KitchenSync;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static CommandManager Commands { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;
    [PluginService] public static DataManager DataManager { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static AetheryteList AetheryteList { get; private set;} = null!;
    [PluginService] public static ToastGui Toast { get; private set;} = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;
    [PluginService] public static TargetManager TargetManager { get; private set; } = null!;

    public static System.CommandManager CommandSystem = null!;
    public static System.WindowManager WindowManager = null!;
    public static System.Configuration Configuration = null!;
    public static System.KitchenSyncLogic LogicModule = null!;
}