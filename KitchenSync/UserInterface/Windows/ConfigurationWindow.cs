using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KitchenSync.Data;
using KitchenSync.UserInterface.Components;
using KitchenSync.Utilities;

namespace KitchenSync.UserInterface.Windows;

internal class ConfigurationWindow : Window, IDisposable
{
    private readonly InfoBox transparency = new();
    private readonly InfoBox hotbarSelection = new();

    private static HotbarSettings Settings => Service.Configuration.HotbarSettings;

    public ConfigurationWindow() : base("KitchenSync Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200 * (16.0f / 9.0f), 400),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.AlwaysVerticalScrollbar;
    }

    public void Dispose()
    {

    }

    public override void PreOpenCheck()
    {
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {
        transparency
            .AddTitle("Transparency")
            .AddDragFloat("", Settings.Transparency, 0.10f, 1.0f, transparency.InnerWidth)
            .Draw();

        hotbarSelection
            .AddTitle("Hotbar Selection")
            .AddHotbarConfiguration(Settings.Hotbars)
            .Draw();
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "KitchenSync", NotificationType.Success);
        Service.Configuration.Save();
    }
}

internal static class InfoBoxExtensions
{
    internal static InfoBox AddHotbarConfiguration(this InfoBox infoBox, Dictionary<HotbarName, Setting<bool>> configurations)
    {
        var list = infoBox.BeginList();

        foreach (var entry in configurations)
        {
            list.AddConfigCheckbox(entry.Key.ToString(), entry.Value);
        }

        return list.EndList();
    }
}