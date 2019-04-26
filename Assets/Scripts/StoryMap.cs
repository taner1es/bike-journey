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
        DestinationNames dest;
        if(Enum.TryParse<DestinationNames>(AppController.instance.currentPlayer.Destination, true, out dest))
        {
            AppController.instance.goDestination = new Destination(dest);
            AppController.instance.PrepareVideoClip();
            AppController.instance.SetStage(ApplicationStates.VideoSection);
        }
        else
        {
            Debug.LogError("Destination not found : " + dest.ToString(), this);
        }
    }
}
