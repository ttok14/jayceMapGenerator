using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// * Base MapObject Management structure
/// 맵 오브젝트의 생성 관리의 기본 구조를 담당하는 추상 클래스,
/// </summary>
/// <typeparam name="T : Object Type Managed"></typeparam>
abstract public class MapObjectEditGeneratorBase<T> : MonoBehaviour
    where T : IBaseMapObjectInstanceSetter, new()
{
    protected List<T> objs = new List<T>();
    protected IGridEditHandler gridHandler;

    bool setup;

    public void Setup(IGridEditHandler gridHandler)
    {
        setup = true;
        this.gridHandler = gridHandler;
    }

    public bool Generate(
        BaseMapObjectAttribute attribute,
       int instanceID,
       GameObject sourceObj,
       Transform parent,
       Action<T, GameObject> onCreated)
    {
        if (setup == false)
        {
            Debug.LogError("Setup First:");
            return false;
        }

        if (gridHandler.IsGridShowing() == false)
            return false;

        var target = Instantiate(sourceObj, parent);

        T instance = new T();
        instance.SetInstance(instanceID, attribute);

        objs.Add(instance);

        if (onCreated != null)
            onCreated(instance, target);

        return true;
    }

    public bool Remove(Predicate<T> findPredicate, Action<T> onDispose)
    {
        var target = objs.Find(findPredicate);
        bool result = false;

        if(target != null && 
            onDispose != null)
        {
            result = true;
            onDispose(target);
        }

        return result;
    }
}
