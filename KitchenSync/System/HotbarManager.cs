using System;
using System.Collections.Generic;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.System;

internal class HotbarManager : IDisposable
{
    private readonly List<Hotbar> hotbarList = new();
    private static HotbarSettings Settings => Service.Configuration.HotbarSettings;

    public HotbarManager()
    {
        LoadHotbars();

        Service.PlayerEventManager.PlayerLevelChanged += OnLevelChange;
        Service.ControllerEventManager.ControllerHotbarUpdate += OnControllerHotbarChange;
    }

    public void Dispose()
    {
        Service.PlayerEventManager.PlayerLevelChanged -= OnLevelChange;
        Service.ControllerEventManager.ControllerHotbarUpdate -= OnControllerHotbarChange;
    }

    private void OnControllerHotbarChange(object? sender, EventArgs e) => ApplyTransparency();

    private void OnLevelChange(object? sender, EventArgs e) => ApplyTransparency();

    public void Refresh() => ApplyTransparency();

    private void ApplyTransparency()
    {
        foreach (var hotbar in hotbarList)
        {
            // If this hotbar is enabled
            if (Settings.Hotbars[hotbar.Name].Value)
            {
                hotbar.ApplyTransparency(Settings.Transparency.Value);
            }
            else
            {
                hotbar.ResetTransparency();
            }
        }
    }

    private void LoadHotbars()
    {
        foreach (var hotbar in Settings.Hotbars)
        {
            hotbarList.Add(new Hotbar(hotbar.Key));
        }
    }
}