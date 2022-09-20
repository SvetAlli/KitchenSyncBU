using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Logging;
using KitchenSync.Utilities;

namespace KitchenSync.System;

internal unsafe class KitchenSyncLogic : IDisposable
{
    private readonly CancellationTokenSource cancellationToken = new();

    public KitchenSyncLogic()
    {
        UpdateHotbar();
    }

    private void UpdateHotbar()
    {
        Service.Framework.RunOnTick(UpdateHotbar, TimeSpan.FromMilliseconds(100), 0, cancellationToken.Token);

        try
        {
            foreach (var index in Enumerable.Range(8, 12))
            {
                var hotbarSlots = new BaseNode($"_ActionBar")
                    .GetNestedNode((uint) index, 3, 2)
                    .GetImageNode(17);
                    
                if (hotbarSlots->AtkResNode is {MultiplyBlue:50, MultiplyRed:50, MultiplyGreen:50})
                {
                    hotbarSlots->AtkResNode.Color.A = 0x35;
                }
                else
                {
                    hotbarSlots->AtkResNode.Color.A = 0xFF;
                }
            }

            foreach (var hotbar in Enumerable.Range(1, 3))
            {
                foreach (var index in Enumerable.Range(8, 12))
                {
                    var hotbarSlots = new BaseNode($"_ActionBar0{hotbar}")
                        .GetNestedNode((uint) index, 3, 2)
                        .GetImageNode(17);
                    
                    if (hotbarSlots->AtkResNode is {MultiplyBlue:50, MultiplyRed:50, MultiplyGreen:50})
                    {
                        hotbarSlots->AtkResNode.Color.A = 0x35;
                    }
                    else
                    {
                        hotbarSlots->AtkResNode.Color.A = 0xFF;
                    }
                }
            }
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "you fucked up");
        }
    }

    public void Dispose()
    {
        cancellationToken.Dispose();
    }
}