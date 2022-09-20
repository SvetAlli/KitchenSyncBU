using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Utilities;
using Dalamud.Interface.Windowing;
using KitchenSync.UserInterface.Windows;

namespace KitchenSync.System;

internal class WindowManager : IDisposable
{
    private readonly WindowSystem windowSystem = new("KitchenSync");

    private readonly List<Window> windows = new()
    {
        new ConfigurationWindow(),
    };

    public WindowManager()
    {
        foreach (var window in windows)
        {
            windowSystem.AddWindow(window);
        }

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void DrawUI() => windowSystem.Draw();

    private void DrawConfigUI()
    {
        if(Service.ClientState.IsPvP)
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");

        windows[0].IsOpen = true;
    }

    public T? GetWindowOfType<T>()
    {
        return windows.OfType<T>().FirstOrDefault();
    }

    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        foreach (var window in windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        windowSystem.RemoveAllWindows();
    }
}