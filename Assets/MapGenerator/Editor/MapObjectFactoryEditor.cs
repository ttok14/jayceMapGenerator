using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapObjectFactoryEditor : EditorWindow
{
    [MenuItem("MapTool/Open Factory")]
    static void Open()
    {
        var window = GetWindow<MapObjectFactoryEditor>();


    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create DataFile"))
        {
            var file = Resources.Load<MapObjectDataFile>(MapObjectGlobalVariables.MapObjectDataResourcePath);
            
            if (file != null)
            {
                EditorUtility.DisplayDialog("Already Exist", "Already Exist", "OK");
            }
            else
            {
                var instance = ScriptableObject.CreateInstance<MapObjectDataFile>();
                AssetDatabase.CreateAsset(instance, MapObjectGlobalVariables.MapObjectDataAssetPath);
                file = AssetDatabase.LoadAssetAtPath<MapObjectDataFile>(MapObjectGlobalVariables.MapObjectDataAssetPath);
            }

            ProjectWindowUtil.ShowCreatedAsset(file);
        }
    }
}
