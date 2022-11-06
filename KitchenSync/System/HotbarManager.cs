using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KitchenSync.Data;
using KitchenSync.Utilities;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace KitchenSync.System;

internal unsafe class HotbarManager : IDisposable
{
    private delegate void HotbarUpdateDelegate(AtkUnitBase* addon);

    private delegate byte CrossHotbarUpdateDelegate(AtkUnitBase* addon, IntPtr a2, byte a3);

    [Signature("E8 ?? ?? ?? ?? 80 BB ?? ?? ?? ?? ?? 74 4C 80 BB", DetourName = nameof(OnHotbarUpdate))]
    private readonly Hook<HotbarUpdateDelegate>? hotbarUpdateHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 57 48 83 EC 20 48 8B 42 20", DetourName = nameof(OnCrossUpdate))]
    private readonly Hook<CrossHotbarUpdateDelegate>? crossHotbarUpdateHook = null;

    private readonly List<Hotbar> hotbarList = new();
    private static HotbarSettings Settings => Service.Configuration.HotbarSettings;

    public HotbarManager()
    {
        LoadHotbars();

        SignatureHelper.Initialise(this);

        hotbarUpdateHook?.Enable();
        crossHotbarUpdateHook?.Enable();
    }

    public void Dispose()
    {
        foreach (var hotbar in hotbarList)
        {
            hotbar.ResetTransparency();
        }

        hotbarUpdateHook?.Dispose();
        crossHotbarUpdateHook?.Dispose();
    }

    private void OnHotbarUpdate(AtkUnitBase* addon)
    {
        hotbarUpdateHook!.Original(addon);

        try
        {
            UpdateHotbar(addon);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, $"Something went wrong updating hotbar {new IntPtr(addon):X8}");
        }
    }

    private byte OnCrossUpdate(AtkUnitBase* addon, IntPtr a2, byte a3)
    {
        var result = crossHotbarUpdateHook!.Original(addon, a2, a3);

        try
        {
            UpdateHotbar(addon);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, $"Something went wrong updating hotbar {new IntPtr(addon):X8}");
        }

        return result;
    }

    private void UpdateHotbar(AtkUnitBase* hotbarAddon)
    {
        var targetHotbar = hotbarList.FirstOrDefault(hotbar => hotbar.ActionBar == hotbarAddon);

        if (targetHotbar != null && Settings.Hotbars[targetHotbar.Name].Value)
        {
            if (Settings.DisableInSanctuaries.Value && GameMain.IsInSanctuary())
            {
                targetHotbar.ResetTransparency();
            }
            else
            {
                targetHotbar.ApplyTransparency(Settings.Transparency.Value);
            }
        }
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