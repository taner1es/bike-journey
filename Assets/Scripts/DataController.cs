using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class DataController
{
    public Item[] allItemData;
    string gameObjectDataFileName = "itemList";

    public void LoadGameData()
    {
        TextAsset targetFile;
        
        if (targetFile = Resources.Load<TextAsset>(gameObjectDataFileName))
        {
            string dataAsJson = targetFile.text;
            ItemData loadedData = JsonUtility.FromJson<ItemData>(dataAsJson);

            allItemData = loadedData.allItemData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
}
