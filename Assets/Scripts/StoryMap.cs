﻿using System;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;
using TMPro;
using UnityEngine.UI;

public class StoryMap : MonoBehaviour
{
    public TextMeshProUGUI textMeshProProgressBar;
    public Transform progressBarFiller;
    public GameObject gameFinishedPanel;
    public GameObject character;
    public GameObject progressPanelIcon;
    public TextMeshProUGUI textMeshProProgressInfoPanel;
    public GameObject watchButton;
    public GameObject[] paths;
    

    bool riding = false;

    List<Transform> icons = null;

    private bool continueButtonClicked;

    public static bool stay;

    private void Awake()
    {
        //load destination icons to memory
        icons = new List<Transform>();

        foreach (GameObject iterator in GameObject.FindGameObjectsWithTag("Destination"))
        {
            icons.Add(iterator.transform);
        }
    }

    private void OnEnable()
    {
        continueButtonClicked = false;
        ApplyProgress();
        LocateCharacter();
        PrepareProgressInfoPanel();
    }

    private void PrepareProgressInfoPanel()
    {
        string spritePath = "Textures/Destinations/" + AppController.instance.currentPlayer.Destination;
        var texture = Resources.Load<Texture2D>(spritePath);
        progressPanelIcon.GetComponent<Image>().sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;

        int numberOfItemLeft, numberOfItemLearned;

        numberOfItemLearned = AppController.instance.currentPlayer.LearnedItems.FindAll(e => e.itemDestination == AppController.instance.currentPlayer.Destination).Count;

        int cnt = 0;
        foreach (Item iterator in AppController.instance.dataController.allItemData)
        {
            if (iterator.itemDestination == AppController.instance.currentPlayer.Destination)
                cnt++;
        }

        numberOfItemLeft = cnt - numberOfItemLearned;

        textMeshProProgressInfoPanel.SetText(AppController.instance.currentPlayer.Destination + "\n <color=\"red\">" + numberOfItemLearned + "/" + cnt + "</color>");

        if (stay)
            watchButton.SetActive(true);
        else
            watchButton.SetActive(false);
        
    }

    private void LocateCharacter()
    {
        if (icons.Count > 0)
        {
            int index = (int)Enum.Parse(typeof(DestinationNames), AppController.instance.currentPlayer.Destination);

            if (!stay)
                character.transform.position = icons[index].position;
            else
                character.transform.position = icons[index+1].position;

            character.transform.rotation = Quaternion.identity;
        }
    }

    private void ApplyProgress()
    {
        stay = true;

        AppController.instance.currentPlayer.SetGoDest();

        AppController.instance.currentPlayer.CalculateProgressPercentage();

        textMeshProProgressBar.text = "%" + AppController.instance.currentPlayer.ProgressPercentage/* + " Finished ("
            + AppController.instance.currentPlayer.LearnedItems.Count.ToString() + " of "
            + AppController.instance.dataController.allItemData.Length.ToString() + " words have learned)"*/;

        progressBarFiller.localScale = new Vector3(AppController.instance.currentPlayer.ProgressPercentage, 1, 1);

        ProgressController.SaveProgress();

        if (AppController.instance.currentPlayer.ProgressPercentage >= 100)
        {
            gameFinishedPanel.SetActive(true);
        }
        else
        {
            gameFinishedPanel.SetActive(false);
        }
    }

    public void OnBackButtonClicked()
    {
        riding = false;
        Follow.stop = true;
        AppController.instance.SetState(ApplicationStates.StartMenu);
    }

    public void OnWatchVideoClicked()
    {
        if (stay)
        {
            AppController.instance.SetState(ApplicationStates.VideoSection);
        }
    }

    public void OnContinueButtonClicked()
    {
        if (Follow.stop)
        {
            RideTheBike();
        }
        continueButtonClicked = true;
    }

    private void RideTheBike()
    {
        if (!stay)
        {
            int index = (int)Enum.Parse(typeof(DestinationNames), AppController.instance.currentPlayer.Destination);
            character.GetComponent<Follow>().path = paths[index];
            Follow.stop = false;
            Follow.currentTargetIndex = 0;
            riding = true;
        }
    }

    private void Update()
    {
        if (continueButtonClicked)
        {
            if (stay)
            {
                AppController.instance.SetState(ApplicationStates.BalloonGame);
            }
            else if (riding && Follow.stop)//waits for riding until finishing all the path before changing state to video section
            {
                AppController.instance.SetState(ApplicationStates.VideoSection);
                riding = false;
            }
        }
    }

    public void OnPlayAgainClicked()
    {
        ProgressController.SaveProgress();
        ProgressController.CreateNewPlayer();
        gameFinishedPanel.SetActive(false);
        AppController.instance.SetState(ApplicationStates.StartMenu);
        Debug.Log("Restart : Old Character saved as finished, new character created to play from begin.");
    }

}
