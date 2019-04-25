using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MatchingGameEvents : MonoBehaviour
{
    class MatchingItemValues
    {
        public GameObject instance;
        public bool matched;
        public Item item;
        private float itemWidth;

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
                string spritePath = "Textures/" + item.itemName;
                var texture = Resources.Load<Texture2D>(spritePath);
                instance.GetComponent<SpriteRenderer>().sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
            }
        }
    }

    public GameObject matchingIconStandPrefab;
    public GameObject matchingAreaStandPrefab;
    public GameObject matchingIconPrefab;
    public GameObject matchingAreaPrefab;

    [Range(0f,100f)]
    public float paddingItemToRight;

    private List<MatchingItemValues> matchingIcons;
    private List<MatchingItemValues> matchingAreas;

    private Vector3 inputPosition;

    private float inputInterval;
    private bool heldStand;
    private bool heldIcon;
    private bool heldStandArea;
    private bool heldStandIcon;
    private Collider2D heldCollider;

    private int firstIndex;
    private int lastIndex;

    private void OnEnable()
    {
        matchingIcons = new List<MatchingItemValues>();
        matchingAreas = new List<MatchingItemValues>();

        CreateMatchingItems();


        heldCollider = null;
        heldStand = false;
        heldIcon = false;
        heldStandArea = false;
        heldStandIcon = false;

    }

    private void FixedUpdate()
    {
        SlideEvents();
        CheckForGameFinished();
        Debugger();
    }

    //checks  for all items matched
    private void CheckForGameFinished()
    {
        if(matchingIcons.Count == 0)
        {
            Invoke("End", 3);
        }
    }

    //matching game ends
    private void End()
    {
        ProgressController.SaveProgress();
        AppController.instance.SetStage(AppEnums.ApplicationStates.StoryMap);
    }

    //manage debug messages which is needed in runtime debugging
    private void Debugger()
    {
        Debugging.SetDebugText(
            " heldIcon: " + heldIcon.ToString() +
            "\n heldStand: " + heldStand.ToString() +
            "\n heldStandArea: " + heldStandArea +
            "\n heldStandIcon: " + heldStandIcon);
    }

    /*manages slide handler priority,
     * sample-scenerio: if icon is held then don't touch to stand slider anymore until release the icon
     * sample-scenerio-2: if slider stand held then don't touch to icons anymore until release the stand.. */
    private void SlideEvents()
    {
        GetFirstClick();

        if (heldIcon)
        {
            DragIcon();
        }
        else if (heldStand)
        {
            if(heldStandArea)
                SlideStandCheck(matchingAreaStandPrefab);
            else if(heldStandIcon)
                SlideStandCheck(matchingIconStandPrefab);
        }
    }

    //optimized input handler
    private void GetFirstClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(heldIcon)
                RevertIconToFirstPlace(heldCollider);
            heldStand = false;
            heldIcon = false;
            heldStandArea = false;
            heldStandIcon = false;
        }
        if(!heldStand & !heldIcon)
        {
            if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider;
                foreach (MatchingItemValues item in matchingIcons)
                {
                    if(item.instance != null)
                    {
                        collider = item.instance.GetComponent<Collider2D>();
                        if (collider.OverlapPoint(inputPosition))
                        {
                            heldIcon = true;
                            heldCollider = collider;
                            heldStand = false;
                            return;
                        }
                    }
                }

                collider = matchingIconStandPrefab.GetComponent<Collider2D>();
                if (collider.OverlapPoint(inputPosition))
                {
                    heldStand = true;
                    heldStandArea = false;
                    heldStandIcon = true;
                    heldCollider = collider;
                    heldIcon = false;
                    return;
                }

                collider = matchingAreaStandPrefab.GetComponent<Collider2D>();
                if (collider.GetComponent<Collider2D>().OverlapPoint(inputPosition))
                {
                    heldStand = true;
                    heldStandArea = true;
                    heldStandIcon = false;
                    heldCollider = collider;
                    heldIcon = false;
                    return;
                }
                else
                {
                    heldStand = false;
                    heldStandArea = false;
                    heldStandIcon = false;
                }
            }
        }
    }

    //slider distributor
    private void SlideStandCheck(GameObject standPrefab)
    {
        SetInputIntervalOnStand(standPrefab);
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (Input.GetMouseButton(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = standPrefab.GetComponent<Collider2D>();
            if (collider.OverlapPoint(inputPosition) && !heldIcon)
            {
                if(standPrefab.transform == matchingAreaStandPrefab.transform)
                {
                    SlideStand(standPrefab, matchingAreas);
                }
                else if (standPrefab.transform == matchingIconStandPrefab.transform )
                {
                    SlideStand(standPrefab, matchingIcons);
                }
            }
            else
            {
                heldStand = false;
            }
        }
        else
        {
            heldStand = false;
        }
    }

    //when an item removed from list update the new first and last index variables
    private void ReorderFirstIndexAndLastIndex()
    {
        for (int i = 0; i < matchingIcons.Count; i++){
            if (matchingIcons[i].instance != null)
            {
                firstIndex = i;
                break;
            }
        }

        for (int i = matchingIcons.Count-1; i > -1; i--){
            if (matchingIcons[i].instance != null)
            {
                lastIndex = i;
                break;
            }
        }

        foreach (MatchingItemValues item in matchingIcons)
        {
            RevertIconToFirstPlace(item.instance.GetComponent<Collider2D>());
        }
    }

    //checks for if the slider disappeared and reverts position
    private void GetSliderInsideScreen(GameObject standPrefab, List<MatchingItemValues> list)
    {
        int li, fi;

        if (standPrefab.CompareTag("IconStand"))
        {
            fi = firstIndex;
            li = lastIndex;
        }else if (standPrefab.CompareTag("AreaStand"))
        {
            fi = 0;
            li = matchingAreas.Count-1;
        }

        if (list[firstIndex].instance.transform.position.x > AppController.instance.cameraWidth || list[lastIndex].instance.transform.position.x < -AppController.instance.cameraWidth)
        {
            if (standPrefab.transform.position.x > 0)
                standPrefab.transform.position = new Vector3(-17, standPrefab.transform.position.y, standPrefab.transform.position.z);
            else
                standPrefab.transform.position = new Vector3(45 + list[firstIndex].instance.transform.position.x, standPrefab.transform.position.y, standPrefab.transform.position.z);
        }






    }

    //stand sliding handler
    private void SlideStand(GameObject standPrefab, List<MatchingItemValues> list)
    {
        if (standPrefab.CompareTag("IconStand"))
        {
            if (list[firstIndex].instance.transform.position.x < AppController.instance.cameraWidth && list[lastIndex].instance.transform.position.x > -AppController.instance.cameraWidth)
            {
                inputPosition = new Vector3(inputPosition.x - inputInterval, standPrefab.transform.position.y, 0);
                standPrefab.transform.position = inputPosition;
            }
            else
            {
                GetSliderInsideScreen(matchingIconStandPrefab, matchingIcons);
                GetSliderInsideScreen(matchingAreaStandPrefab, matchingAreas);

                heldStand = false;
            }
        }
        else if (standPrefab.CompareTag("AreaStand"))
        {
            if (list[0].instance.transform.position.x < AppController.instance.cameraWidth && list[list.Count-1].instance.transform.position.x > -AppController.instance.cameraWidth)
            {
                inputPosition = new Vector3(inputPosition.x - inputInterval, standPrefab.transform.position.y, 0);
                standPrefab.transform.position = inputPosition;
            }
            else
            {
                GetSliderInsideScreen(matchingIconStandPrefab, matchingIcons);
                GetSliderInsideScreen(matchingAreaStandPrefab, matchingAreas);

                heldStand = false;
            }
        }
        else
        {
            return;
        }
        
    }

    //icon dragging handler
    private void DragIcon()
    {
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (heldCollider != null)
        {
            if (Input.GetMouseButton(0))
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (heldCollider.OverlapPoint(inputPosition))
                {
                    inputPosition.z = -2;
                    if (inputPosition.x > -AppController.instance.cameraWidth && inputPosition.x < AppController.instance.cameraWidth)
                    {
                        heldCollider.transform.position = inputPosition;
                    }
                }
                else
                {
                    RevertIconToFirstPlace(heldCollider);
                }
            }
            else
            {
                RevertIconToFirstPlace(heldCollider);
            }
        }
        else
        {
            CheckIfItemMatchedAndDestroyed();
        }
    }

    //checks if an item matched correctly and destroyed from the list
    private bool CheckIfItemMatchedAndDestroyed()
    {
        if(heldIcon)
        {
            heldIcon = false;
            int i = 0;
            foreach(MatchingItemValues iterator in matchingIcons)
            {
                if(iterator.instance == null)
                {
                    //matchingAreas.RemoveAt(i);
                    matchingIcons.RemoveAt(i);
                    ReorderFirstIndexAndLastIndex();

                    //add correctly matched item to player progress data list
                    AppController.instance.currentPlayer.LearnedItems.Add(iterator.item);
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    //slider interval calculator
    private void SetInputIntervalOnStand(GameObject standPrefab)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider = standPrefab.GetComponent<Collider2D>();
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (collider.OverlapPoint(inputPosition))
            {
                inputInterval = inputPosition.x - standPrefab.transform.position.x;
                heldStand = true;
            }
            else
            {
                inputInterval = 0;
                heldStand = false;
            }
        }
    }

    //initialize all matching icons and ares
    private void CreateMatchingItems()
    {

        //remove all matchingicons if any exists from previous of processes before create new ones
        foreach(GameObject iterator in GameObject.FindGameObjectsWithTag("MatchingIcon"))
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

        SetPositionsOfItems(matchingIcons);
        SetPositionsOfItems(matchingAreas);

        firstIndex = 0;
        lastIndex = matchingIcons.Count - 1;
    }

    //set positions of the items after instantiation
    private void SetPositionsOfItems(List<MatchingItemValues> list)
    {
        int i = 0;
        foreach(MatchingItemValues iterator in list)
        {
            if (i > 0)
            {
                if(list == matchingIcons)
                    //iterator.instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3((matchingIcons[i - 1].instance.GetComponent<Renderer>().bounds.size.x + paddingItemToRight) * i, 0, -2);
                    iterator.instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3(paddingItemToRight * i, 0, -2);
                else if(list == matchingAreas)
                    //iterator.instance.transform.position = matchingAreaStandPrefab.transform.position + new Vector3((matchingAreaBoundSizeX + paddingItemToRight) * i, 0, 0);
                    iterator.instance.transform.position = matchingAreaStandPrefab.transform.position + new Vector3((paddingItemToRight-5) * i, 0, 0);
            }
            i++;
        }
    }

    //reverts held icon to first place if unholded at wrong matching position
    private void RevertIconToFirstPlace(Collider2D iconToRevert)
    {
        int i = 0;
        foreach(MatchingItemValues item in matchingIcons)
        {
            if(item.instance != null)
            {
                if (item.instance.transform == iconToRevert.transform)
                {
                    if (i > 0 && matchingIcons[i-1].instance != null)
                        matchingIcons[i].instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3(paddingItemToRight * i, 0, -2);
                    else if(i == 0)
                        matchingIcons[i].instance.transform.position = new Vector3(matchingIconStandPrefab.transform.position.x, matchingIconStandPrefab.transform.position.y, -2);
                    else
                        matchingIcons[i].instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3(paddingItemToRight * i, 0, -2);
                    heldIcon = false;
                    return;
                }
            }
            i++;
        }
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
}
