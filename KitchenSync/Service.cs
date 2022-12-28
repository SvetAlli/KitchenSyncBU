using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using KitchenSync.Data;
using KitchenSync.System;

namespace KitchenSync;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;

    public static Configuration Configuration = null!;
    public static HotbarManager HotbarManager = null!;
}