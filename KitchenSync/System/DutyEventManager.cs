﻿using System;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using Condition = KitchenSync.Utilities.Condition;

namespace KitchenSync.System;

internal unsafe class DutyEventManager : IDisposable
{
    private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);

    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(ProcessNetworkPacket))]
    private readonly Hook<DutyEventDelegate>? dutyEventHook = null;

    public event EventHandler<uint>? DutyStarted;
    public event EventHandler<uint>? DutyCompleted;
    public event EventHandler<uint>? DutyWipe;
    public event EventHandler<uint>? DutyRecommence;

    private bool dutyStartedThisInstance;

    public DutyEventManager()
    {
        SignatureHelper.Initialise(this);
        dutyEventHook?.Enable();

        if (Condition.IsBoundByDuty())
        {
            PluginLog.Debug("Plugin was loaded while in duty, triggering start delayed 1000ms");
            Task.Delay(1000).ContinueWith(_ => DutyStarted?.Invoke(this, Service.ClientState.TerritoryType) );
        }
        else
        {
            PluginLog.Debug("Plugin was loaded while not in duty, triggering reset delayed 1000ms");
            Task.Delay(1000).ContinueWith(_ => DutyCompleted?.Invoke(this, Service.ClientState.TerritoryType) );
        }

        Service.Framework.Update += FrameworkUpdate;
        Service.ClientState.TerritoryChanged += TerritoryChanged;
    }
    
    public void Dispose()
    {
        dutyEventHook?.Dispose();
        
        Service.Framework.Update -= FrameworkUpdate;
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
    }

    private void FrameworkUpdate(Framework framework)
    {
        if (!dutyStartedThisInstance && Condition.IsBoundByDuty() && Service.Condition[ConditionFlag.InCombat])
        {
            PluginLog.Debug("Combat Initiated Start Triggered");
            dutyStartedThisInstance = true;
            DutyStarted?.Invoke(this, Service.ClientState.TerritoryType);
        }
    }

    private void TerritoryChanged(object? sender, ushort e)
    {
        PluginLog.Debug("Zone Change Reset Triggered");
        dutyStartedThisInstance = false;
        DutyCompleted?.Invoke(this, Service.ClientState.TerritoryType);
    }

    private byte ProcessNetworkPacket(void* a1, void* a2, ushort* a3)
    {
        try
        {
            var category = *a3;
            var type = *(uint*)(a3 + 4);

            // DirectorUpdate Category
            if (category == 0x6D)
            {
                switch (type)
                {
                    // Duty Commenced
                    case 0x40000001:
                        PluginLog.Debug("Duty Commenced Event Triggered");
                        DutyStarted?.Invoke(this, Service.ClientState.TerritoryType);
                        dutyStartedThisInstance = true;
                        break;

                    // Party Wipe
                    case 0x40000005:
                        DutyWipe?.Invoke(this, Service.ClientState.TerritoryType);
                        break;

                    // Duty Recommence
                    case 0x40000006:
                        DutyRecommence?.Invoke(this, Service.ClientState.TerritoryType);
                        break;

                    // Duty Completed
                    case 0x40000003:
                        //DutyCompleted?.Invoke(this, Service.ClientState.TerritoryType);
                        //dutyStartedThisInstance = false;
                        //PluginLog.Debug("Duty Complete Event Triggered");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to get duty status");
        }

        return dutyEventHook!.Original(a1, a2, a3);
    }
}