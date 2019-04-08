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
    int firstIndex;
    int lastIndex;

    string heldIconName = "icon", collidedAreaName = "area";

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

    void FixedUpdate()
    {
        SlideEvents();
        CheckForGameFinished();
        Debugger();
    }

    private void CheckForGameFinished()
    {
        if(matchingIcons.Count == 0)
        {
            AppController.instance.InitializeApp();
        }
    }

    private void Debugger()
    {
        Debugging.SetDebugText(
            " heldIcon: " + heldIcon.ToString() +
            "\n heldStand: " + heldStand.ToString() +
            "\n heldStandArea: " + heldStandArea +
            "\n heldStandIcon: " + heldStandIcon +
            /*"\n heldCollider: " + heldCollider  == null ? "null" : heldCollider.gameObject.name +*/
            "\n heldIconName: " + heldIconName +
            "\n collidedAreaName: " + collidedAreaName);
    }

    

    void SlideEvents()
    {
        GetFirstClick();

        if (heldIcon)
        {
            SlideIcon();
        }
        else if (heldStand)
        {
            if(heldStandArea)
                SlideStandCheck(matchingAreaStandPrefab);
            else if(heldStandIcon)
                SlideStandCheck(matchingIconStandPrefab);
        }
    }

    void GetFirstClick()
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
    void SlideStandCheck(GameObject standPrefab)
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

    void ReorderFirstIndexAndLastIndex()
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

    void SlideStand(GameObject standPrefab, List<MatchingItemValues> list)
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

    void SlideIcon()
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
    
    bool CheckIfItemMatchedAndDestroyed()
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
                    return true;
                }
                i++;
            }
        }
        return false;
    }
    
    //slider interval calculator
    void SetInputIntervalOnStand(GameObject standPrefab)
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

    //initialize all matchin areas
    void CreateMatchingItems()
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
        int i = 0;
        float matchingAreaBoundSizeX = matchingAreaPrefab.GetComponent<Renderer>().bounds.size.x;
        
        foreach (Item item in AppController.instance.goDestination.items)
        {
            matchingAreas.Add(new MatchingItemValues(item, Instantiate(matchingAreaPrefab, matchingAreaStandPrefab.transform), false));
            matchingIcons.Add(new MatchingItemValues(item, Instantiate(matchingIconPrefab, matchingIconStandPrefab.transform), true));

            if(i > 0)
            {
                matchingIcons[i].instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3((matchingIcons[i - 1].instance.GetComponent<Renderer>().bounds.size.x + paddingItemToRight) * i, 0, -2);
                matchingAreas[i].instance.transform.position = matchingAreaStandPrefab.transform.position + new Vector3((matchingAreaBoundSizeX + paddingItemToRight) * i,0,0);
            }
            
            i++;
        }
        firstIndex = 0;
        lastIndex = matchingIcons.Count - 1;
    }

    void RevertIconToFirstPlace(Collider2D iconToRevert)
    {
        int i = 0;
        foreach(MatchingItemValues item in matchingIcons)
        {
            if(item.instance != null)
            {
                if (item.instance.transform == iconToRevert.transform)
                {
                    if (i > 0 && matchingIcons[i-1].instance != null)
                        matchingIcons[i].instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3((matchingIcons[i - 1].instance.GetComponent<Renderer>().bounds.size.x + paddingItemToRight) * i, 0, -2);
                    else if(i == 0)
                        matchingIcons[i].instance.transform.position = new Vector3(matchingIconStandPrefab.transform.position.x, matchingIconStandPrefab.transform.position.y, -2);
                    else
                        matchingIcons[i].instance.transform.position = matchingIconStandPrefab.transform.position + new Vector3((item.instance.GetComponent<Renderer>().bounds.size.x + paddingItemToRight) * i, 0, -2);
                    heldIcon = false;
                    return;
                }
            }
            i++;
        }
    }
}
