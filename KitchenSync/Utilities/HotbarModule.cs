using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace KitchenSync.Utilities;

internal unsafe class HotbarModule
{
    private readonly AddonActionBarBase* actionBar;
    private readonly HotBar* hotbarModule;

    public HotbarModule(string addonName)
    {
        actionBar = (AddonActionBarBase*) Service.GameGui.GetAddonByName(addonName, 1);
        hotbarModule = Framework.Instance()->UIModule->GetRaptureHotbarModule()->HotBar[actionBar->RaptureHotbarId];
    }

    public void ResetAllTransparency()
    {
        var hotbarSize = actionBar->SlotCount;

        for (var i = 0; i < hotbarSize; ++i)
        {
            var uiSlot = actionBar->Slot[i];

            uiSlot.Icon->AtkResNode.Color.A = 0xFF;
        }
    }

    public void SetActionTransparency(float percentage)
    {
        var hotbarSize = actionBar->SlotCount;

        for (var i = 0; i < hotbarSize; ++i)
        {
            var moduleSlot = hotbarModule->Slot[i];
            var uiSlot = actionBar->Slot[i];

            if (moduleSlot->CommandType == HotbarSlotType.Action)
            {
                if (IsActionUnavailable(moduleSlot->CommandId))
                {
                    uiSlot.Icon->AtkResNode.Color.A = (byte) (0xFF * percentage);
                }
                else
                {
                    uiSlot.Icon->AtkResNode.Color.A = 0xFF;
                }
            }
        }
    }

    private bool IsActionUnavailable(uint actionID)
    {
        var currentLevel = Service.ClientState.LocalPlayer?.Level ?? 0;
        var adjustedActionID = ActionManager.Instance()->GetAdjustedActionId(actionID);

        if (adjustedActionID == 0) return false;

        var adjustedAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(adjustedActionID);
        if (adjustedAction == null) return false;

        if (adjustedAction.IsRoleAction) return false;

        return adjustedAction.ClassJobLevel > currentLevel;
    }
}