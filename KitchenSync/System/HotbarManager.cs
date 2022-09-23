﻿using System;
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

        Service.DutyEventManager.DutyStarted += OnDutyStart;
        Service.DutyEventManager.DutyCompleted += OnDutyEnd;
    }

    public void Dispose()
    {
        Service.DutyEventManager.DutyStarted -= OnDutyStart;
        Service.DutyEventManager.DutyCompleted -= OnDutyEnd;
    }

    private void OnDutyStart(object? sender, uint e) => ApplyTransparency();

    private void OnDutyEnd(object? sender, uint e) => ClearTransparency();

    public void Refresh()
    {
        if (Condition.IsBoundByDuty())
        {
            ApplyTransparency();
        }
        else
        {
            ClearTransparency();
        }
    }

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

    private void ClearTransparency()
    {
        foreach (var hotbar in hotbarList)
        {
            hotbar.ResetTransparency();
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