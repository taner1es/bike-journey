using UnityEngine;
using AppEnums;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class AppController : MonoBehaviour
{
    public bool mute;
    public static AppController instance = null; //singleton

    public ApplicationStates appState { get; set; }

    public DestinationNames nextDestination;

    public PlayerProgress allPlayerProgressData; 
    public Player currentPlayer;

    public GameObject[] stages = new GameObject[5];
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

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
        currentPlayer = null;

        //save file path
        Debug.Log(Application.persistentDataPath);

        //holding camera values
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        //sets active screen to welcome
        SetStage(ApplicationStates.StartMenu);

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
    public void SetStage(ApplicationStates stageToSet)
    {
        //goDestination = new Destination(DestinationNames.School);
        foreach(GameObject iterator in stages)
        {
            if (iterator.name != stageToSet.ToString())
            {
                iterator.SetActive(false);
            }
            else
            {
                iterator.SetActive(true);
                appState = stageToSet;
            }
        }
    }

    public void PrepareVideoClip()
    {
        string destination = goDestination.destinationName.ToString();
        
        switch (destination)
        {
            case "School":
                videoPlayer.clip = videoClips[0];
                break;
            case "Camp":
                videoPlayer.clip = videoClips[1];
                break;
            default:
                videoPlayer.clip = videoClips[0];
            break;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.Prepare();
    }
}
