using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BalloonGameEvents : MonoBehaviour
{
    public class BalloonValues
    {
        public GameObject balloonInstance;
        public GameObject inflator;
        private Item item;
        public bool inflated;

        public BalloonValues(Item newItem, GameObject newInflator)
        {
            item = newItem;
            inflator = newInflator;
            inflated = false;
        }

        public void inflate(GameObject balloonPrefab, GameObject parent)
        {
            if (!inflated)
            {
                balloonInstance = Instantiate(balloonPrefab, parent.transform);
                balloonInstance.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f,1f,1f,1f,1f,1f,0.4f,0.7f);
                balloonInstance.transform.GetChild(0).GetComponent<TextMeshPro>().text = item.itemName;
                inflated = true;
                SoundManager.instance.PronounceItemName(item);
            }
        }
    }

    public GameObject balloonPrefab;
    public GameObject inflateParentPrefab;
    public GameObject inflatePrefab;

    private float maxThrust = 100;
    public float thrust { get; private set; }

    private int maxTopForce = 6;
    private int maxBottomForce = 4;

    public List<BalloonValues> balloons;

    Vector3 inputPosition;

    float inputInterval;
    bool clickedOnStand;

    private void OnEnable()
    {
        balloons = new List<BalloonValues>();
        clickedOnStand = false;
        CreateButtons();
    }

    void FixedUpdate()
    {
        CheckForBalloonClicked();
        CheckForInflateClicked();

        if(balloons != null && balloons.Count > 9)
            CheckForStandSlider();

        if (balloons != null)
            if(balloons.Count > 0)
                BalloonMovement();
    }

    //initialize all buttons for specific destination
    void CreateButtons()
    {
        foreach(GameObject iterator in GameObject.FindGameObjectsWithTag("Balloon"))
        {
            Destroy(iterator);
        }

        foreach(GameObject iterator in GameObject.FindGameObjectsWithTag("InflateSpriteButton"))
        {
            Destroy(iterator);
        }

        //create buttons dynamically
        foreach (Item item in AppController.instance.goDestination.items)
        {
            balloons.Add(new BalloonValues(item, Instantiate(inflatePrefab, inflateParentPrefab.transform)));
        }
    }

    //resize balloons on each iteration
    public void FitBalloonsSize()
    {
        if(balloons.Count > 1)
        {
            foreach (BalloonValues item in balloons)
            {
                if(item.balloonInstance != null)
                {
                    if(item.balloonInstance.transform.localScale.x > 2)
                    {
                        item.balloonInstance.transform.localScale += new Vector3(-1f, -1f, 0);
                    }
                }
            }
        }
    }

    //slider event
    void CheckForStandSlider()
    {
        SetInputIntervalOnStand();
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (clickedOnStand)
        {
            if (Input.GetMouseButton(0))
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider = inflateParentPrefab.GetComponent<Collider2D>();
                if (collider.OverlapPoint(inputPosition))
                {
                    if (balloons[0].inflator.transform.position.x < AppController.instance.cameraWidth && balloons[balloons.Count - 1].inflator.transform.position.x > -AppController.instance.cameraWidth)
                    {
                        inputPosition = new Vector3(inputPosition.x - inputInterval, inflateParentPrefab.transform.position.y, 0);
                        inflateParentPrefab.transform.position = inputPosition;
                    }
                    else
                    {
                        if (inflateParentPrefab.transform.position.x > 0)
                            inflateParentPrefab.transform.position = new Vector3(inflateParentPrefab.transform.position.x - 6f, inflateParentPrefab.transform.position.y, 0);
                        else
                            inflateParentPrefab.transform.position = new Vector3(inflateParentPrefab.transform.position.x + 6f, inflateParentPrefab.transform.position.y, 0);

                        clickedOnStand = false;
                    }
                }
                else
                {
                    clickedOnStand = false;
                }
            }
            else
            {
                clickedOnStand = false;
            }
        }
    }

    //slider interval calculator
    void SetInputIntervalOnStand()
    {
        if (!clickedOnStand)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D collider = inflateParentPrefab.GetComponent<Collider2D>();
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (collider.OverlapPoint(inputPosition))
                {
                    inputInterval = inputPosition.x - inflateParentPrefab.transform.position.x;
                    clickedOnStand = true;
                }
                else
                {
                    inputInterval = 0;
                    clickedOnStand = false;
                }
            }
        }
    }

    //check input for inflate
    void CheckForInflateClicked()
    {
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(balloons != null)
            {
                foreach (BalloonValues item in balloons) 
                {
                    Collider2D collider = item.inflator.GetComponent<Collider2D>();
                    if (collider.OverlapPoint(inputPosition) && !item.inflated)
                    {
                        item.inflate(balloonPrefab, gameObject);
                        item.inflator.GetComponent<Renderer>().material.color = item.balloonInstance.GetComponent<Renderer>().material.color;
                        FitBalloonsSize();
                        CheckForAllBalloonsHaveInflated();
                    }
                }
            }
        }
    }

    //check all balloons have inflated
    void CheckForAllBalloonsHaveInflated()
    {
        foreach(BalloonValues item in balloons)
        {
            if (item.inflated == false)
                return;
        }

        //if all balloons have inflated we can direct user to matching game
        Invoke("GoToMatchingGame", 1f);
    }

    //switch to matching game
    void GoToMatchingGame()
    {
        AppController.instance.SetStage(AppEnums.ApplicationStates.MatchingGame);
    }

    IEnumerator WaitInSeconds(float second)
    {
        yield return new WaitForSeconds(second);
    }
        //check input for dragging for all inflated balloons
        void CheckForBalloonClicked()
    {
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (Input.GetMouseButton(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(balloons != null)
            {
                foreach(BalloonValues item in balloons)
                {
                    if(item.balloonInstance != null)
                    {
                        Collider2D collider = item.balloonInstance.GetComponent<Collider2D>();
                        if (collider.OverlapPoint(inputPosition))
                        {
                            inputPosition.z = 0;
                            if (inputPosition.y > -3 && inputPosition.x > -AppController.instance.cameraWidth && inputPosition.x < AppController.instance.cameraWidth)
                                item.balloonInstance.transform.position = inputPosition;
                        }
                    }
                }
            }
        }
    }

    //physic event block for all inflated balloons
    void BalloonMovement()
    {
        foreach(BalloonValues item in balloons)
        {
            if(item.balloonInstance != null)
            {
                if (thrust < maxThrust && thrust > -maxThrust)
                {
                    if (item.balloonInstance.transform.position.y < maxBottomForce)
                    {
                        thrust = UnityEngine.Random.Range(0, maxThrust);
                    }
                    else if (item.balloonInstance.transform.position.y > maxTopForce)
                    {
                        thrust = UnityEngine.Random.Range(-maxThrust, 0);
                    }
                    else
                    {
                        thrust = UnityEngine.Random.Range(-maxThrust, maxThrust);
                    }
                }
                else
                {
                    thrust = 1;
                }

                item.balloonInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * thrust);
                item.balloonInstance.GetComponent<Rigidbody2D>().AddForce(transform.right * thrust);
            }
        }
    }
}
