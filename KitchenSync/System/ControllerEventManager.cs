using System;
using System.Threading.Tasks;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KitchenSync.System;

internal unsafe class ReceiveEventArgs : EventArgs
{
    public ReceiveEventArgs(AgentInterface* agentInterface, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong senderID)
    {
        AgentInterface = agentInterface;
        RawData = rawData;
        EventArgs = eventArgs;
        EventArgsCount = eventArgsCount;
        SenderID = senderID;
    }

    public AgentInterface* AgentInterface;
    public void* RawData;
    public AtkValue* EventArgs;
    public uint EventArgsCount;
    public ulong SenderID;

    public void PrintData()
    {
        PluginLog.Verbose("ReceiveEvent Argument Printout --------------");
        PluginLog.Verbose($"AgentInterface: {(IntPtr)AgentInterface:X8}");
        PluginLog.Verbose($"RawData: {(IntPtr)RawData:X8}");
        PluginLog.Verbose($"EventArgs: {(IntPtr)EventArgs:X8}");
        PluginLog.Verbose($"EventArgsCount: {EventArgsCount}");
        PluginLog.Verbose($"SenderID: {SenderID}");

        for (var i = 0; i < EventArgsCount; i++)
        {
            PluginLog.Verbose($"[{i}] {EventArgs[i].Int}, {EventArgs[i].Type}");
        }

        PluginLog.Verbose("End -----------------------------------------");
    }
}

internal unsafe class ControllerEventManager : IDisposable
{
    private delegate IntPtr AgentReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, uint eventDataItemCount, ulong senderID);
    private readonly Hook<AgentReceiveEvent>? agentReceiveHook = null;
    
    public event EventHandler? ControllerHotbarUpdate;

    public ControllerEventManager()
    {
        var agent = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);

        agentReceiveHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent->VTable->ReceiveEvent), OnReceiveEvent);
        agentReceiveHook?.Enable();
    }

    public void Dispose()
    {
        agentReceiveHook?.Dispose();
    }

    private IntPtr OnReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, uint eventDataItemCount, ulong senderID)
    {
        var result = agentReceiveHook!.Original(addon, a2, eventData, eventDataItemCount, senderID);

        try
        {
            if (eventDataItemCount == 5 &&
                eventData[0].Int == 8 &&
                eventData[1].Int == 1 &&
                eventData[2].UInt == 62 &&
                eventData[3].UInt is >= 10 and <= 17)
            {
                Task.Delay(TimeSpan.FromMilliseconds(1)).ContinueWith(_ => ControllerHotbarUpdate?.Invoke(this, new ReceiveEventArgs(addon, a2, eventData, eventDataItemCount, senderID)));
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "LotteryWeekly Receive Event ran into a problem");
        }

        return result;
    }
}