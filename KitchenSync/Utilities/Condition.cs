using Dalamud.Game.ClientState.Conditions;

namespace KitchenSync.Utilities;

internal static class Condition
{
    public static bool IsBoundByDuty()
    {
        var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
        var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
        var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];
            
        return baseBoundByDuty || boundBy56 || boundBy95;
    }
}