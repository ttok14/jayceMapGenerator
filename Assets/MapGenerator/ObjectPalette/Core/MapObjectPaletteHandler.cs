using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapObjectPaletteHandler : IMapObjectPaletteSystem<MapObjectPaletteObject>
{
    Dictionary<MapObjectCategoryAttribute, List<MapObjectPaletteObject>> palettes = new Dictionary<MapObjectCategoryAttribute, List<MapObjectPaletteObject>>();

    public void AddItem(object category, MapObjectPaletteObject item)
    {
        var targetCategory = GetCategory(category);

        if (targetCategory == null)
        {
            var newCategory = new MapObjectCategoryAttribute((string)category);
            palettes.Add(newCategory, new List<MapObjectPaletteObject>() { item });
        }
        else
        {
            palettes[targetCategory].Add(item);
        }
    }

    public IEnumerable<MapObjectPaletteObject> GetItems(object category)
    {
        IEnumerable<MapObjectPaletteObject> result = null;
        var targetCategory = GetCategory(category);

        if (targetCategory != null)
        {
            result = palettes[targetCategory];
        }

        return result;
    }

    MapObjectCategoryAttribute GetCategory(object obj)
    {
        string catg = obj as string;
        MapObjectCategoryAttribute result = null;

        foreach (var t in palettes)
        {
            if (t.Key.name.Equals(catg))
            {
                result = t.Key;
                break;
            }
        }

        return result;
    }

}
