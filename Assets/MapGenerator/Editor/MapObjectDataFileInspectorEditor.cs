using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(MapObjectDataFile))]
public class MapObjectDataFileInspectorEditor : Editor
{
    public class ItemEditingProperty
    {
        public bool foldOut;
        public int selectedCategory;
    }

    new MapObjectDataFile target;

    bool categoryEdit;
    bool objectEdit;

    string categoryToAdd;

    int categorySelectedObjectDummy = 0;

    public List<ItemEditingProperty> objsExtraProp;
    public MapObjectAttribute objectDummy = new MapObjectAttribute();

    string[] categories;

    bool dirty;

    private void OnEnable()
    {
        target = (MapObjectDataFile)base.target;

        categories = target.category.ToArray();

        objsExtraProp = new List<ItemEditingProperty>(target.items.Count);

        for (int i = 0; i < objsExtraProp.Capacity; i++)
        {
            objsExtraProp.Add(new ItemEditingProperty());
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("PrintInfo"))
        {
            Debug.Log("Attribute Exist : " + (target.items != null && target.items.Count > 0));

            if (target.category != null)
            {
                foreach (var item in target.category)
                {
                    Debug.Log("category : " + item);
                }
            }

            if (target.items != null)
            {
                foreach (var item in target.items)
                {
                    Debug.Log("item : " + item.name);
                }
            }
        }

        DrawCategoryEdit();
        DrawObjectEdit();

        if (dirty)
        {
            dirty = false;
            EditorUtility.SetDirty(target);
        }
    }

    private void DrawObjectEdit()
    {
        objectEdit = EditorGUILayout.Foldout(objectEdit, "Objects");

        if (objectEdit)
        {
            EditorGUI.indentLevel++;

            if (GUILayout.Button("Add"))
            {
                target.items.Add(new MapObjectAttribute("N/S"));
                objsExtraProp.Add(new ItemEditingProperty());
            }

            for (int i = 0; i < target.items.Count; i++)
            {
                objsExtraProp[i].foldOut = EditorGUILayout.Foldout(objsExtraProp[i].foldOut, target.items[i].name + string.Concat("(", target.items[i].category.name, ")"));

                if (objsExtraProp[i].foldOut)
                {
                    EditorGUI.indentLevel++;

                    target.items[i].name = EditorGUILayout.TextField(target.items[i].name, "Name");
                    objsExtraProp[i].selectedCategory = EditorGUILayout.Popup(objsExtraProp[i].selectedCategory, categories);

                    EditorGUI.indentLevel--;
                }
            }

            if (target.category.Count > 0)
            {
                //                categorySelectedObjectDummy = EditorGUILayout.Popup(categorySelectedObjectDummy, categories);
            }
            else
            {
            }

            EditorGUI.indentLevel--;
            //objectDummy.category.name= 
        }
    }

    private void DrawCategoryEdit()
    {
        categoryEdit = EditorGUILayout.Foldout(categoryEdit, "Category");

        if (categoryEdit)
        {
            EditorGUI.indentLevel++;

            categoryToAdd = EditorGUILayout.TextField("CategoryName", categoryToAdd);

            GUI.enabled = string.IsNullOrEmpty(categoryToAdd) == false;

            if (GUILayout.Button("Add Category"))
            {
                bool exist = target.category.Exists(t => t.Equals(categoryToAdd));

                if (exist)
                {
                    EditorUtility.DisplayDialog("Already Exist", "Category \"" + categoryToAdd + "\" is already exist", "OK");
                }
                else
                {
                    target.category.Add(categoryToAdd);
                    SetDirty();

                    categories = target.category.ToArray();
                }
            }

            EditorGUI.indentLevel--;
            GUI.enabled = true;
        }
    }

    void ResizeList<T>(List<T> list, int cnt)
        where T : class
    {
        if (cnt != list.Count)
        {
            if (cnt < list.Count)
            {
                list.RemoveRange(cnt, list.Count - cnt);
            }
            else
            {
                int c = cnt - list.Count;

                for (int i = 0; i < c; i++)
                {
                    list.Add(null);
                }
            }
        }
    }

    new void SetDirty()
    {
        dirty = true;
    }
}
