using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;
using TMPro;

public class StoryMap : MonoBehaviour
{
    public TextMeshProUGUI textMeshProProgressBar;
    public Transform progressBarFiller;
    public GameObject gameFinishedPanel;

    private void OnEnable()
    {
        ApplyProgress();
    }

    private void ApplyProgress()
    {
        AppController.instance.currentPlayer.CalculateProgress();

        textMeshProProgressBar.text = "%" + AppController.instance.currentPlayer.ProgressPercentage + " Finished ("
            + AppController.instance.currentPlayer.LearnedItems.Count.ToString() + " of "
            + AppController.instance.dataController.allItemData.Length.ToString() + " words have learned)";

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
        AppController.instance.SetState(ApplicationStates.StartMenu);
    }

    public void OnContinueButtonClicked()
    {
        AppController.instance.currentPlayer.SetGoDest();
        AppController.instance.SetState(ApplicationStates.VideoSection);
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
