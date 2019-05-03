using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null; //singleton

    List<AudioClip> bgMusics = new List<AudioClip>();
    List<AudioSource> sources = new List<AudioSource>();

    bool bgPaused = false;

    void Awake()
    {
        //singleton initialization
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);

        DontDestroyOnLoad(instance);

        //audio sources initialization
        Initialize();
    }

    private void Update()
    {
        BackgroundMusic();
    }

    private void BackgroundMusic()
    {
        //pause music on video section and allow for all other states
        if(AppController.instance.appState == AppEnums.ApplicationStates.VideoSection)
        {
            if(!bgPaused)
            {
                sources[0].Pause();
                bgPaused = true;
            }
        }
        else
        {
            if (bgPaused)
            {
                sources[0].UnPause();
                bgPaused = false;
            }
            else if (!sources[0].isPlaying)
            {
                System.Random random = new System.Random();
                int i = random.Next(0, bgMusics.Count);

                sources[0].clip = bgMusics[i];
                sources[0].Play();
            }
        }
    }

    private void Initialize()
    {
        foreach(AudioSource iterator in gameObject.GetComponents<AudioSource>())
        {
            sources.Add(iterator);
        }

        foreach(AudioClip iterator in Resources.LoadAll<AudioClip>("Audios/Musics/"))
        {
            bgMusics.Add(iterator);
        }
    }

    public void PronounceItemName(Item item)
    {
        AudioClip aClip = Resources.Load<AudioClip>("Audios/Items/" + item.itemName);
        if(aClip != null)
            sources[1].PlayOneShot(aClip);
        else
            Debug.LogError("pronounciation clip couldn't be loaded: aClip:null");
    }
}
