using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoEvents : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.GetComponent<VideoPlayer>().Play();
        if (AppController.instance.mute)
            videoPlayer.GetComponent<VideoPlayer>().SetDirectAudioMute(0, true);
        else
            videoPlayer.GetComponent<VideoPlayer>().SetDirectAudioMute(0, false);
    }

    public void onButtonPause()
    {
        if(videoPlayer.isPlaying)
            videoPlayer.Pause();
    }

    public void onButtonResume()
    {
        if (videoPlayer.isPaused)
            videoPlayer.Play();
    }

    public void onButtonReplay()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void onButtonSkip()
    {
        AppController.instance.videoPanel.SetActive(false);
        AppController.instance.ballonGameParent.SetActive(true);
    }
}
