using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KitchenSync.Data;
using Lumina.Excel.GeneratedSheets;
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

        var startingIndex = GetStartingIndex();
        var hotbarSize = GetSize();

        foreach (var index in Enumerable.Range(startingIndex, hotbarSize))
        {
            var dataSlot = GetHotBarSlot(index);
            var uiSlot = GetUISlot(index);

            if(dataSlot == null || uiSlot == null) continue;

            TryApplyTransparency(percentage, dataSlot, uiSlot);
        }
    }

    private void TryApplyTransparency(float percentage, HotBarSlot* hotbarSlot, ActionBarSlot* uiSlot)
    {
        switch (hotbarSlot->CommandType)
        {
            case HotbarSlotType.Action when ShouldApplyTransparency(hotbarSlot):
            case HotbarSlotType.Macro when Settings.IncludeMacros.Value && IsSyncMacroAction(hotbarSlot):
                ApplyTransparencyToSlot(uiSlot, percentage);
                break;

            default:
                ResetTransparencyToSlot(uiSlot);
                break;
        }
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

    private bool ShouldApplyTransparency(HotBarSlot* slot) => (!IsRoleAction(slot) && IsSyncAction(slot)) || (Settings.IncludeNotUnlocked.Value && !IsActionUnlocked(slot));

    private void ApplyTransparencyToSlot(ActionBarSlot* uiSlot, float percentage) => uiSlot->Icon->AtkResNode.Color.A = (byte)(0xFF * percentage);

    private void ResetTransparencyToSlot(ActionBarSlot* uiSlot) => uiSlot->Icon->AtkResNode.Color.A = 0xFF;
    
    private bool IsHoldControlRtlt() => ((AddonActionCross*) ActionBar)->ExpandedHoldControlsRTLT != 0;

    private bool IsHoldControlLtrt() => ((AddonActionCross*) ActionBar)->ExpandedHoldControlsLTRT != 0;
    
    private bool IsRoleAction(HotBarSlot* dataSlot) => GetAdjustedAction(dataSlot->CommandId) is {IsRoleAction: true};

    private Action? GetAdjustedAction(uint actionID) => Service.DataManager.GetExcelSheet<Action>()!.GetRow(ActionManager.Instance()->GetAdjustedActionId(actionID));

    private bool IsActionUnlocked(HotBarSlot* dataSlot)
    {
        if (Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(Service.ClientState.TerritoryType)!.TerritoryIntendedUse == 31) return true;
        
        var action = GetAdjustedAction(dataSlot->CommandId);

        if ( action is null or { UnlockLink: 0 } ) return true;

        return UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(action.UnlockLink, 1);
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

    private int GetCrossHotBarIndex()
    {
        var actionCrossBar = (AddonActionCross*) ActionBar;

        if (actionCrossBar->ExpandedHoldControlsLTRT != 0) return actionCrossBar->ExpandedHoldControlsLTRT - 1;
        if (actionCrossBar->ExpandedHoldControlsRTLT != 0) return actionCrossBar->ExpandedHoldControlsRTLT - 1;

        return actionCrossBar->ActionBarBase.RaptureHotbarId;
    }

    private HotBarSlot* GetHotBarSlot(int index)
    {
        return Name switch
        {
            HotbarName.DoubleCrossR => HotbarModule->Slot[index + 8],
            _ => HotbarModule->Slot[index]
        };
    }

    private ActionBarSlot* GetUISlot(int index)
    {
        return Name switch
        {
            // If we are looking at LT -> RT, then we are 0 indexed, we need to refer to 4 - 12
            HotbarName.CrossHotbar when IsHoldControlLtrt() => ActionBar->ActionBarSlots + index + 4,

            // If we are looking at RT -> LR, then we are 8 indexed, we need to refer to 4 - 12
            HotbarName.CrossHotbar when IsHoldControlRtlt() => ActionBar->ActionBarSlots + index - 4,

            // All other Hotbars
            _ => ActionBar->ActionBarSlots + index
        };
    }

    private int GetHotbarIndex()
    {
        return Name switch
        {
            HotbarName.CrossHotbar when IsHoldControlLtrt() => GetCrossHotBarIndex() / 2 + 10,
            HotbarName.CrossHotbar when IsHoldControlRtlt() => GetCrossHotBarIndex() / 2 + 10,
            HotbarName.DoubleCrossR => ((AddonActionDoubleCrossBase*)ActionBar)->BarTarget,
            HotbarName.DoubleCrossL => ((AddonActionDoubleCrossBase*)ActionBar)->BarTarget,
            _ => ActionBar->RaptureHotbarId
        };
    }

    private int GetSize()
    {
        return Name switch
        {
            HotbarName.CrossHotbar when IsHoldControlLtrt() => 8,
            HotbarName.CrossHotbar when IsHoldControlRtlt() => 8,
            _ => ActionBar->SlotCount
        };
    }

    private int GetStartingIndex()
    {
        return Name switch
        {
            HotbarName.CrossHotbar when IsHoldControlRtlt() => 8,
            _ => 0
        };
    }
}