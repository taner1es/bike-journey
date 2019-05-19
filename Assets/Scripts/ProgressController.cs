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
            {
                AppController.instance.allPlayerProgressData.lastSessionPlayerId = AppController.instance.currentPlayer.PlayerID;

                //update progress information for current session in playerlist then serialize to binary
                int index = AppController.instance.allPlayerProgressData.playersList.FindIndex(e => e.PlayerID == AppController.instance.currentPlayer.PlayerID);
                AppController.instance.allPlayerProgressData.playersList[index] = AppController.instance.currentPlayer;
            }
                

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(progressFilePath);
            bf.Serialize(file, AppController.instance.allPlayerProgressData);
            file.Close();
            Debug.Log("Progress Saved : " + progressFilePath);
        }
    }

    public static void CreateNewPlayerProgress()
    {
        AppController.instance.allPlayerProgressData = new PlayerProgress();
        SaveProgress();
    }
    
    public static void CreateNewPlayer()
    {
        int id = PlayerProgress.idCounter;
        string name = "Player_" + id;

        if (AppController.instance.allPlayerProgressData == null)
            CreateNewPlayerProgress();

        AppController.instance.allPlayerProgressData.playersList.Add(new Player(name));

        SwitchCharacter(AppController.instance.allPlayerProgressData.playersList[AppController.instance.allPlayerProgressData.playersList.Count - 1]);
        SaveProgress();

        Debug.Log("New Character Created, Switched and Saved.");
    }

    public static bool DeleteCurrentCharacter()
    {
        if(AppController.instance.currentPlayer != null)
        {
            Player plyr = AppController.instance.allPlayerProgressData.playersList.Find(e => e.PlayerID == AppController.instance.currentPlayer.PlayerID);
            if (AppController.instance.allPlayerProgressData.playersList.Remove(plyr))
            {
                Debug.Log(AppController.instance.currentPlayer.PlayerName + " - Successfully Deleted");
                if (AppController.instance.allPlayerProgressData.playersList.Count > 0)
                    AppController.instance.currentPlayer = AppController.instance.allPlayerProgressData.playersList[0];
                else
                    AppController.instance.currentPlayer = null;

                UpdateProfileInfoBar();
                SaveProgress();
                return true;
            }

            else
            {
                Debug.LogError(AppController.instance.currentPlayer.PlayerName + " - Can Not Be Removed");
                return false;
            }
        }
        else
        {
            return false;
        }
            

    }

    public static void SwitchCharacter(Player playerToSwitch)
    {
        if (playerToSwitch != null)
        {
            AppController.instance.currentPlayer = playerToSwitch;
            UpdateProfileInfoBar();
            SaveProgress();
            Debug.Log("currentPlayer : " + AppController.instance.currentPlayer.PlayerName);
        }
        else
        {
            Debug.Log("Couldn't switched");
        }
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
        GameObject infoBar;
        if (infoBar = GameObject.FindGameObjectWithTag("TMPLoadedProfileInfoBar"))
        {
            TextMeshProUGUI infoBarTMP = infoBar.GetComponent<TextMeshProUGUI>();
            if (AppController.instance.currentPlayer != null)
                infoBarTMP.text = AppController.instance.currentPlayer.PlayerID.ToString() + " - " + AppController.instance.currentPlayer.PlayerName + ". Loaded!";
            else
                infoBarTMP.text = "NOT A PROFILE FOUND";

            Debug.Log("ProfileInfo Updated !");
        }
    }

    public static string GetPlayerDestination(Player player)
    {

        return "";
    }
}
