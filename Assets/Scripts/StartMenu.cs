using AppEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    public GameObject[] startMenuButtons = new GameObject[3];
    public GameObject progressSectionCanvas;

    private void OnEnable()
    {
        progressSectionCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        
    }

    private void FixedUpdate()
    {
        ButtonHandler();   
    }

    void ButtonHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider;
            foreach (GameObject iterator in startMenuButtons)
            {
                if (iterator != null)
                {
                    collider = iterator.GetComponent<Collider2D>();
                    if (collider.OverlapPoint(inputPosition))
                    {
                        string buttonName = iterator.name;
                        switch (buttonName)
                        {
                            case "go":
                                OnGoButtonClicked();
                                break;
                            case "progress":
                                OnProgressButtonClicked();
                                break;
                            case "quit":
                                OnQuitButtonClicked();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public void OnProgressButtonClicked()
    {
        //toggle panel menu
        if(!progressSectionCanvas.activeSelf)
            progressSectionCanvas.SetActive(true);
    }

    //Exit Button event on Start Menu Screen
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    //Play Button event on Start Menu Screen
    public void OnGoButtonClicked()
    {
        if(AppController.instance.currentPlayer != null)
            AppController.instance.SetStage(ApplicationStates.StoryMap);
        else
        {
            
        }
    }

}
