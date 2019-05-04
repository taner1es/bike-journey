using AppEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject progressSection;

    private void OnEnable()
    {
        progressSection.SetActive(false);
        ProgressController.UpdateProfileInfoBar();
    }

    public void OnProgressButtonClicked()
    {
        //toggle panel menu
        if(!progressSection.activeSelf)
            progressSection.SetActive(true);
    }

    //Play Button event on Start Menu Screen
    public void OnGoButtonClicked()
    {
        if(AppController.instance.currentPlayer != null && AppController.instance.allPlayerProgressData.playersList.Count > 0)
            AppController.instance.SetStage(ApplicationStates.StoryMap);
        else
        {
            ProgressController.CreateNewPlayer();

            AppController.instance.SetStage(ApplicationStates.StoryMap);

            Debug.Log("No Characted Found - New Character Created and Saved Automatically.");
        }
    }
}
