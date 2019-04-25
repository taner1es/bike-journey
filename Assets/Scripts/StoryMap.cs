using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppEnums;

public class StoryMap : MonoBehaviour
{
    public void OnBackButtonClicked()
    {
        AppController.instance.SetStage(ApplicationStates.StartMenu);
    }

    public void OnContinueButtonClicked()
    {
        AppController.instance.goDestination = new Destination(AppController.instance.nextDestination);
        AppController.instance.PrepareVideoClip();
        AppController.instance.SetStage(ApplicationStates.VideoSection);
    }
}
