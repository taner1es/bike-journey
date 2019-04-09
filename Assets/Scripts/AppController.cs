using UnityEngine;
using AppEnums;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class AppController : MonoBehaviour
{
    public bool mute;
    public static AppController instance = null; //singleton

    public ApplicationStates appState { get; set; }

    public Player player;

    public GameObject startMenuPanel;
    public GameObject storyMapPanel;
    public GameObject videoPanel;
    public GameObject ballonGameParent;
    public GameObject matchingGameParent;
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

        //Debug.Log("Screen Resolution :  " + Screen.currentResolution);

        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        //InitializeApp();
        StartForTesting();
    }

    void StartForTesting()
    {
        goDestination = new Destination(DestinationNames.School);
        //only startmenu panel works on initilization
        startMenuPanel.SetActive(false);

        //all other panels have been disabled 
        storyMapPanel.SetActive(false);
        videoPanel.SetActive(false);
        ballonGameParent.SetActive(false);
        matchingGameParent.SetActive(true);
    }

    public void InitializeApp()
    {
        //only startmenu panel works on initilization
        startMenuPanel.SetActive(true);

        //all other panels have been disabled 
        storyMapPanel.SetActive(false);
        videoPanel.SetActive(false);
        ballonGameParent.SetActive(false);
        matchingGameParent.SetActive(false);
    }

    //Play Button event on Start Menu Screen
    public void OnPlayButtonClicked() {
        startMenuPanel.SetActive(false);
        storyMapPanel.SetActive(true);

        player = new Player();
        appState = ApplicationStates.StoryMap;
    }

    //Destinate selection event on story map screen
    public void OnDestinationButtonClicked()
    {
        player.SelectDestination(EventSystem.current.currentSelectedGameObject);
    }

    //Go button event on story map to video panel
    public void OnGoButtonClicked()
    {
        goDestination = new Destination(player.selectedDestination);

        PrepareVideoClip();

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

        storyMapPanel.SetActive(false);
        videoPanel.SetActive(true);
    }

    public void Exit()
    {

    }

    public void ShowInstructions()
    {

    }

    public void ShowSettings()
    {

    }

    private void Update()
    {
        switch (appState.ToString())
        {
            case "Welcome":

                break;
            case "StoryMap":

                break;
            case "Video":

                break;
            case "BalloonGame":

                break;
            case "MatchingGame":

                break;
        }
    }

}
