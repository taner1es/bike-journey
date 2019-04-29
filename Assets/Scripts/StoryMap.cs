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
    }

    public void OnBackButtonClicked()
    {
        AppController.instance.SetStage(ApplicationStates.StartMenu);
    }

    public void OnContinueButtonClicked()
    {
        AppController.instance.currentPlayer.GoDest();

        if (AppController.instance.videoPlayer.clip != null)
        {
            AppController.instance.PrepareVideoClip();
            AppController.instance.SetStage(ApplicationStates.VideoSection);
        }
        else
        {
            AppController.instance.SetStage(ApplicationStates.BalloonGame);
        }
    }
}
