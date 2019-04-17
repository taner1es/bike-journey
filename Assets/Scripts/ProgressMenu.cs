using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ProgressMenu : MonoBehaviour
{
    public GameObject prefabPlayerSelectButton;
    public GameObject prefabPlayerList;
    public GameObject deleteApplyPanel;
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
        if(clickedPlayer != null)
        {
            if (AppController.instance.allPlayerProgressData.playersList.Contains(clickedPlayer))
            {
                deleteApplyPanel.SetActive(true);
                GameObject.FindGameObjectWithTag("AskForSure").GetComponent<TextMeshProUGUI>().text = "Are you sure to delete all data for <color=\"blue\">" + clickedPlayer.PlayerName + "</color>";
            }
            else
            {
                foreach (Player iterator in AppController.instance.allPlayerProgressData.playersList)
                {
                    Debug.Log(iterator.PlayerName + " vs " + clickedPlayer.PlayerName);
                }
            }
        }
    }

    public void OnYesDeleteButtonClicked()
    {
        if (AppController.instance.allPlayerProgressData.playersList.Remove(clickedPlayer))
            Debug.Log(clickedPlayer.PlayerName + " - Successfully Deleted");
        else
            Debug.Log(clickedPlayer.PlayerName + " - Can Not Be Removed");

        deleteApplyPanel.SetActive(false);

        ProgressController.SaveProgress();
        LoadPlayerList();
        clickedPlayer = null;
    }

    public void OnNoDeleteButtonClicked()
    {
        deleteApplyPanel.SetActive(false);
    }

    public void OnSwitchButtonClicked()
    {
        AppController.instance.currentPlayer = clickedPlayer;
        Debug.Log("currentPlayer : " + AppController.instance.currentPlayer.PlayerName);
    }

    public void OnLoadButtonClicked()
    {
        LoadPlayerList();
    }
    public void OnCreateButtonClicked()
    {
        LoadPlayerList();
        int id = PlayerProgress.idCounter;
        string name = "Player_" + id;
        AppController.instance.allPlayerProgressData.playersList.Add(new Player(name));
        ProgressController.SaveProgress();
        LoadPlayerList();
        if(GameObject.FindGameObjectWithTag("PlayerListScrollBar") != null)
            GameObject.FindGameObjectWithTag("PlayerListScrollBar").GetComponent<Scrollbar>().value = 0f;
    }

    public void OnClosePanelButtonClicked()
    {
        DestroyListedPlayers();
        clickedPlayer = null;
        this.gameObject.SetActive(false);
    }


    private void LoadPlayerList()
    {
        //remove old listed objects before listing new ones
        DestroyListedPlayers();

        bool loaded = ProgressController.LoadPlayerList();

        if (loaded && AppController.instance.allPlayerProgressData.playersList.Count > 0)
        {
            GameObject enabledButton = prefabPlayerSelectButton;

            //reload all again
            foreach (Player iterator in AppController.instance.allPlayerProgressData.playersList)
            {
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
    }

    private void DestroyListedPlayers()
    {
        foreach (Transform iterator in prefabPlayerList.GetComponentInChildren<Transform>())
        {
            Destroy(iterator.gameObject);
        }
    }
}
