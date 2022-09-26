using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

namespace KitchenSync.System;

internal class FateEventManager : IDisposable
{
    private delegate IntPtr FateStatusChangedDelegate(IntPtr a1, ushort fateId);

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

    private IntPtr FateStatusChanged(IntPtr a1, ushort fateId)
    {
        var result = fateStatusChangedHook!.Original(a1, fateId);

        try
        {
            if (fateId == 0)
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