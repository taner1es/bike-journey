using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string itemDestination;
    
    public Item(string newItemDestination, int newID, string newName)
    {
        itemID = newID;
        itemName = newName;
        itemDestination = newItemDestination;
    }
}
