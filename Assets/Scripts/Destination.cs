using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;

public class Destination
{
    DestinationNames destinationName;
    public List<Item> items = new List<Item>();

    private bool videoWatched;
    private bool balloonGamePlayed;
    private bool matchingGamePlayed;

    public Destination(DestinationNames destinationToLoad)
    {
        AppController.instance.dataController = new DataController();
        AppController.instance.dataController.LoadGameData();

        destinationName = destinationToLoad;
        
        foreach(Item item in AppController.instance.dataController.allItemData)
        {
            if (item.itemDestination == destinationName.ToString())
                items.Add(item);
        }
    }
}
