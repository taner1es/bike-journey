using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public Item[] allItemData;
}

[System.Serializable]
public class Item
{
    private int itemID;
    public string itemName;
    public string itemDestination;

    public Item(string newItemDestination, int newID, string newName)
    {
        itemID = newID;
        itemName = newName;
        itemDestination = newItemDestination;
    }
}

[System.Serializable]
public class PlayerProgress
{
    public static int idCounter;
    public int idCounterSaved;
    public int lastSessionPlayerId;
    public List<Player> playersList;

    public PlayerProgress(string name)
    {
        playersList = new List<Player>();
        idCounter = 0;
        playersList.Add(new Player(name));
    }
}

[System.Serializable]
public class Player
{
    int playerID;
    string playerName;
    string destination;
    float progressPercentage;
    List<Item> learnedItems;

    public Player(string newName)
    {
        progressPercentage = 0;
        destination = "School";
        playerID = PlayerProgress.idCounter;
        if (newName == "no-name")
            playerName = "Player_" + playerID.ToString();
        else
            playerName = newName;
        learnedItems = new List<Item>();

        PlayerProgress.idCounter++;
    }

    public void CalculateProgress()
    {
        int countedItemsAll = AppController.instance.dataController.allItemData.Length;
        int countedItemsLearned = learnedItems.Count;

        progressPercentage = 100f / countedItemsAll * countedItemsLearned;

        Debug.Log("calculated progressPercentage = " + progressPercentage);
    }

    public void FindDestination()
    {
        Item[] allItems = AppController.instance.dataController.allItemData;

        foreach(Item learnedItem in learnedItems)
        {
            foreach(Item itemToCheck in allItems)
            {
                if(learnedItem.itemName != itemToCheck.itemName && string.Compare(destination,itemToCheck.itemDestination) != 0)
                {
                    destination = itemToCheck.itemDestination;
                    Debug.Log("destination changed to : " + destination);
                    return;
                }
            }
        }
    }

    public int PlayerID { get => playerID;}
    public string PlayerName { get => playerName; set => playerName = value; }
    public List<Item> LearnedItems { get => learnedItems; set => learnedItems = value; }
    public float ProgressPercentage { get => progressPercentage; private set => progressPercentage = value; }
    public string Destination { get => destination;private set => destination = value; }
}

