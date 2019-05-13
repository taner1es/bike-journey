using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class GameDataEditor : EditorWindow
{
    public ItemData itemData;
    private string gameObjectDataFileName = "itemList";

    Vector2 scrollPos;

    [MenuItem("Window/Game Data Editor")]
    static void Init()
    {
        GameDataEditor window = (GameDataEditor)EditorWindow.GetWindow(typeof(GameDataEditor));
        window.maxSize = new Vector2(1920, 1080);
        window.minSize = new Vector2(300, 1000);
        window.Show();

    }

    void OnGUI()
    {
        scrollPos = scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(this.minSize.x), GUILayout.Height(this.minSize.y));

        if (itemData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("itemData");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Save data"))
            {
                SaveGameData();
            }
        }
        if(GUILayout.Button("Load data"))
        {
            LoadGameData();
        }
        EditorGUILayout.EndScrollView();
    }

    private void LoadGameData()
    {
        if (Resources.Load<TextAsset>(gameObjectDataFileName))
        {
            AssetDatabase.ImportAsset("Assets/Resources/itemList.json");
            string dataAsJson = Resources.Load<TextAsset>(gameObjectDataFileName).text;
            itemData = JsonUtility.FromJson<ItemData>(dataAsJson);
        }
        else
        {
            itemData = new ItemData();
        }
        AssetDatabase.Refresh();
    }
    void SaveGameData()
    {
        int lower = itemData.allItemData.GetLowerBound(0);
        int upper = itemData.allItemData.GetUpperBound(0);

        for (int i = lower; i <= upper; i++)
        {
            itemData.allItemData[i].itemID = i;
        }

        string dataAsJson = JsonUtility.ToJson(itemData);
        string filePath = Application.dataPath + "/Resources/itemList.json";
        File.WriteAllText(filePath, dataAsJson);

        AssetDatabase.Refresh();
    }
}
