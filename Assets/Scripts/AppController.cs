using UnityEngine;
using AppEnums;

public class AppController : MonoBehaviour
{
    public bool mute;
    public static AppController instance = null; //singleton

    public ApplicationStates appState { get; set; }

    public PlayerProgress allPlayerProgressData; 
    public Player currentPlayer;

    public GameObject[] stateParents = new GameObject[5];

    private string currentAppState;
    public Destination goDestination;

    public DataController dataController;

    //Camera Values
    public float cameraHeight;
    public float cameraWidth;

    private void Awake()
    {
        //singleton initialization
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);

        DontDestroyOnLoad(instance);

        //set null current player on game awaked
        AppController.instance.currentPlayer = null;
        AppController.instance.allPlayerProgressData = null;

        //save file path
        Debug.Log(Application.persistentDataPath);

        //holding camera values
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        //sets active screen to welcome
        SetState(ApplicationStates.StartMenu); 

        //load game data from JSON
        AppController.instance.dataController = new DataController();
        AppController.instance.dataController.LoadGameData();

        //tries to load saved game data if any exists from previous session(s)
        ProgressController.LoadLastSession();
    }

    private void OnApplicationQuit()
    {
        if(currentPlayer != null)
        {
            allPlayerProgressData.lastSessionPlayerId = currentPlayer.PlayerID;
            Debug.Log("Saved - Last Session Id : " + allPlayerProgressData.lastSessionPlayerId);
            ProgressController.SaveProgress();
        }
        else
        {
            Debug.Log("No currentPlayer Found and progress not saved.");
        }
    }
    public void SetState(ApplicationStates stateToSet)
    {
        foreach(GameObject iterator in stateParents)
        {
            if (iterator.name != stateToSet.ToString())
            {
                iterator.SetActive(false);
            }
            else
            {
                iterator.SetActive(true);
                appState = stateToSet;
            }
        }
    }

    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
