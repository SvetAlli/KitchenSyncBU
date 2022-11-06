using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
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
    private readonly InfoBox previewMode = new();
    private readonly InfoBox baseline = new();
    private readonly InfoBox vanilla = new();
    private readonly InfoBox extraOptions = new();

    private static Configuration Settings => Service.Configuration;

    public ConfigurationWindow() : base("KitchenSync Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350, 350),
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
        baseline
            .AddTitle("Default Available")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), 1.0f)
            .Draw();

        vanilla
            .AddTitle("Default Level Sync")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f})
            .Draw();

        previewMode
            .AddTitle("Modified Level Sync")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) { W = Settings.HotbarSettings.Transparency.Value }).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) { W = Settings.HotbarSettings.Transparency.Value }).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) { W = Settings.HotbarSettings.Transparency.Value }).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) { W = Settings.HotbarSettings.Transparency.Value }).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) { W = Settings.HotbarSettings.Transparency.Value })
            .Draw();

        transparency
            .AddTitle("Transparency")
            .AddDragFloat("", Settings.HotbarSettings.Transparency, 0.10f, 1.0f, transparency.InnerWidth)
            .Draw();

        extraOptions
            .AddTitle("Extra Options")
            .AddConfigCheckbox("Disable in Sanctuaries", Settings.HotbarSettings.DisableInSanctuaries)
            .AddConfigCheckbox("Apply to Macros", Settings.HotbarSettings.IncludeMacros)
            .AddConfigCheckbox("Apply to not yet unlocked", Settings.HotbarSettings.IncludeNotUnlocked, "Applies transparency to skills that you do not have unlocked")
            .Draw();

        hotbarSelection
            .AddTitle("Hotbar Selection")
            .AddHotbarConfiguration(Settings.HotbarSettings.Hotbars)
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