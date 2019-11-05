using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectPaletteWindow : MonoBehaviour
{
    MapObjectPaletteHandler handler = new MapObjectPaletteHandler();
    public GameObject itemPrefab;

    public GameObject rootWindow;

    public bool hideWindowOnStart;

    public Transform itemContentRoot;
    public Transform inactivatedContentsRoot;

    Dictionary<MapObjectCategoryAttribute, List<MapObjectPaletteItemGUI>> groups = new Dictionary<MapObjectCategoryAttribute, List<MapObjectPaletteItemGUI>>();

    string currentCategory;

    private void Start()
    {
        Validate();

        SetWindowActive(hideWindowOnStart);

        AddItem(MapObjectDataMgr.Instance.GetMapObject("Category01", "Object01"));

        SelectCategory("Category01");
    }

    private void Validate()
    {
        if (itemContentRoot == null)
        {
            Debug.LogError("No ItemContentRoot Found , Please Link");
        }

        if(inactivatedContentsRoot == null)
        {
            Debug.LogError("No InactivatedContentRoot Found, Please Link");
        }
        else
        {
            inactivatedContentsRoot.gameObject.SetActive(false);
        }
    }

    void AddItem(MapObjectDataAttribute obj)
    {
        var itemObject = Instantiate(itemPrefab, inactivatedContentsRoot);
        var eventDelegator = itemObject.AddComponent<ButtonEventDelegator>();
        var comp = itemObject.GetComponent<MapObjectPaletteItemGUI>();

        if (groups.ContainsKey(obj.baseAttributeRef.category) == false)
        {
            AddCategory(obj.baseAttributeRef.category);
        }

        groups[obj.baseAttributeRef.category].Add(comp);

        comp.SetInfo(obj.sprite, obj.baseAttributeRef.name);

        eventDelegator.AddListener(ButtonEventDelegator.EventType.OneClick, OnClickObject);
        eventDelegator.AddListener(ButtonEventDelegator.EventType.DoubleClick, OnDoubleClickObject);
        eventDelegator.AddListener(ButtonEventDelegator.EventType.PointerEnter, OnPointerEnterObject);
        eventDelegator.AddListener(ButtonEventDelegator.EventType.PointerExit, OnPointerExitObject);
    }

    void AddCategory(MapObjectCategoryAttribute category)
    {
        if (groups.ContainsKey(category) == false)
        {
            groups.Add(category, new List<MapObjectPaletteItemGUI>());
        }
    }

    void UpdateUI()
    {
    }

    public void SelectCategory(string category)
    {
        foreach (var t in groups)
        {
            bool active = t.Key.name.Equals(category);

            foreach (var item in t.Value)
            {
                item.gameObject.SetActive(active);
            }
        }
    }

    public void SetWindowActive(bool active)
    {
        rootWindow.SetActive(active);
    }

    void OnClickObject(object param)
    {

    }

    void OnDoubleClickObject(object param)
    {

    }

    void OnPointerEnterObject(object param)
    {

    }

    void OnPointerExitObject(object param)
    {

    }
}
