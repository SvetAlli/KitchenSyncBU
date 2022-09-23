using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using HotbarPointer = FFXIVClientStructs.FFXIV.Client.UI.Misc.HotBar;

namespace KitchenSync.Utilities;

internal unsafe class Hotbar
{
    public HotbarName Name { get; }

    private AddonActionBarBase* ActionBar => (AddonActionBarBase*) Service.GameGui.GetAddonByName(Name.GetAddonName(), 1);
    private HotbarPointer* HotbarModule => Framework.Instance()->UIModule->GetRaptureHotbarModule()->HotBar[ActionBar->RaptureHotbarId];

    public Hotbar(HotbarName name)
    {
        Name = name;
    }

    public void ApplyTransparency(float percentage)
    {
        if (ActionBar == null || HotbarModule == null) return;

        var hotbarSize = ActionBar->SlotCount;

        foreach (var index in Enumerable.Range(0, hotbarSize))
        {
            var hotbarSlot = HotbarModule->Slot[index];
            var uiSlot = ActionBar->ActionBarSlots + index;

            ApplyTransparencyToSlot(uiSlot, hotbarSlot, percentage);
        }
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

    private void ApplyTransparencyToSlot(ActionBarSlot* uiSlot, HotBarSlot* dataSlot, float percentage)
    {
        if (!IsAction(dataSlot)) return;
        if (IsRoleAction(dataSlot)) return;
        if (!IsSyncAction(dataSlot)) return;

        uiSlot->Icon->AtkResNode.Color.A = (byte) (0xFF * percentage);
    }

    private void ResetTransparencyToSlot(ActionBarSlot* uiSlot)
    {
        if (uiSlot == null) return;
        
        var icon = uiSlot->Icon;
        if (icon == null) return;
        
        icon->AtkResNode.Color.A = 0xFF;
    }

    private bool IsAction(HotBarSlot* dataSlot)
    {
        return dataSlot->CommandType == HotbarSlotType.Action;
    }

    private bool IsRoleAction(HotBarSlot* dataSlot)
    {
        var action = GetAdjustedAction(dataSlot->CommandId);

        return action is {IsRoleAction: true};
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