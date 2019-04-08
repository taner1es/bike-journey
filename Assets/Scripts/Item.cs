using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string itemDestination;
    public string audioFileName;
    public string imageFileName;
    
    public Item(string newItemDestination, int newID, string newName, string newAudioName, string newImageName)
    {
        itemID = newID;
        itemName = newName;
        itemDestination = newItemDestination;
        audioFileName = newAudioName;
        imageFileName = newImageName;
    }
}
