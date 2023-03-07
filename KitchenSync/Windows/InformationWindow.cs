using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace KitchenSync.Windows;

public class InformationWindow : Window
{
    public InformationWindow() : base("KitchenSync is Depricated")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(575,125),
            MaximumSize = new Vector2(575,125)
        };

        Flags |= ImGuiWindowFlags.NoResize;

        IsOpen = true;
    }
    
    public override void Draw()
    {
        ImGui.Text("KitchenSync is now deprecated (that means old and no longer relevant)\n" +
                   "A Simple Tweaks version of this plugin is now available.\n" +
                   "Please use `Fade Unavailable Actions` tweak.\n" +
                   "You can choose to ignore this and continue using KitchenSync until the next ffxiv patch.");
    }
}