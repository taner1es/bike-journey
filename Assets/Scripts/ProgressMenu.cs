using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressMenu : MonoBehaviour
{
    public GameObject prefabPlayerSelectButton;
    public GameObject prefabPlayerList;
    public GameObject deleteApplyPanel;
    public static Player clickedPlayer;

    public void OnDeleteButtonClicked()
    {
        if (AppController.instance.allPlayerProgressData.playersList.Contains(clickedPlayer))
        {
            deleteApplyPanel.SetActive(true);
            GameObject.FindGameObjectWithTag("AskForSure").GetComponent<TextMeshProUGUI>().text = "Are you sure to delete all data for <color=\"blue\">" + clickedPlayer.PlayerName + "</color>";
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
        int id = PlayerProgress.idCounter + 1;
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
        this.gameObject.SetActive(false);
    }


    private void LoadPlayerList()
    {
        //remove old listed objects before listing new ones
        DestroyListedPlayers();

        if (ProgressController.LoadPlayerList())
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
