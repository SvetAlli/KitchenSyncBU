using System.Linq;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.Windows.Tabs;

public class CrossHotbarSelectionTabItem : ITabItem
{
    private static Configuration Settings => Service.Configuration;
    public string TabName => "Cross Hotbars";
    public bool Enabled => true;
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle("CrossHotbar Selection", 0.90f)
            .AddHotbarConfiguration(Settings.HotbarSettings.Hotbars.Where(hotbar => hotbar.Key is HotbarName.CrossHotbar or HotbarName.DoubleCrossL or HotbarName.DoubleCrossR))
            .Draw();
    }
}