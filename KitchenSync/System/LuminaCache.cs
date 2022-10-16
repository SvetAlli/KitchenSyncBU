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
        if (cache.ContainsKey(id))
        {
            return cache[id];
        }
        else
        {
            cache.Add(id, Service.DataManager.GetExcelSheet<T>()!.GetRow(id)!);
            return cache[id];
        }
    }
}