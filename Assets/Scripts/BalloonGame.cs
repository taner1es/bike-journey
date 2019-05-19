using AppEnums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalloonGame : MonoBehaviour
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
            inflator.name = newItem.itemName;
            inflated = false;
        }

        public void inflate(GameObject balloonPrefab, GameObject parent)
        {
            if (!inflated)
            {
                balloonInstance = Instantiate(balloonPrefab, parent.transform);
                balloonInstance.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 0.4f, 0.7f);
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

    private void OnEnable()
    {
        balloons = new List<BalloonValues>();
        CreateButtons();
    }

    void Update()
    {
        CheckForBalloonClicked();
        CheckForInflateClicked();

        if (balloons != null)
            if (balloons.Count > 0)
                BalloonMovement();
    }

    private void CreateButtons()
    {
        foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("Balloon"))
        {
            Destroy(iterator);
        }

        foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("InflateButton"))
        {
            Destroy(iterator);
        }

        balloons.Clear();

        //create buttons dynamically
        foreach (Item item in AppController.instance.goDestination.items)
        {
            balloons.Add(new BalloonValues(item, Instantiate(inflatePrefab, inflateParentPrefab.transform)));
        }
    }

    //check input for inflate
    void CheckForInflateClicked()
    {
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (balloons != null)
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
        foreach (BalloonValues item in balloons)
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
        AppController.instance.SetState(AppEnums.ApplicationStates.MatchingGame);
    }
    //check input for dragging for all inflated balloons
    void CheckForBalloonClicked()
    {
        //PC,MAC,BROWSER INPUT BY MOUSE
        if (Input.GetMouseButton(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (balloons != null)
            {
                foreach (BalloonValues item in balloons)
                {
                    if (item.balloonInstance != null)
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

    //resize balloons on each iteration
    public void FitBalloonsSize()
    {
        if (balloons.Count > 1)
        {
            foreach (BalloonValues item in balloons)
            {
                if (item.balloonInstance != null)
                {
                    if (item.balloonInstance.transform.localScale.x > 2)
                    {
                        item.balloonInstance.transform.localScale += new Vector3(-1f, -1f, 0);
                    }
                }
            }
        }
    }

    //physic event block for all inflated balloons
    void BalloonMovement()
    {
        foreach (BalloonValues item in balloons)
        {
            if (item.balloonInstance != null)
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

    public void OnBackButtonClicked()
    {
        AppController.instance.SetState(ApplicationStates.StoryMap);
    }
}
