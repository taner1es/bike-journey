using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

public class VideoEvents : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    bool fired;

    private void Awake()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    private void OnEnable()
    {
        PrepareVideoPlayer();
    }

    private void PrepareVideoPlayer()
    {
        fired = false;

        if (videoPlayer.clip != null)
            videoPlayer.clip = null;
        
        VideoClip vclip = Resources.Load<VideoClip>("Videos/" + AppController.instance.currentPlayer.Destination);

        videoPlayer.clip = vclip;
        videoPlayer.Prepare();
    }

    private void OnDisable()
    {
        videoPlayer.clip = null;
    }

    private void Update()
    {
        if (!videoPlayer.isPlaying && videoPlayer.isPrepared && !fired)
        {
            videoPlayer.Play();
            fired = true;
        }   
    }

    public void OnButtonPause()
    {
        if(videoPlayer.isPlaying)
            videoPlayer.Pause();
    }

    public void OnButtonResume()
    {
        if (videoPlayer.isPaused)
            videoPlayer.Play();
    }

    public void OnButtonReplay()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void OnButtonSkip()
    {
        NextState();
    }
    void EndReached(VideoPlayer vp)
    {
        videoPlayer.clip = null;
        Invoke("NextState", 1f);
    }
    void NextState()
    {
        AppController.instance.SetState(AppEnums.ApplicationStates.BalloonGame);
    }
}
