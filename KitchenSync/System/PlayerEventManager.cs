using System;
using Dalamud.Game;
using Dalamud.Logging;

namespace KitchenSync.System;

internal class PlayerEventManager : IDisposable
{
    public event EventHandler? PlayerLevelChanged;

    private int lastPlayerLevel;

    public PlayerEventManager()
    {
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.LocalPlayer == null) return;

        var playerLevel = Service.ClientState.LocalPlayer.Level;

        if (playerLevel != lastPlayerLevel)
        {
            PluginLog.Debug($"Player Level Changed, {lastPlayerLevel} -> {playerLevel}");

            lastPlayerLevel = playerLevel;
            PlayerLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}