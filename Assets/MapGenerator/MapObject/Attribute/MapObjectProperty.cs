using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseMapObjectCategoryGetter
{
    MapObjectCategoryAttribute GetCategory();
}

public interface IBaseMapObjectAttributeSetter
{
    void SetIdentifier(string name, MapObjectCategoryAttribute category);
}

public interface IBaseMapObjectInstanceSetter
{
    void SetInstance(int instanceID, BaseMapObjectAttribute baseAttributeRef);
}

public interface MapObjectResourcePathGetter
{
    string GetPrefabPath();
    string GetSpritePath();
}

// 맵 오브젝트의 카테고리 정보 
[Serializable]
public class MapObjectCategoryAttribute
{
    public string name;

    public MapObjectCategoryAttribute() { }
    public MapObjectCategoryAttribute(string name)
    {
        this.name = name;
    }
}

// 하나의 맵 오브젝트의 가장 기본적인 정보 
[Serializable]
abstract public class BaseMapObjectAttribute : IBaseMapObjectCategoryGetter, IBaseMapObjectAttributeSetter
{
    public string name;
    public MapObjectCategoryAttribute category;

    public BaseMapObjectAttribute()
    {
        category = new MapObjectCategoryAttribute();
    }

    public BaseMapObjectAttribute(string name, MapObjectCategoryAttribute category)
    {
        SetIdentifier(name, category);
    }

    public MapObjectCategoryAttribute GetCategory()
    {
        return category;
    }

    public void SetIdentifier(string name, MapObjectCategoryAttribute category)
    {
        this.name = name;
        this.category = category;
    }
}

public class MapObjectDataAttribute
{
    public BaseMapObjectAttribute baseAttributeRef;

    public GameObject prefab;
    public Sprite sprite;

    public MapObjectDataAttribute(
        BaseMapObjectAttribute baseAttributeRef,
        GameObject prefab,
        Sprite sprite)
    {
        this.baseAttributeRef = baseAttributeRef;
        this.prefab = prefab;
        this.sprite = sprite;
    }
}

// 맵 오브젝트를 인스턴스화했을때의 기본적인 정보 
// 중요한건 BaseMapObjectAttribute 를 상속받지않고 포함하여 들고있어서 
// 참조만걸어줌 
[Serializable]
abstract public class BaseMapObjectInstanceAttribute : IBaseMapObjectInstanceSetter
{
    public BaseMapObjectAttribute baseAttributeRef;
    public int instanceID;

    public void SetInstance(int instanceID, BaseMapObjectAttribute baseAttributeRef)
    {
        this.baseAttributeRef = baseAttributeRef;
        this.instanceID = instanceID;
    }
}