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
    }

    public void ApplyTransparency(float percentage)
    {
        if (ActionBar == null || HotbarModule == null) return;

        if (Name == HotbarName.CrossHotbar && (IsHoldControlRtlt() || IsHoldControlLtrt()))
        {
            // If our hotbar index is the left side of a crosshotbar, we start at 0
            // If out index is the right side of a crosshotbar, we start at 8
            var startIndex = GetCrossHotBarIndex() % 2 == 0 ? 0 : 8;

            foreach (var index in Enumerable.Range(startIndex, 8))
            {
                var hotbarSlot = GetHotBarSlot(index);

                // The ui slots that are replaced are the middle 8, starting at index 4 and ending at index 12
                var uiSlot = ActionBar->ActionBarSlots + (startIndex == 0 ? index + 4 : index - 4);

                if (hotbarSlot == null || uiSlot == null) continue;
                TryApplyTransparency(percentage, hotbarSlot, uiSlot);
            }
        }
        else
        {
            foreach (var index in Enumerable.Range(0, ActionBar->SlotCount))
            {
                var hotbarSlot = GetHotBarSlot(index);
                var uiSlot = ActionBar->ActionBarSlots + index;

                if (hotbarSlot == null || uiSlot == null) continue;
                TryApplyTransparency(percentage, hotbarSlot, uiSlot);
            }
        }
    }

    private void TryApplyTransparency(float percentage, HotBarSlot* hotbarSlot, ActionBarSlot* uiSlot)
    {
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

    private HotBarSlot* GetHotBarSlot(int index)
    {
        if (Name == HotbarName.DoubleCrossR)
        {
            return HotbarModule->Slot[index + 8];
        }

        return HotbarModule->Slot[index];
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
            HotbarName.CrossHotbar when IsHoldControlLtrt() => GetCrossHotBarIndex() / 2 + 10,
            HotbarName.CrossHotbar when IsHoldControlRtlt() => GetCrossHotBarIndex() / 2 + 10,
            HotbarName.CrossHotbar => ActionBar->RaptureHotbarId,
            HotbarName.DoubleCrossR => ((AddonActionDoubleCrossBase*)ActionBar)->BarTarget,
            HotbarName.DoubleCrossL => ((AddonActionDoubleCrossBase*)ActionBar)->BarTarget,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void ResetTransparency()
    {
        if (ActionBar == null || HotbarModule == null) return;

        foreach (var index in Enumerable.Range(0, ActionBar->SlotCount))
        {
            var uiSlot = ActionBar->ActionBarSlots + index;

            if (uiSlot == null) continue;
            ResetTransparencyToSlot(uiSlot);
        }
    }

    private void ApplyTransparencyToSlot(ActionBarSlot* uiSlot, float percentage)
    {
        uiSlot->Icon->AtkResNode.Color.A = (byte) (0xFF * percentage);
    }

    private void ResetTransparencyToSlot(ActionBarSlot* uiSlot)
    {
        uiSlot->Icon->AtkResNode.Color.A = 0xFF;
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

    private int GetCrossHotBarIndex()
    {
        var actionCrossBar = (AddonActionCross*) ActionBar;

        if (actionCrossBar->ExpandedHoldControlsLTRT != 0) return actionCrossBar->ExpandedHoldControlsLTRT - 1;
        if (actionCrossBar->ExpandedHoldControlsRTLT != 0) return actionCrossBar->ExpandedHoldControlsRTLT - 1;

        return actionCrossBar->ActionBarBase.RaptureHotbarId;
    }

    private bool IsHoldControlRtlt()
    {
        var actionCrossBar = (AddonActionCross*) ActionBar;

        return actionCrossBar->ExpandedHoldControlsRTLT != 0;
    }

    private bool IsHoldControlLtrt()
    {
        var actionCrossBar = (AddonActionCross*) ActionBar;

        return actionCrossBar->ExpandedHoldControlsLTRT != 0;
    }
}