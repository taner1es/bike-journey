using UnityEngine;
using System.Collections.Generic;
using AppEnums;
using System;

[System.Serializable]
public class ItemData
{
    public Item[] allItemData;
}

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string itemDestination;
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
    List<string> completedDestinations;

    public Player(string newName)
    {
        progressPercentage = 0;
        destination = DestinationNames.School.ToString();
        playerID = PlayerProgress.idCounter;
        if (newName == "no-name")
            playerName = "Player_" + playerID.ToString();
        else
            playerName = newName;
        learnedItems = new List<Item>();
        completedDestinations = new List<string>();

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

        if(completedDestinations != null)
        {
            foreach (var iterator in Enum.GetValues(typeof(DestinationNames)))
            {
                //check for the current item destination already completed before
                if (!completedDestinations.Exists(e => e.EndsWith(iterator.ToString())))
                {
                    destination = iterator.ToString();
                    Debug.Log("destination changed to : " + destination);
                    return;
                }
            }
        }
        else
        {
            destination = DestinationNames.School.ToString();
            Debug.Log("CompletedDestination list is empty. destination has set to : " + destination);
        }
    }

    public void GoDest()
    {
        DestinationNames dest;
        if (Enum.TryParse<DestinationNames>(AppController.instance.currentPlayer.Destination, true, out dest))
        {
            AppController.instance.goDestination = new Destination(dest);
            if (AppController.instance.goDestination.fillItemListAgain)
                GoDest();
        }
        else
        {
            Debug.LogError("Destination not found : " + dest.ToString());
        }
    }

    public int PlayerID { get => playerID;}
    public string PlayerName { get => playerName; set => playerName = value; }
    public float ProgressPercentage { get => progressPercentage; private set => progressPercentage = value; }
    public string Destination { get => destination;private set => destination = value; }
    public List<Item> LearnedItems { get => learnedItems; set => learnedItems = value; }
    public List<string> CompletedDestinations { get => completedDestinations; set => completedDestinations = value; }
}

