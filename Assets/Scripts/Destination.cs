using System.Collections.Generic;
using AppEnums;

public class Destination
{
    public DestinationNames destinationName;
    public List<Item> items;
    public bool fillItemListAgain;

    public Destination(DestinationNames destinationToLoad)
    {
        if (AppController.instance.currentPlayer.LearnedItems.Count != AppController.instance.dataController.allItemData.Length)
            FillItemList(destinationToLoad);
        else
            AppController.instance.currentPlayer.Finished = true;
    }

    private void FillItemList(DestinationNames destinationToLoad)
    {
        destinationName = destinationToLoad;

        int i = 0;
        items = new List<Item>();

        foreach (Item item in AppController.instance.dataController.allItemData)
        {
            //check for desired destination
            if (item.itemDestination == destinationName.ToString())
            {
                //check for item learned before
                if (!AppController.instance.currentPlayer.LearnedItems.Exists(e => e.itemID.Equals(item.itemID)))
                {
                    items.Add(item);
                    i++;
                    if (i == 5)
                        break;
                }
            }
        }

        //means there is no more item to learn. So, find next destination
        if (i == 0)
        {
            if (!AppController.instance.currentPlayer.CompletedDestinations.Exists(e => e == destinationToLoad.ToString()))
                AppController.instance.currentPlayer.CompletedDestinations.Add(destinationToLoad.ToString());

            AppController.instance.currentPlayer.FindNextDestination();
            StoryMap.stay = false;
            fillItemListAgain = true;
        }
        else
        {
            if (AppController.instance.currentPlayer.LearnedItems.Count == 0)
                StoryMap.stay = false;
            fillItemListAgain = false;
        }
    }
}
