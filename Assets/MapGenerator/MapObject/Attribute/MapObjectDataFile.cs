using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectDataFile : ScriptableObject
{
    [HideInInspector]
    public List<string> category;
    [HideInInspector]
    public List<MapObjectAttribute> items;
}