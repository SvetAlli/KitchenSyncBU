using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Utilities;
using KitchenSync.Data;
using KitchenSync.Utilities;
using KitchenSync.Windows.Tabs;

namespace KitchenSync.Windows;

internal class ConfigurationWindow : Window
{
    private static Configuration Settings => Service.Configuration;

    private readonly TabBar tabBar = new("KitchenSyncTabBar", Vector2.Zero);
    
    public ConfigurationWindow() : base("KitchenSync Configuration")
    {
        KamiCommon.CommandManager.AddCommand(new ConfigurationWindowCommands<ConfigurationWindow>());
        
        tabBar.AddTab(new BaseOptionsTabItem());
        tabBar.AddTab(new RegularHotbarsTabItem());
        tabBar.AddTab(new CrossHotbarSelectionTabItem());
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(575,350),
            MaximumSize = new Vector2(575,350)
        };

        Flags |= ImGuiWindowFlags.NoResize;
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
            ImGuiHelpers.ScaledDummy(30.0f);
            DrawPreviews();
            DrawVersionNumber();
        }
        ImGui.EndChild();
        ImGui.SameLine();
        DrawVerticalLine();
        ImGuiHelpers.ScaledDummy(0.0f);
        ImGui.SameLine();
        
        if (ImGui.BeginChild("RightSide", windowSize with {X = windowSize.X / 2.0f}))
        {
            tabBar.Draw();
        }
        ImGui.EndChild();
    }
    
    private void DrawPreviews()
    {
        InfoBox.Instance
            .AddTitle("Default Available", 1.0f)
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), 1.0f).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), 1.0f)
            .Draw();

        InfoBox.Instance
            .AddTitle("Default Level Sync", 1.0f)
            .AddIcon(454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3064, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3662, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(3454, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f}).SameLine()
            .AddIcon(216, ImGuiHelpers.ScaledVector2(40.0f), new Vector4(0.50f) {W = 1.0f})
            .Draw();

        InfoBox.Instance
            .AddTitle("Modified Level Sync", 1.0f)
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
    
    private void DrawVersionNumber()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');
        var versionString = assemblyInformation[1].Replace('=', ' ');

        var stringSize = ImGui.CalcTextSize(versionString);

        var x = ImGui.GetContentRegionAvail().X / 2.0f - stringSize.X / 2;
        var y = ImGui.GetWindowHeight() - 25 * ImGuiHelpers.GlobalScale;
            
        ImGui.SetCursorPos(new Vector2(x, y));

        ImGui.TextColored(Colors.Grey, versionString);
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