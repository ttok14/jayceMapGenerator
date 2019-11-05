using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapObjectPaletteSystem<T>
    where T : MapObjectPaletteObjectBase
{
    void AddItem(object category, T item);
    IEnumerable<T> GetItems(object category);
}
