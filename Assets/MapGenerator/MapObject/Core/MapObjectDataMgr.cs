using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapObjectDataMgr
{
    static MapObjectDataMgr instance;
    public static MapObjectDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapObjectDataMgr();
                instance.Setup();
            }

            return instance;
        }
    }

    Dictionary<MapObjectCategoryAttribute, List<MapObjectDataAttribute>> data = new Dictionary<MapObjectCategoryAttribute, List<MapObjectDataAttribute>>();

    public void Setup()
    {
        string loadDataPath = MapObjectGlobalVariables.MapObjectDataResourcePath;

        var itemsData = Resources.Load(loadDataPath);

        if (itemsData == null)
        {
            Debug.LogError("Could not find Item Data at : " + loadDataPath);
        }
        else
        {

        }
    }

    public List<MapObjectDataAttribute> GetMapObjectList(string category)
    {
        List<MapObjectDataAttribute> result = null;

        foreach (var t in data)
        {
            if (t.Key.name.Equals(category))
            {
                result = t.Value;
                break;
            }
        }

        return result;
    }

    public MapObjectDataAttribute GetMapObject(string category, string objectName)
    {
        var list = GetMapObjectList(category);

        if (list == null)
            return null;

        MapObjectDataAttribute targetObject = null;

        foreach (var t in list)
        {
            if (t.baseAttributeRef.name.Equals(objectName))
            {
                targetObject = t;
                break;
            }
        }

        return targetObject;
    }
}
