using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoEvents : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    bool fired = false;


    private void OnEnable()
    {
        fired = false;
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.Play();
        videoPlayer.Pause();
        if (AppController.instance.mute)
            videoPlayer.GetComponent<VideoPlayer>().SetDirectAudioMute(0, true);
        else
            videoPlayer.GetComponent<VideoPlayer>().SetDirectAudioMute(0, false);
    }

    private void Update()
    {
        Debug.Log("videoPlayer.isPrepared : " + videoPlayer.isPrepared.ToString());
        if (!videoPlayer.isPlaying && videoPlayer.isPrepared && !fired)
        {
            videoPlayer.Play();
            fired = true;
        }
        Debug.Log(videoPlayer.frame.ToString() + " , " + videoPlayer.frameCount.ToString());
        if((ulong)videoPlayer.frame == videoPlayer.frameCount)
        {
            NextStage();
        }
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
        fired = false;
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    public void onButtonSkip()
    {
        NextStage();
    }
    void EndReached(VideoPlayer vp)
    {
        Invoke("NextStage", 2f);
    }
    void NextStage()
    {
        AppController.instance.videoPanel.SetActive(false);
        AppController.instance.ballonGameParent.SetActive(true);
    }
}
