using System;
using System.Collections.Generic;
using Dalamud.Game;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.System;

internal class HotbarManager : IDisposable
{
    private readonly List<Hotbar> hotbarList = new();
    private static HotbarSettings Settings => Service.Configuration.HotbarSettings;

    private readonly Queue<Hotbar> updateQueue;

    public HotbarManager()
    {
        LoadHotbars();

        updateQueue = new Queue<Hotbar>(hotbarList);

        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (!Service.ClientState.IsLoggedIn) return;

        if (Settings.PotatoMode.Value)
        {
            var currentHotbar = updateQueue.Dequeue();

            if (Settings.Hotbars[currentHotbar.Name].Value)
            {
                currentHotbar.ApplyTransparency(Settings.Transparency.Value);
            }
            else
            {
                currentHotbar.ResetTransparency();
            }

            updateQueue.Enqueue(currentHotbar);
        }
        else
        {
            foreach (var hotbar in hotbarList)
            {
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
    }

    public void Dispose()
    {
        foreach (var hotbar in hotbarList)
        {
            hotbar.ResetTransparency();
        }

        Service.Framework.Update -= OnFrameworkUpdate;
    }

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
        // If the config doesn't have any hotbars
        if (Settings.Hotbars.Count == 0) Service.Configuration = new Configuration();

        // Load all hotbars
        foreach (var hotbar in Settings.Hotbars)
        {
            hotbarList.Add(new Hotbar(hotbar.Key));
        }
    }
}