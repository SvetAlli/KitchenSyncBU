using System.Linq;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KitchenSync.Data;
using KitchenSync.Utilities;

namespace KitchenSync.Windows.Tabs;

public class RegularHotbarsTabItem : ITabItem
{
    private static Configuration Settings => Service.Configuration;
    public string TabName => "Regular Hotbars";
    public bool Enabled => true;
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle("Hotbar Selection", 0.90f)
            .AddHotbarConfiguration(Settings.HotbarSettings.Hotbars.Where(hotbar => hotbar.Key is not (HotbarName.CrossHotbar or HotbarName.DoubleCrossL or HotbarName.DoubleCrossR)))
            .Draw();
    }
}