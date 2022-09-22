using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.System;

internal class HotbarManager : IDisposable
{
    private readonly List<HotbarModule> hotbarList = new();
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
        ClearTransparency();
        hotbarList.Clear();
        LoadHotbars();

        if (Condition.IsBoundByDuty())
        {
            ApplyTransparency();
        }
        else
        {
            ClearTransparency();
        }
    }

    private IEnumerable<HotbarSetting> GetEnabledHotbars()
    {
        return Service.Configuration.HotbarSettings.Hotbars
            .Where(hotbar => hotbar.Enabled.Value);
    }

    private void ApplyTransparency()
    {
        foreach (var enabledHotbar in hotbarList)
        {
            enabledHotbar.SetActionTransparency(Settings.Transparency.Value);
        }
    }

    private void ClearTransparency()
    {
        foreach (var enabledHotbar in hotbarList)
        {
            enabledHotbar.ResetAllTransparency();
        }
    }

    private void LoadHotbars()
    {
        foreach (var hotbarSetting in GetEnabledHotbars())
        {
            PluginLog.Debug($"Loading Hotbar {hotbarSetting.AddonName}");
            hotbarList.Add(new HotbarModule(hotbarSetting.AddonName));
        }
    }
}