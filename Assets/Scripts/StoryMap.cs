using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;

public class StoryMap : MonoBehaviour
{
    //public GameObject[] storyMapButtons = new GameObject[5]; /*cancelled*/
    public GameObject goButton;

    private void FixedUpdate()
    {
        ButtonHandler();
    }

    private void ButtonHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider;
            collider = goButton.GetComponent<Collider2D>();
            if (collider.OverlapPoint(inputPosition))
            {
                OnGoButtonClicked();
            }
            else
            {
                Debug.Log("Destination Selection Feature Disabled, Player can go only according to progress schedule.");
                /*
                foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("StoryMapDestination"))
                {
                    collider = iterator.GetComponent<Collider2D>();
                    if (collider.OverlapPoint(inputPosition))
                    {
                        if(Enum.TryParse<DestinationNames>(iterator.name, out AppController.instance.nextDestination))
                        {
                            Debug.Log("Next Destination Reassigned: " + AppController.instance.nextDestination);
                        }
                        else
                        {
                            Debug.Log("Insuccesfull Next Destination Assign Operation.");
                        }
                    }
                }*/
            }
        }
    }

    public void OnGoButtonClicked()
    {
        AppController.instance.goDestination = new Destination(AppController.instance.nextDestination);
        AppController.instance.PrepareVideoClip();
        AppController.instance.SetStage(ApplicationStates.VideoSection);
    }
}
