using System;
using System.Collections.Generic;
using Lumina.Excel;

namespace KitchenSync.System;

internal class LuminaCache<T> : IDisposable where T : ExcelRow
{
    private readonly Dictionary<uint, T> cache = new();

    public LuminaCache()
    {

    }

    public void Dispose()
    {
        
    }

    public T GetRow(uint id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return cache[id] = Service.DataManager.GetExcelSheet<T>()!.GetRow(id)!;
        }
    }
}