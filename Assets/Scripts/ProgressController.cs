using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public static class ProgressController
{
    public static string progressFilePath = Application.persistentDataPath + "/gameprogress.save";

    public static bool LoadPlayerList()
    {
        if (File.Exists(progressFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(progressFilePath, FileMode.Open);
            AppController.instance.allPlayerProgressData = (PlayerProgress)bf.Deserialize(file);
            file.Close();

            PlayerProgress.idCounter = AppController.instance.allPlayerProgressData.idCounterSaved;

            Debug.Log("Player List Succesfully Loaded");
            return true;
        }
        else
        {
            Debug.Log("Player List Not Found !");
            return false;
        }
    }

    public static bool SetCurrentPlayer(int playerID)
    {
        if (File.Exists(progressFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(progressFilePath, FileMode.Open);
            AppController.instance.allPlayerProgressData = (PlayerProgress)bf.Deserialize(file);
            file.Close();

            foreach(Player iterator in AppController.instance.allPlayerProgressData.playersList)
            {
                if (playerID == iterator.PlayerID)
                {
                    AppController.instance.currentPlayer = iterator;
                    Debug.Log("Loaded Player : " + iterator.PlayerID + " / " + iterator.PlayerName + " / amount of known item: " + iterator.LearnedItems.Count);
                    break;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SaveProgress()
    {
        if(AppController.instance.allPlayerProgressData != null)
        {
            AppController.instance.allPlayerProgressData.idCounterSaved = PlayerProgress.idCounter;
            if(AppController.instance.currentPlayer != null)
                AppController.instance.allPlayerProgressData.lastSessionPlayerId = AppController.instance.currentPlayer.PlayerID;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(progressFilePath);
            bf.Serialize(file, AppController.instance.allPlayerProgressData);
            file.Close();
            Debug.Log("Progress Saved : " + progressFilePath);
        }
    }

    public static void CreateNewPlayer(string name = "no-name")
    {
        AppController.instance.allPlayerProgressData = new PlayerProgress(name);
        Debug.Log("New Player Created");
        SaveProgress();
    }

    public static bool LoadLastSession()
    {
        if (LoadPlayerList())
        {
            Debug.Log("Loaded - Last Session ID : " + AppController.instance.allPlayerProgressData.lastSessionPlayerId);
            if (SetCurrentPlayer(AppController.instance.allPlayerProgressData.lastSessionPlayerId))
            {
                UpdateProfileInfoBar();
                return true;
            }
            else
            {
                Debug.Log("Last Lession Not Found!");
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static void UpdateProfileInfoBar()
    {
        TextMeshProUGUI infoBar = GameObject.FindGameObjectWithTag("TMPLoadedProfileInfoBar").GetComponent<TextMeshProUGUI>();
        if (AppController.instance.currentPlayer != null)
            infoBar.text = AppController.instance.currentPlayer.PlayerID.ToString() + " - " + AppController.instance.currentPlayer.PlayerName + ". Loaded!";
        else
            infoBar.text = "NOT A PROFILE FOUND";

        Debug.Log("ProfileInfo Updated !");
    }

    public static string GetPlayerDestination(Player player)
    {

        return "";
    }
}
