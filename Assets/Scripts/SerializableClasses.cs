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
    public List<Player> playersList;

    public PlayerProgress(string name)
    {
        idCounter = 0;
        playersList.Add(new Player(name));
    }
}

[System.Serializable]
public class Player
{
    int playerID;
    string playerName;
    List<Item> learnedItems;

    public Player(string newName)
    {
        PlayerID = PlayerProgress.idCounter;
        PlayerName = newName;

        PlayerProgress.idCounter++;
    }

    public int PlayerID { get => playerID; set => playerID = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public List<Item> LearnedItems { get => learnedItems; set => learnedItems = value; }
}

