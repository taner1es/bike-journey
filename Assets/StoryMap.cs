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
        }
    }

    public void OnGoButtonClicked()
    {
        AppController.instance.goDestination = new Destination(AppController.instance.nextDestination);
        AppController.instance.PrepareVideoClip();
        AppController.instance.SetStage(ApplicationStates.VideoSection);
    }

    // this section makes possible to select a destination on story map but for now this feature cancelled 
    //(reason: progress system applied, user should player in order for each destination)
    //(remainder: if this section decided to reuse again, dont forget to add collider component to destination sprites on story map

    /*void ButtonHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider;
            foreach (GameObject button in storyMapButtons)
            {
                if (button != null)
                {
                    collider = button.GetComponent<Collider2D>();
                    if (collider.OverlapPoint(inputPosition))
                    {
                        foreach (string destination in Enum.GetNames(typeof(DestinationNames)))
                        {
                            if (button.name == destination.ToString())
                                SelectDestination(button.name);
                        }
                    }
                }
            }
        }
    }*/

    //Destinate selection event on story map screen
    /*public void SelectDestination(string destinationName)
    {
        foreach (DestinationNames iterator in (DestinationNames[])Enum.GetValues(typeof(DestinationNames)))
        {
            if(iterator.ToString() == destinationName)
            {
                AppController.instance.selectedDestination = iterator;
                return;
            }
        }
        Debug.LogError("Destination not found : " + destinationName);
    }*/
}
