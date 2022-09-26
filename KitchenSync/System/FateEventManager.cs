using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;

namespace KitchenSync.System;

internal unsafe class FateEventManager : IDisposable
{
    private delegate IntPtr FateStatusChangedDelegate(FateManager* fateManager, ushort syncdFateId);

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 0F B7 81 ?? ?? ?? ?? 0F B7 DA", DetourName = nameof(FateStatusChanged))]
    private readonly Hook<FateStatusChangedDelegate>? fateStatusChangedHook = null;

    public event EventHandler? FateSyncd;
    public event EventHandler? FateUnsyncd;

    public FateEventManager()
    {
        SignatureHelper.Initialise(this);

        fateStatusChangedHook?.Enable();
    }

    public void Dispose()
    {
        fateStatusChangedHook?.Dispose();
    }

    private IntPtr FateStatusChanged(FateManager* fateManager, ushort syncdFateId)
    {
        var result = fateStatusChangedHook!.Original(fateManager, syncdFateId);

        try
        {
            if (syncdFateId == 0)
            {
                FateUnsyncd?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                FateSyncd?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something went wrong while updating Fate Status");
        }

        return result;
    }
}