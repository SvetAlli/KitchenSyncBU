using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using KitchenSync.Data;
using Lumina.Excel.GeneratedSheets;
using CommandManager = Dalamud.Game.Command.CommandManager;
using Condition = Dalamud.Game.ClientState.Conditions.Condition;

namespace KitchenSync;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static CommandManager Commands { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;
    [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static DataManager DataManager { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;

    public static Configuration Configuration = null!;
    public static System.CommandManager CommandSystem = null!;
    public static System.WindowManager WindowManager = null!;
    public static System.HotbarManager HotbarManager = null!;
    public static System.IconManager IconManager = null!;
    public static System.LuminaCache<Action> ActionCache = null!;
    public static System.LuminaCache<TerritoryType> TerritoryCache = null!;
}