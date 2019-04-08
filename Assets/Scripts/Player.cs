using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Player
{
    public DestinationNames currentDestination;
    public DestinationNames selectedDestination;

    public void SelectDestination(GameObject clickedButtonGameObject)
    {
        Button clickedButton = clickedButtonGameObject.GetComponent<Button>();

        //checks for user at storymap and null verification
        if (AppController.instance.appState == ApplicationStates.StoryMap && EventSystem.current.currentSelectedGameObject != null)
        {
            ButtonEvent(clickedButton);   
        }
    }

    public void ButtonEvent(Button clickedButton)
    {
        //modify clicked button to attract to user
        /*
            Color x = new Color(255f, 255f, 255f, 1);
            ColorBlock cb = clickedButton.GetComponent<Button>().colors;
            cb.highlightedColor = x;
            clickedButton.GetComponent<Button>().colors = cb;
        */
        //update selectedDestination
        string temp;
        temp = clickedButton.name;
        temp = temp.Substring(6);

        if (!Enum.TryParse<DestinationNames>(temp, out selectedDestination))
            Debug.LogError("DestinationName can't find in the enum set for clickedButton", clickedButton);
    }
}
