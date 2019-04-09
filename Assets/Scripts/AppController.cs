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
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);

        DontDestroyOnLoad(instance);

        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        SetStage(ApplicationStates.StartMenu);
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
