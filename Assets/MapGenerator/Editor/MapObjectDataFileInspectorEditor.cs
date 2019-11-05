using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(MapObjectDataFile))]
public class MapObjectDataFileInspectorEditor : Editor
{
    public enum DirtyInfo
    {
        None = -1,
        AddCategory,
        RemoveCategory,
        ModifyCategory,
        AddObject,
        RemoveObject,
        ModifyObject,
    }

    public class ItemEditingProperty : MapObjectAttribute
    {
        public bool foldOut;
        public int selectedCategory;

        public void CopyFrom(MapObjectAttribute source)
        {
            name = source.name;
            objectPrefabResourcePath = source.objectPrefabResourcePath;
            spriteResourcePath = source.spriteResourcePath;
            hp = source.hp;
            category.name = source.category.name;
        }
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

        objsExtraProp = new List<ItemEditingProperty>(target.items.Count);

        ApplyInternalCategoriCache();

        for (int i = 0; i < objsExtraProp.Capacity; i++)
        {
            objsExtraProp.Add(new ItemEditingProperty());
            //objsExtraProp.Last().CopyFrom(target.items[i]);
        }
    }

    void ApplyInternalCategoriCache()
    {
        categories = target.category.ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = dirty;

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(target);
            dirty = false;
        }

        GUI.enabled = true;

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
                SetDirty(DirtyInfo.AddObject);
            }

            for (int i = 0; i < target.items.Count; i++)
            {
                objsExtraProp[i].foldOut = EditorGUILayout.Foldout(objsExtraProp[i].foldOut, target.items[i].name + string.Concat("(", target.items[i].category.name, ")"));

                if (objsExtraProp[i].foldOut)
                {
                    EditorGUI.indentLevel++;

                    // BackUp For detect changes
                    objsExtraProp[i].name = target.items[i].name;
                    objsExtraProp[i].objectPrefabResourcePath = target.items[i].objectPrefabResourcePath;
                    objsExtraProp[i].spriteResourcePath = target.items[i].spriteResourcePath;
                    objsExtraProp[i].category.name = target.items[i].category.name;

                    target.items[i].name = EditorGUILayout.TextField(target.items[i].name, "Name");
                    target.items[i].hp = int.Parse(EditorGUILayout.TextField("HP", target.items[i].hp.ToString()));
                    objsExtraProp[i].selectedCategory = EditorGUILayout.Popup("Category", objsExtraProp[i].selectedCategory, categories);
                    target.items[i].category.name = objsExtraProp[i].category.name;

                    if (objsExtraProp[i].name.Equals(target.items[i].name) == false ||
                        objsExtraProp[i].hp != target.items[i].hp ||
                        objsExtraProp[i].objectPrefabResourcePath.Equals(target.items[i].objectPrefabResourcePath) == false ||
                        objsExtraProp[i].spriteResourcePath.Equals(target.items[i].spriteResourcePath) == false ||
                        objsExtraProp[i].category.name.Equals(target.items[i].category.name) == false)
                    {
                        SetDirty(DirtyInfo.ModifyObject);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            if (target.category.Count > 0)
            {
                // categorySelectedObjectDummy = EditorGUILayout.Popup(categorySelectedObjectDummy, categories);
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

            if (GUILayout.Button("Add"))
            {
                bool exist = target.category.Exists(t => t.Equals(categoryToAdd));

                if (exist)
                {
                    EditorUtility.DisplayDialog("Already Exist", "Category \"" + categoryToAdd + "\" is already exist", "OK");
                }
                else
                {
                    target.category.Add(categoryToAdd);
                    categories = target.category.ToArray();
                    SetDirty(DirtyInfo.AddCategory);
                }
            }

            GUI.enabled = true;

            EditorGUILayout.LabelField("Current Categories");

            int removeID = -1;

            for (int i = 0; i < target.category.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                target.category[i] = EditorGUILayout.TextField(target.category[i]);

                if (GUILayout.Button("X"))
                {
                    removeID = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeID != -1)
            {
                target.category.Remove(target.category[removeID]);
                SetDirty(DirtyInfo.RemoveObject);
            }

            EditorGUI.indentLevel--;
        }
    }

    new void SetDirty(DirtyInfo dirtyInfo)
    {
        switch (dirtyInfo)
        {
            case DirtyInfo.None:
                break;
            case DirtyInfo.AddCategory:
                ApplyInternalCategoriCache();
                break;
            case DirtyInfo.ModifyCategory:
                break;
            case DirtyInfo.AddObject:
                break;
            case DirtyInfo.ModifyObject:
                break;
            default:
                Debug.LogError("Add Case");
                break;
        }

        dirty = true;
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
}
