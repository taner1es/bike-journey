﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ProgressMenu : MonoBehaviour
{
    public GameObject prefabButtonCurrentPlayer;
    public GameObject prefabPlayerSelectButton;
    public GameObject prefabPlayerList;
    public GameObject deleteApplyPanel;
    public TextMeshProUGUI progressInfoTMP;

    public static Player clickedPlayer;

    public void OnPlayerSelectButtonClicked()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;

        if (button.CompareTag("ProgressMenuPlayerSelectButton"))
        {
            char[] numbers = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
            foreach (Player iterator in AppController.instance.allPlayerProgressData.playersList)
            {
                string trimmed = button.name.Trim(numbers);
                if (trimmed == iterator.PlayerID.ToString())
                {
                    clickedPlayer = iterator;
                    Debug.Log("clicked player : " + clickedPlayer.PlayerName);
                    return;
                }
            }
        }
    }

    public void OnDeleteButtonClicked()
    {
        if(AppController.instance.currentPlayer != null)
        {
            if (AppController.instance.allPlayerProgressData.playersList.Exists(e => e.PlayerID == AppController.instance.currentPlayer.PlayerID))
            {
                deleteApplyPanel.SetActive(true);
                GameObject.FindGameObjectWithTag("AskForSure").GetComponent<TextMeshProUGUI>().text = "Are you sure to delete all data for <color=\"blue\">" + AppController.instance.currentPlayer.PlayerName + "</color>";
            }
        }
    }

    public void OnYesDeleteButtonClicked()
    {
        Player plyr = AppController.instance.allPlayerProgressData.playersList.Find(e => e.PlayerID == AppController.instance.currentPlayer.PlayerID);
        //int index = AppController.instance.allPlayerProgressData.playersList.FindIndex(e => e.PlayerID == AppController.instance.currentPlayer.PlayerID);
        if (AppController.instance.allPlayerProgressData.playersList.Remove(plyr))
            Debug.Log(AppController.instance.currentPlayer.PlayerName + " - Successfully Deleted");
        else
            Debug.Log(AppController.instance.currentPlayer.PlayerName + " - Can Not Be Removed");

        deleteApplyPanel.SetActive(false);

        ProgressController.SaveProgress();

        if (AppController.instance.allPlayerProgressData.playersList.Count > 0)
            AppController.instance.currentPlayer = AppController.instance.allPlayerProgressData.playersList[0];
        clickedPlayer = null;
        LoadPlayerList();
    }

    public void OnNoDeleteButtonClicked()
    {
        deleteApplyPanel.SetActive(false);
    }

    public void OnSwitchButtonClicked()
    {
        SwitchCharacter(clickedPlayer);
    }

    private void SwitchCharacter(Player switchTo)
    {
        if (switchTo != null)
        {
            AppController.instance.currentPlayer = switchTo;
            clickedPlayer = switchTo;
            ProgressController.UpdateProfileInfoBar();
            ProgressController.SaveProgress();
            Debug.Log("currentPlayer : " + AppController.instance.currentPlayer.PlayerName);
        }
        else
        {
            Debug.Log("there is no clicked player, please click first.");
        }

        LoadPlayerList();
    }

    private void ShowProgressInfoForCurrentPlayer()
    {
        string text = "";

        if(AppController.instance.currentPlayer != null)
        {
            text = "ID: " + "<color=\"red\">" + AppController.instance.currentPlayer.PlayerID.ToString() + "</color>";
            text += "\nName: " + "<color=\"red\">" + AppController.instance.currentPlayer.PlayerName + "</color>";
            text += "\nLearned Words:\n\n";
            text += "<color=\"yellow\"><align=\"center\">";

            if (AppController.instance.currentPlayer.LearnedItems != null && AppController.instance.currentPlayer.LearnedItems.Count > 0)
            {

                foreach (Item iterator in AppController.instance.currentPlayer.LearnedItems)
                {
                    text += iterator.itemName + "\n";
                }
                text += "\n<size=90%>" + AppController.instance.currentPlayer.LearnedItems.Count + " words have learned.";
            }
            else
            {
                text += "<size=90%>No Words Have Learned Yet. You Are Ready to Learn New Words, Let's Begin..";
            }
            text += "</align></color>";
        }
        else
        {
            text = "No Character Found, You Can Create One.";
        }

        progressInfoTMP.text = text;
    }

    private void OnEnable()
    {
        LoadPlayerList();
    }

    public void OnCreateButtonClicked()
    {
        int id = PlayerProgress.idCounter;
        string name = "Player_" + id;
        AppController.instance.allPlayerProgressData.playersList.Add(new Player(name));
        ProgressController.SaveProgress();
        
        if(GameObject.FindGameObjectWithTag("PlayerListScrollBar") != null)
            GameObject.FindGameObjectWithTag("PlayerListScrollBar").GetComponent<Scrollbar>().value = 0f;

        SwitchCharacter(AppController.instance.allPlayerProgressData.playersList[AppController.instance.allPlayerProgressData.playersList.Count - 1]);
    }

    public void OnClosePanelButtonClicked()
    {
        DestroyListedPlayers();
        clickedPlayer = null;
        deleteApplyPanel.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        //enable start menu buttons collisions again
        foreach(GameObject iterator in GameObject.FindGameObjectsWithTag("StartMenuButton"))
        {
            iterator.GetComponent<Collider2D>().enabled = true;
        }
    }

    private void LoadPlayerList()
    {
        //remove old listed objects before listing new ones
        DestroyListedPlayers();

        bool loaded = ProgressController.LoadPlayerList();

        if (loaded && AppController.instance.allPlayerProgressData.playersList.Count > 0)
        {
            GameObject enabledButton;

            //reload all again
            foreach (Player iterator in AppController.instance.allPlayerProgressData.playersList)
            {
                if (iterator.PlayerID == AppController.instance.currentPlayer.PlayerID)
                    enabledButton = prefabButtonCurrentPlayer;
                else
                    enabledButton = prefabPlayerSelectButton;

                enabledButton.GetComponentInChildren<TextMeshProUGUI>().text = iterator.PlayerName;
                enabledButton.GetComponent<Button>().interactable = true;
                enabledButton.name = iterator.PlayerID.ToString();
                Instantiate(enabledButton, prefabPlayerList.transform);
            }
        }
        else
        {
            GameObject disabledButton = prefabPlayerSelectButton;
            disabledButton.GetComponent<Button>().interactable = false;
            disabledButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player List Not Found";
            Instantiate(disabledButton, prefabPlayerList.transform);
        }

        ShowProgressInfoForCurrentPlayer();
    }

    private void DestroyListedPlayers()
    {
        foreach (Transform iterator in prefabPlayerList.GetComponentInChildren<Transform>())
        {
            Destroy(iterator.gameObject);
        }
    }
}
