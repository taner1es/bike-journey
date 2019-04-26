using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;

public class Destination
{
    public DestinationNames destinationName;
    public List<Item> items = new List<Item>();

    public Destination(DestinationNames destinationToLoad)
    {
        destinationName = destinationToLoad;

        int i = 0;

        foreach(Item item in AppController.instance.dataController.allItemData)
        {
            if (item.itemDestination == destinationName.ToString())
            {
                items.Add(item);
                i++;
                if (i == 5)
                    break;
            }

        }
    }
}
