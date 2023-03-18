using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource audioSource;

    void Start()
    {
        EvtSystem.EventDispatcher.AddListener<PlayAudio>(PlayAudioClip);
    }

    private void PlayAudioClip(PlayAudio data)
    {
        audioSource.Stop();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(data.clipToPlay);
        }
    }

}
