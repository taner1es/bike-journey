using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using AppEnums;
using UnityEngine.UI;

public class MatchingGameEvents : MonoBehaviour
{
    class MatchingItemValues
    {
        public GameObject instance;
        public bool matched;
        public Item item;
        public Vector2 positionToRevert;

        //Argument : flag , if(true) -> Icon, if(false) ->area
        public MatchingItemValues(Item newItem, GameObject newInstance, bool flag)
        {
            item = newItem;
            matched = false;
            instance = newInstance;
            instance.name = item.itemName;

            if (!flag)
            {
                //add area text
                instance.transform.GetChild(0).GetComponent<TextMeshPro>().text = item.itemName;
            }
            else
            {
                string spritePath = "Textures/Items/Destinations/" + item.itemDestination + "/" + item.itemName;
                var texture = Resources.Load<Texture2D>(spritePath);
                instance.GetComponent<SpriteRenderer>().sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
                positionToRevert = instance.transform.position;
            }
        }
    }

    public GameObject matchingIconStandPrefab;
    public GameObject matchingAreaStandPrefab;
    public GameObject matchingIconPrefab;
    public GameObject matchingAreaPrefab;

    private List<MatchingItemValues> matchingIcons;
    private List<MatchingItemValues> matchingAreas;
    private bool heldIcon;
    private Collider2D heldCollider;

    private void OnEnable()
    {
        CreateMatchingItems();
    }

    private void Update()
    {
        DragIcon();
    }

    //checks  for all items matched
    private void CheckForGameFinished()
    {
        if (!GameObject.FindGameObjectWithTag("MatchingIcon"))
        {
            StartCoroutine(End(3));
        }
    }

    //matching game ends in seconds 
    private IEnumerator End(int inSec)
    {
        yield return new WaitForSeconds(inSec);

        ProgressController.SaveProgress();
        AppController.instance.SetState(AppEnums.ApplicationStates.StoryMap);
    }

    private void DragIcon()
    {
        if (!heldIcon)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider;
                foreach (MatchingItemValues item in matchingIcons)
                {
                    if (item.instance != null)
                    {
                        collider = item.instance.GetComponent<Collider2D>();
                        if (collider.OverlapPoint(inputPosition))
                        {
                            heldIcon = true;
                            heldCollider = collider;
                            return;
                        }
                        else
                        {
                            heldCollider = null;
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && heldCollider != null)
            {
                Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                inputPosition.z = 0;
                heldCollider.transform.position = inputPosition;
            }
            else
            {
                SortIcons();
                heldIcon = false;
                heldCollider = null;

                if (CheckIfItemMatchedAndDestroyed())
                {
                    CheckForGameFinished();
                }
            }
        }
    }

    //checks if an item matched correctly and destroyed from the iconlist
    private bool CheckIfItemMatchedAndDestroyed()
    {
        int i = 0;
        foreach (MatchingItemValues iterator in matchingIcons)
        {
            if (iterator.instance == null)
            {
                //matchingAreas.RemoveAt(i);
                matchingIcons.RemoveAt(i);

                //add correctly matched item to player progress data list
                AppController.instance.currentPlayer.LearnedItems.Add(iterator.item);

                //pronounce item name
                SoundManager.instance.PronounceItemName(iterator.item);
                return true;
            }
            i++;
        }

        return false;
    }

    //triggers the unity UI Layout component to sort all elements again.
    private void SortIcons()
    {
        matchingIconStandPrefab.GetComponent<HorizontalLayoutGroup>().enabled = false;
        matchingIconStandPrefab.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    private void CreateMatchingItems()
    {

        matchingIcons = new List<MatchingItemValues>();
        matchingAreas = new List<MatchingItemValues>();

        //remove all matchingicons if any exists from previous of processes before create new ones
        foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("MatchingIcon"))
        {
            Destroy(iterator);
        }

        //remove all matchingarea if any exists from previous of processes before create new ones
        foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("MatchingArea"))
        {
            Destroy(iterator);
        }

        //also clear lists to be sure that they are empty
        matchingAreas.Clear();
        matchingIcons.Clear();

        //create buttons dynamically
        ShuffleItems<Item>(AppController.instance.goDestination.items);
        foreach (Item item in AppController.instance.goDestination.items)
            matchingIcons.Add(new MatchingItemValues(item, Instantiate(matchingIconPrefab, matchingIconStandPrefab.transform), true));

        ShuffleItems<Item>(AppController.instance.goDestination.items);
        foreach (Item item in AppController.instance.goDestination.items)
            matchingAreas.Add(new MatchingItemValues(item, Instantiate(matchingAreaPrefab, matchingAreaStandPrefab.transform), false));
    }

    //shuffling list, used for item list to shuffle before instantiation
    private void ShuffleItems<T>(List<T> list)
    {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    //takes user back to storymap
    public void OnBackButtonClicked()
    {
        AppController.instance.SetState(ApplicationStates.StoryMap);
    }

}
