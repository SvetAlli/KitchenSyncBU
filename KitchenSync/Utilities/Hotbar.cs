using System;
using System.Linq;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KitchenSync.Data;
using Action = Lumina.Excel.GeneratedSheets.Action;
using HotbarPointer = FFXIVClientStructs.FFXIV.Client.UI.Misc.HotBar;

namespace KitchenSync.Utilities;

internal unsafe class Hotbar
{
    private HotbarSettings Settings => Service.Configuration.HotbarSettings;

    public HotbarName Name { get; }

    private AddonActionBarBase* ActionBar => (AddonActionBarBase*) Service.GameGui.GetAddonByName(Name.GetAddonName(), 1);
    private HotbarPointer* HotbarModule => Framework.Instance()->UIModule->GetRaptureHotbarModule()->HotBar[GetHotbarIndex()];

    public Hotbar(HotbarName name)
    {
        Name = name;

        PluginLog.Debug($"{new IntPtr(Framework.Instance()->UIModule->GetRaptureHotbarModule()->HotBar[0]):X8}");
    }

    public void ApplyTransparency(float percentage)
    {
        if (ActionBar == null || HotbarModule == null) return;

        foreach (var index in Enumerable.Range(0, ActionBar->SlotCount))
        {
            var hotbarSlot = HotbarModule->Slot[Name == HotbarName.DoubleCrossR ? index + 8 : index];
            var uiSlot = ActionBar->ActionBarSlots + index;

            switch (hotbarSlot->CommandType)
            {
                case HotbarSlotType.Action when !IsRoleAction(hotbarSlot) && IsSyncAction(hotbarSlot):
                case HotbarSlotType.Macro when Settings.IncludeMacros.Value && IsSyncMacroAction(hotbarSlot):
                    ApplyTransparencyToSlot(uiSlot, percentage);
                    break;

                default:
                    ResetTransparencyToSlot(uiSlot);
                    break;
            }
        }
    }

    private int GetHotbarIndex()
    {
        return Name switch
        {
            HotbarName.Hotbar1 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar2 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar3 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar4 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar5 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar6 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar7 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar8 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar9 => ActionBar->RaptureHotbarId,
            HotbarName.Hotbar10 => ActionBar->RaptureHotbarId,
            HotbarName.CrossHotbar => ActionBar->RaptureHotbarId,
            HotbarName.DoubleCrossR => 17,
            HotbarName.DoubleCrossL => 17,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void ResetTransparency()
    {
        if (ActionBar == null || HotbarModule == null) return;

        var hotbarSize = ActionBar->SlotCount;

        foreach (var index in Enumerable.Range(0, hotbarSize))
        {
            var uiSlot = ActionBar->ActionBarSlots + index;

            ResetTransparencyToSlot(uiSlot);
        }
    }

    private void ApplyTransparencyToSlot(ActionBarSlot* uiSlot, float percentage)
    {
        uiSlot->Icon->AtkResNode.Color.A = (byte) (0xFF * percentage);
    }

    private void ResetTransparencyToSlot(ActionBarSlot* uiSlot)
    {
        if (uiSlot == null) return;
        
        var icon = uiSlot->Icon;
        if (icon == null) return;
        
        icon->AtkResNode.Color.A = 0xFF;
    }

    private bool IsRoleAction(HotBarSlot* dataSlot)
    {
        var action = GetAdjustedAction(dataSlot->CommandId);

        return action is {IsRoleAction: true};
    }

    private bool IsSyncMacroAction(HotBarSlot* dataSlot)
    {
        var action = GetAdjustedAction(dataSlot->IconA);
        var level = Service.ClientState.LocalPlayer?.Level ?? 0;

        return action?.ClassJobLevel > level;
    }

    private bool IsSyncAction(HotBarSlot* dataSlot)
    {
        var action = GetAdjustedAction(dataSlot->CommandId);
        var level = Service.ClientState.LocalPlayer?.Level ?? 0;

        return action?.ClassJobLevel > level;
    }

    private Action? GetAdjustedAction(uint actionID)
    {
        var adjustedActionID = ActionManager.Instance()->GetAdjustedActionId(actionID);

        return Service.DataManager.GetExcelSheet<Action>()!.GetRow(adjustedActionID);
    }
}