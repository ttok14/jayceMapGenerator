using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

public class MapObjectGlobalVariables
{
    public const string MapObjectDataResourceFileName = "MapObjectDataFile";
    public const string MapObjectDataResourcePath = "MapObjectData/" + MapObjectDataResourceFileName;
    public const string MapObjectDataAssetPath = "Assets/MapGenerator/Resources/MapObjectData/" + MapObjectDataResourceFileName + ".asset";
}

// 하나의 통합 맵 오브젝트 속성 하나 
// 맵 오브젝트의 데이터베이스에 저장을 하며 
// 이거 자체는 serialize 를 할 필요없는게 정상 
[Serializable]
public class MapObjectAttribute : BaseMapObjectAttribute, MapObjectResourcePathGetter
{
    public string objectPrefabResourcePath;
    public string spriteResourcePath;

    // FIX ME : TEMP 
    public int hp;

    public MapObjectAttribute() { }
    public MapObjectAttribute(string name)
    {
        base.name = name;
    }
    public MapObjectAttribute(
        string objectPrefabResourcePath,
        string spriteResourcePath,
        int hp)
    {
        this.objectPrefabResourcePath = objectPrefabResourcePath;
        this.spriteResourcePath = spriteResourcePath;
        this.hp = hp;
    }

    public string GetPrefabPath()
    {
        return objectPrefabResourcePath;
    }

    public string GetSpritePath()
    {
        return spriteResourcePath;
    }
}

[Serializable]
abstract public class MapObjectInstanceBase_JsonForm : BaseMapObjectInstanceAttribute, IJsonConvertible
{
    public double posX, posY, posZ;

    abstract public void SetJsonProperty();

    public string Convert()
    {
        SetJsonProperty();
        return JsonMapper.ToJson(this);
    }
}

public class MapObjectSingleInstance : MapObjectInstanceBase_JsonForm
{
    public GameObject gameObject;

    public override void SetJsonProperty()
    {
        posX = gameObject.transform.position.x;
        posY = gameObject.transform.position.y;
        posZ = gameObject.transform.position.z;
    }
}
