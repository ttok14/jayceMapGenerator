using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text;

[CustomEditor(typeof(MapObjectDataFile))]
public class MapObjectDataFileInspectorEditor : Editor
{
    public enum MapObjectInvalidFlag
    {
        Valid = 0,
        CategoryMissing = 0x1
    }

    public enum ObjectViewOption
    {
        뷰옵션 고 
    }

    public class ItemEditingProperty : MapObjectAttribute
    {
        public bool foldOut;
        public int selectedCategory;

        public ItemEditingProperty(string name, string category)
        {
            base.name = name;
            base.category.name = category;
        }

        public void CopyFrom(MapObjectAttribute source)
        {
            name = source.name;
            objectPrefabResourcePath = source.objectPrefabResourcePath;
            spriteResourcePath = source.spriteResourcePath;
            hp = source.hp;
            category.name = source.category.name;
        }
    }

    public struct ItemInvalidProperty
    {
        public MapObjectAttribute target;
        public MapObjectInvalidFlag flag;

        public ItemInvalidProperty(MapObjectAttribute target, MapObjectInvalidFlag flag)
        {
            this.target = target;
            this.flag = flag;
        }
    }

    public class CategoryEditingProperty : MapObjectCategoryAttribute
    {
        public CategoryEditingProperty(string category)
           : base(category)
        {

        }
    }

    new MapObjectDataFile target;

    bool categoryEdit;
    bool objectEdit;

    string categoryToAdd;
    string objectNameToAdd;

    int categorySelectedObjectDummy = 0;

    public List<ItemEditingProperty> objsExtraProp;
    public MapObjectAttribute objectDummy = new MapObjectAttribute();
    List<ItemInvalidProperty> objectInvalidInfo = new List<ItemInvalidProperty>();

    string[] categories;

    bool dirty;

    private void OnEnable()
    {
        target = (MapObjectDataFile)base.target;

        if (target.items == null)
            target.items = new List<MapObjectAttribute>();
        if (target.category == null)
            target.category = new List<string>();

        objsExtraProp = new List<ItemEditingProperty>(target.items.Count);
        //   categoryExtraProp = new List<CategoryEditingProperty>(target.category.Count);

        UpdateData();
    }

    private void UpdateData()
    {
        RebuildCategory();
        RebuildObject();
    }

    private void RebuildObject()
    {
        ResizeList(objsExtraProp, target.items.Count);

        for (int i = 0; i < target.items.Count; i++)
        {
            if (objsExtraProp[i] == null)
            {
                objsExtraProp[i] = new ItemEditingProperty("N/S", string.Empty);
            }

            objsExtraProp[i].category.name = target.items[i].category.name;
            objsExtraProp[i].hp = target.items[i].hp;
            objsExtraProp[i].name = target.items[i].name;
            objsExtraProp[i].objectPrefabResourcePath = target.items[i].objectPrefabResourcePath;
            objsExtraProp[i].spriteResourcePath = target.items[i].spriteResourcePath;
            objsExtraProp[i].selectedCategory = Array.IndexOf(categories, target.items[i].category.name);
        }
    }

    private void UpdateObjectCategoryIndex()
    {
        for (int i = 0; i < objsExtraProp.Count; i++)
        {
            objsExtraProp[i].selectedCategory = Array.IndexOf(categories, target.items[i].category.name);
        }
    }

    void RebuildCategory()
    {
        categories = target.category.ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = dirty;

        if (GUILayout.Button("Save / Update"))
        {
            if (CheckSaveValidation())
            {
                EditorUtility.SetDirty(target);
                UpdateData();
                dirty = false;
            }
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

    private bool CheckSaveValidation()
    {
        objectInvalidInfo.Clear();

        for (int i = 0; i < target.items.Count; i++)
        {
            if (target.category.Exists(t => t.Equals(target.items[i].category.name)) == false)
            {
                objectInvalidInfo.Add(new ItemInvalidProperty(target.items[i], MapObjectInvalidFlag.CategoryMissing));
            }
        }

        if (objectInvalidInfo.Count > 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Name\tFlag");

            for (int i = 0; i < objectInvalidInfo.Count; i++)
            {
                AddInvalidObjectMsg(sb, objectInvalidInfo[i]);
            }

            EditorUtility.DisplayDialog("Invalid Value Found!", sb.ToString(), "OK");

            return false;
        }

        return true;
    }

    void AddInvalidObjectMsg(StringBuilder sb, ItemInvalidProperty info)
    {
        sb.AppendLine("");
        sb.Append("Name : " + info.target.name);

        if ((info.flag & MapObjectInvalidFlag.CategoryMissing) != 0)
        {
            sb.Append(", Category Missing : " + info.target.category.name);
        }
    }

    private void DrawObjectEdit()
    {
        objectEdit = EditorGUILayout.Foldout(objectEdit, "Objects");

        if (objectEdit)
        {
            EditorGUI.indentLevel++;

            objectNameToAdd = EditorGUILayout.TextField("Name", objectNameToAdd);

            GUI.enabled = string.IsNullOrEmpty(objectNameToAdd) == false;

            if (GUILayout.Button("Add"))
            {
                var duplicate = target.items.Find(t => t.name.Equals(objectNameToAdd));

                if (duplicate != null)
                {
                    EditorUtility.DisplayDialog("Already Exist", "Object \"" + duplicate.name + "\" is already exist", "OK");
                }
                else
                {
                    string category = GetCurrentDefaultCategory();

                    target.items.Add(new MapObjectAttribute(objectNameToAdd, category));
                    objsExtraProp.Add(new ItemEditingProperty(objectNameToAdd, category));
                    SetDirty();

                    objectNameToAdd = string.Empty;
                    RefreshGUITextField();
                }
            }

            GUI.enabled = true;

            for (int i = 0; i < target.items.Count; i++)
            {
                bool remove = false;

                EditorGUILayout.BeginHorizontal();

                objsExtraProp[i].foldOut = EditorGUILayout.Foldout(objsExtraProp[i].foldOut, target.items[i].name + string.Concat(" (", target.items[i].category.name, ")"));

                if (GUILayout.Button("X"))
                {
                    remove = EditorUtility.DisplayDialog("Remove?", "Are you sure?", "YES", "NO");

                    if (remove)
                    {
                        target.items.Remove(target.items[i]);
                        RebuildObject();
                        SetDirty();
                        break;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (objsExtraProp[i].foldOut)
                {
                    EditorGUI.indentLevel++;

                    target.items[i].name = EditorGUILayout.TextField("Name", target.items[i].name);
                    target.items[i].hp = int.Parse(EditorGUILayout.TextField("HP", target.items[i].hp.ToString()));
                    objsExtraProp[i].selectedCategory = EditorGUILayout.Popup("Category", objsExtraProp[i].selectedCategory, categories);
                    int categoryIndex = objsExtraProp[i].selectedCategory;
                    target.items[i].category.name = categoryIndex >= 0 ? target.category[categoryIndex] : string.Empty;

                    if (dirty == false)
                    {
                        if (objsExtraProp[i].name.Equals(target.items[i].name) == false ||
                            objsExtraProp[i].hp != target.items[i].hp ||
                            objsExtraProp[i].objectPrefabResourcePath.Equals(target.items[i].objectPrefabResourcePath) == false ||
                            objsExtraProp[i].spriteResourcePath.Equals(target.items[i].spriteResourcePath) == false ||
                            objsExtraProp[i].category.name.Equals(target.items[i].category.name) == false)
                        {
                            SetDirty();
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
            //objectDummy.category.name= 
        }
    }

    private string GetCurrentDefaultCategory()
    {
        if (target.category.Count > 0)
            return target.category[0];
        else return string.Empty;
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
                    RebuildCategory();
                    UpdateObjectCategoryIndex();
                    SetDirty();
                    categoryToAdd = string.Empty;
                    RefreshGUITextField();
                }
            }

            GUI.enabled = true;

            EditorGUILayout.LabelField("Current Categories");

            int removeID = -1;

            // for (int i = 0; i < target.category.Count; i++)
            for (int i = 0; i < target.category.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                //    categoryExtraProp[i].name = EditorGUILayout.TextField(categoryExtraProp[i].name);
                target.category[i] = EditorGUILayout.TextField(target.category[i]);

                if (GUILayout.Button("X"))
                {
                    removeID = i;
                }
                else if (target.category[i].Equals(categories[i]) == false)
                {
                    SetDirty();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeID != -1)
            {
                target.category.Remove(target.category[removeID]);
                RebuildCategory();
                UpdateObjectCategoryIndex();
                SetDirty();
            }

            EditorGUI.indentLevel--;
        }
    }

    new void SetDirty()
    {
        dirty = true;
    }

    void RefreshGUITextField()
    {
        GUIUtility.keyboardControl = 0;
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
