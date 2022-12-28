using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Utilities;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.UserInterface.Windows;

internal class ConfigurationWindow : Window, IDisposable
{
    private static Configuration Settings => Service.Configuration;

    public ConfigurationWindow() : base("KitchenSync Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 400),
            MaximumSize = new Vector2(700,400)
        };

        Flags |= ImGuiWindowFlags.NoResize;
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
        var windowSize = ImGui.GetContentRegionAvail();

        if (ImGui.BeginChild("LeftSide", windowSize with {X = windowSize.X / 2.0f - 7.0f * ImGuiHelpers.GlobalScale}))
        {
            ImGuiHelpers.ScaledDummy(40.0f);
            
            DrawPreviews();
        }
        ImGui.EndChild();
        ImGui.SameLine();
        DrawVerticalLine();
        ImGuiHelpers.ScaledDummy(0.0f);
        ImGui.SameLine();
        
        if (ImGui.BeginChild("RightSide", windowSize with {X = windowSize.X / 2.0f}))
        {
            if(ImGui.BeginTabBar("OptionsTabBar"))
            {
                if (ImGui.BeginTabItem("Options"))
                {
                    DrawBaseOptions();
                    
                    ImGui.EndTabItem();
                }
                
                if (ImGui.BeginTabItem("Regular Hotbars"))
                {
                    InfoBox.Instance
                        .AddTitle("Hotbar Selection")
                        .AddHotbarConfiguration(Settings.HotbarSettings.Hotbars.Where(hotbar => hotbar.Key is not (HotbarName.CrossHotbar or HotbarName.DoubleCrossL or HotbarName.DoubleCrossR)))
                        .Draw();
                    
                    ImGui.EndTabItem();
                }
                
                if (ImGui.BeginTabItem("Cross Hotbars"))
                {
                    InfoBox.Instance
                        .AddTitle("CrossHotbar Selection")
                        .AddHotbarConfiguration(Settings.HotbarSettings.Hotbars.Where(hotbar => hotbar.Key is HotbarName.CrossHotbar or HotbarName.DoubleCrossL or HotbarName.DoubleCrossR))
                        .Draw();
                    
                    ImGui.EndTabItem();
                }
                
                ImGui.EndTabBar();
            }
        }
        ImGui.EndChild();
    }

    private void DrawBaseOptions()
    {
        InfoBox.Instance
            .AddTitle("Transparency")
            .AddDragFloat("", Settings.HotbarSettings.Transparency, 0.10f, 1.0f, InfoBox.Instance.InnerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle("Extra Options")
            .AddConfigCheckbox("Disable in Sanctuaries", Settings.HotbarSettings.DisableInSanctuaries)
            .AddConfigCheckbox("Apply to Macros", Settings.HotbarSettings.IncludeMacros)
            .AddConfigCheckbox("Apply to not yet unlocked", Settings.HotbarSettings.IncludeNotUnlocked, "Applies transparency to skills that you do not have unlocked")
            .Draw();
    }

    private void DrawPreviews()
    {
        InfoBox.Instance
            .AddTitle("Default Available")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), 1.0f)
            .Draw();

        InfoBox.Instance
            .AddTitle("Default Level Sync")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f})
            .Draw();

        InfoBox.Instance
            .AddTitle("Modified Level Sync")
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = Settings.HotbarSettings.Transparency.Value}).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = Settings.HotbarSettings.Transparency.Value}).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = Settings.HotbarSettings.Transparency.Value}).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = Settings.HotbarSettings.Transparency.Value}).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = Settings.HotbarSettings.Transparency.Value})
            .Draw();
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "KitchenSync", NotificationType.Success);
        Service.Configuration.Save();
    }
    
    private void DrawVerticalLine()
    {
        var contentArea = ImGui.GetContentRegionAvail();
        var cursor = ImGui.GetCursorScreenPos();
        var drawList = ImGui.GetWindowDrawList();
        var color = ImGui.GetColorU32(Colors.White);

        drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
    }
}

internal static class InfoBoxExtensions
{
    internal static InfoBox AddHotbarConfiguration(this InfoBox infoBox, IEnumerable<KeyValuePair<HotbarName, Setting<bool>>> configurations)
    {
        var list = infoBox.BeginList();

        foreach (var entry in configurations)
        {
            list.AddConfigCheckbox(entry.Key.ToString(), entry.Value);
        }

        return list.EndList();
    }
}